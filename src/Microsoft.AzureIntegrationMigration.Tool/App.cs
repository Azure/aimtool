using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AzureIntegrationMigration.Runner;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Engine;
using Microsoft.AzureIntegrationMigration.Runner.Model;
using Microsoft.AzureIntegrationMigration.Tool.Commands;
using Microsoft.AzureIntegrationMigration.Tool.Logging;
using Microsoft.AzureIntegrationMigration.Tool.Options;
using Microsoft.AzureIntegrationMigration.Tool.Plugins;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// Defines the implementation of the application.
    /// </summary>
    public class App : IApp
    {
        /// <summary>
        /// Defines a unique ID generator.
        /// </summary>
        private readonly IUniqueIdGenerator _generator;

        /// <summary>
        /// Defines the migration runner builder to use to build a runner instance
        /// based on command line options.
        /// </summary>
        private readonly IRunnerBuilder _builder;

        /// <summary>
        /// Defines the stage component provider to find stage runners.
        /// </summary>
        private readonly IStageComponentProvider _provider;

        /// <summary>
        /// Defines the stage component provider to find model provider.
        /// </summary>
        private readonly PluginModelComponentProvider _modelProvider;

        /// <summary>
        /// Defines the plugin host where plugin assemblies are loaded.
        /// </summary>
        private readonly IPluginHost<IRunnerComponent> _pluginHost;

        /// <summary>
        /// Defines the command line options.
        /// </summary>
        private readonly IAppOptionsService _options;

        /// <summary>
        /// Defines the logging service.
        /// </summary>
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Defines the logger for the runner to use.
        /// </summary>
        private readonly ILogger<Runner.Engine.Runner> _runnerLogger;

        /// <summary>
        /// Defines the logger to use.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs an instance of the <see cref="App"/> class with its dependencies.
        /// </summary>
        /// <param name="generator">The unique ID generator.</param>
        /// <param name="builder">The migration runner builder.</param>
        /// <param name="provider">The provider used to find stage runners.</param>
        /// <param name="modelProvider">The provider used to find model provider.</param>
        /// <param name="pluginHost">The stage runner plugin host.</param>
        /// <param name="options">The command line options.</param>
        /// <param name="loggingService">The logging service.</param>
        /// <param name="runnerLogger">The logger for the runner.</param>
        /// <param name="logger">The logger for the app.</param>
        public App(
            IUniqueIdGenerator generator,
            IRunnerBuilder builder,
            IStageComponentProvider provider,
            PluginModelComponentProvider modelProvider,
            IPluginHost<IRunnerComponent> pluginHost,
            IAppOptionsService options,
            ILoggingService loggingService,
            ILogger<Runner.Engine.Runner> runnerLogger,
            ILogger<App> logger)
        {
            // Validate and set members
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _modelProvider = modelProvider ?? throw new ArgumentNullException(nameof(modelProvider));
            _pluginHost = pluginHost ?? throw new ArgumentNullException(nameof(pluginHost));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _runnerLogger = runnerLogger ?? throw new ArgumentNullException(nameof(runnerLogger));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IApp Interface Implementation

        /// <summary>
        /// Runs the app.
        /// </summary>
        /// <param name="token">A token used to cancel the operation.</param>
        /// <returns>A task used to await the operation with a return code for the CLI.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Converting to return code for command line.")]
        public async Task<int> RunAsync(CancellationToken token)
        {
            // Merge configuration
            _options.MergeConfig();

            // Set defaults if no value set in config or command line
            _options.ApplyDefaults();

            // Validate options
            if (!ValidateOptions(_options))
            {
                return await Task.FromResult(ReturnCodeConstants.ArgsError).ConfigureAwait(false);
            }

            // Set logging level
            SetLogLevel(_options);

            // Set options
            SetOptions(_options);

            // Print options
            _options.PrintOptions();

            // The model and stage runners may contain types that are not loaded into the default assembly load context
            // because they are types used by a plugin that populated the model.  Try and resolve any unresolved assemblies
            // with the plugin host instead when the runner is executed.  This particularly affects XmlSerializer where
            // it generates a custom serialization assembly that is always loaded into the default assembly load context.
            AssemblyLoadContext.Default.Resolving += ResolveAssemblyHandler;

            try
            {
                // Build runner
                var runner = BuildRunner();
                if (runner != null)
                {
                    try
                    {
                        // Run app
                        await runner.RunAsync(token).ConfigureAwait(false);

                        // Save model (if required)
                        SaveModel(runner);

                        // Print out execution stats
                        PrintExecutionStats(runner.RunState);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation(InformationMessages.RunnerCancelled);
                        return await Task.FromResult(ReturnCodeConstants.Cancelled).ConfigureAwait(false);
                    }
                    catch (RunnerException e)
                    {
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogError(e, ErrorMessages.FullExceptionDetails);
                        }

                        return await Task.FromResult(ReturnCodeConstants.AppError).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogError(e, ErrorMessages.FullExceptionDetails);
                        }

                        return await Task.FromResult(ReturnCodeConstants.AppError).ConfigureAwait(false);
                    }

                    return await Task.FromResult(ReturnCodeConstants.Success).ConfigureAwait(false);
                }
                else
                {
                    // Nothing to run
                    return await Task.FromResult(ReturnCodeConstants.Success).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(ErrorMessages.FailedBuildingRunner, e.Message);

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogError(e, ErrorMessages.FullExceptionDetails);
                }

                return await Task.FromResult(ReturnCodeConstants.AppError).ConfigureAwait(false);
            }
            finally
            {
                // Unhook from event
                AssemblyLoadContext.Default.Resolving -= ResolveAssemblyHandler;
            }
        }

        #endregion

        /// <summary>
        /// Sets the log level.
        /// </summary>
        /// <param name="options">The command line options.</param>
        private void SetLogLevel(IAppOptionsService options)
        {
            // Change verbose level, only if verbose logging switched on
            if (options.Options.Verbose)
            {
                if (options.Options.VerboseLevel == '+')
                {
                    // Set maximum level
                    _loggingService.SetLoggingLevel(LogLevel.Trace);
                }
                else
                {
                    // Set minimum level
                    _loggingService.SetLoggingLevel(LogLevel.Debug);
                }
            }
        }

        /// <summary>
        /// Validates the options.
        /// </summary>
        /// <param name="options">The command line options.</param>
        /// <returns>True if the validation succeeded, otherwise False.</returns>
        private bool ValidateOptions(IAppOptionsService options)
        {
            _logger.LogDebug(TraceMessages.ValidatingOptions);

            var errors = options.Validate();
            if (errors != null && errors.Any())
            {
                _logger.LogError(ErrorMessages.ArgumentsFailedValidation, errors.Count());

                foreach (var error in errors)
                {
                    _logger.LogError(ErrorMessages.ArgumentValidationError, error);
                }

                return false;
            }
            else
            {
                _logger.LogDebug(TraceMessages.OptionsValidationSuccessful);

                return true;
            }
        }

        /// <summary>
        /// Sets the options.
        /// </summary>
        /// <param name="options">The command line options.</param>
        private void SetOptions(IAppOptionsService options)
        {
            // Set working path (and reset current directory to working path)
            options.Options.WorkingPath = new DirectoryInfo(BuildWorkingPath(options.Options.WorkingPath.FullName));
            Environment.CurrentDirectory = options.Options.WorkingPath.FullName;

            _logger.LogDebug(TraceMessages.SetWorkingPath, options.Options.WorkingPath);

            // Set state path
            if (options.Options.StatePath == null)
            {
                options.Options.StatePath = new DirectoryInfo(Path.Join(options.Options.WorkingPath.FullName, "state")).FullName;
            }
            else
            {
                options.Options.StatePath = new DirectoryInfo(BuildWorkingPath(options.Options.StatePath)).FullName;
            }

            _logger.LogDebug(TraceMessages.SetStatePath, options.Options.StatePath);

            if (options.Verb == CommandVerb.Assess)
            {
                // Set output model path if specified
                if (options.Options.OutputModel != null)
                {
                    options.Options.OutputModel = new FileInfo(Path.Combine(options.Options.WorkingPath.FullName, options.Options.OutputModel)).FullName;

                    _logger.LogDebug(TraceMessages.SetOutputModelPath, options.Options.OutputModel);
                }
            }

            if (options.Verb == CommandVerb.Migrate || options.Verb == CommandVerb.Convert)
            {
                // Set the conversion path
                if (options.Options.ConversionPath == null)
                {
                    options.Options.ConversionPath = new DirectoryInfo(Path.Join(options.Options.WorkingPath.FullName, "conversion")).FullName;
                }
                else
                {
                    options.Options.ConversionPath = new DirectoryInfo(BuildWorkingPath(options.Options.ConversionPath)).FullName;
                }

                _logger.LogDebug(TraceMessages.SetConversionPath, options.Options.ConversionPath);
            }

            // Generate unique ID to be used for Azure deployments
            if (string.IsNullOrWhiteSpace(options.Options.UniqueDeploymentId))
            {
                options.Options.UniqueDeploymentId = _generator.Generate(truncate: AppOptionsService.UniqueDeploymentIdLengthLimit);
            }
        }

        /// <summary>
        /// Prints the execution stats.
        /// </summary>
        /// <param name="runState">The execution run state.</param>
        private void PrintExecutionStats(IRunState runState)
        {
            if (runState != null && runState.ExecutionState != null)
            {
                if (!_options.Options.Verbose)
                {
                    // Find and print any stage runners that didn't complete
                    if (runState.ExecutionState.Values.Any(s => s.ExecutionState.Any(r => r.State != State.Completed && r.State != State.Skipped)))
                    {
                        foreach (var stageState in runState.ExecutionState.Values)
                        {
                            foreach (var runnerState in stageState.ExecutionState)
                            {
                                if (runnerState.State != State.Completed && runnerState.State != State.Skipped)
                                {
                                    var runnerName = runnerState.StageRunner.Name ?? runnerState.StageRunner.GetType().FullName;

                                    switch (runnerState.State)
                                    {
                                        case State.Failed:
                                            _logger.LogError(ErrorMessages.StageRunnerFailed, runnerName, runnerState.Error.Message);
                                            break;

                                        default:
                                            _logger.LogWarning(WarningMessages.StageRunnerDidNotCompleteSuccessfully, runnerName, runnerState.State);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Print detailed execution stats
                    var firstStageState = runState.ExecutionState.Values.Where(s => s.Stage == Stages.Discover).FirstOrDefault();
                    if (firstStageState != null)
                    {
                        var startTime = firstStageState.Started;

                        foreach (var stageState in runState.ExecutionState.Values)
                        {
                            _logger.LogDebug(TraceMessages.StageExecutionStats, stageState.Stage, stageState.State, (stageState.Started - startTime).TotalMilliseconds, (stageState.Completed - startTime).TotalMilliseconds);

                            foreach (var runnerState in stageState.ExecutionState)
                            {
                                var runnerName = runnerState.StageRunner.Name ?? runnerState.StageRunner.GetType().FullName;

                                switch (runnerState.State)
                                {
                                    case State.Failed:
                                        _logger.LogDebug(TraceMessages.StageRunnerExecutionStatsWithError, runnerName, stageState.Stage, runnerState.State, (runnerState.Started - startTime).TotalMilliseconds, (runnerState.Completed - startTime).TotalMilliseconds, runnerState.Error.Message);
                                        break;

                                    default:
                                        _logger.LogDebug(TraceMessages.StageRunnerExecutionStats, runnerName, stageState.Stage, runnerState.State, (runnerState.Started - startTime).TotalMilliseconds, (runnerState.Completed - startTime).TotalMilliseconds);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Builds the runner.
        /// </summary>
        /// <returns>The runner to execute.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Converting to return code for command line.")]
        private IRunner BuildRunner()
        {
            // Build runner
            IRunnerBuilder runnerBuilder = null;
            switch (_options.Verb)
            {
                case CommandVerb.Assess:

                    _logger.LogInformation(InformationMessages.RunningAssessment);

                    runnerBuilder = _builder
                        .DisableStage(Stages.Convert)
                        .DisableStage(Stages.Verify);

                    break;

                case CommandVerb.Migrate:

                    _logger.LogInformation(InformationMessages.RunningMigration);

                    runnerBuilder = _builder;

                    break;

                case CommandVerb.Convert:

                    _logger.LogInformation(InformationMessages.RunningConversion);

                    runnerBuilder = _builder
                        .DisableStage(Stages.Discover)
                        .DisableStage(Stages.Parse)
                        .DisableStage(Stages.Analyze)
                        .DisableStage(Stages.Report);

                    break;

                case CommandVerb.Verify:

                    _logger.LogInformation(InformationMessages.RunningVerification);

                    runnerBuilder = _builder
                        .DisableStage(Stages.Discover)
                        .DisableStage(Stages.Parse)
                        .DisableStage(Stages.Analyze)
                        .DisableStage(Stages.Report)
                        .DisableStage(Stages.Convert);

                    break;
            }

            if (runnerBuilder != null)
            {
                // Enable flags and stages, if set
                runnerBuilder = _options.Options.NoAbort ? runnerBuilder : runnerBuilder.EnableFailFast();
                if (_options.Options.AbortStage != null && _options.Options.AbortStage.Any())
                {
                    foreach (var stage in _options.Options.AbortStage)
                    {
                        runnerBuilder = runnerBuilder.EnableFailStage(stage);
                    }
                }

                // Load model (if required), otherwise get model from provider
                Func<IApplicationModel> modelFunc = () =>
                {
                    IApplicationModel model = null;
                    if (_options.Verb == CommandVerb.Convert)
                    {
                        model = LoadModel();
                        if (model == null)
                        {
                            throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.FailedToLoadModel, _options.Options.Model.FullName));
                        }
                    }
                    else
                    {
                        // Find model provider
                        var provider = _modelProvider.FindModelProvider();
                        if (provider == null)
                        {
                            throw new ApplicationException(ErrorMessages.FailedToFindModelProvider);
                        }
                        else
                        {
                            model = provider.GetModel();
                        }
                    }

                    return model;
                };

                // Build runner
                var runner = runnerBuilder
                    .FindStageRunners(_provider)
                    .SetLogger(_runnerLogger)
                    .SetArgs(_options.ParseArgs())
                    .SetModel(modelFunc)
                    .Build();

                // Attach event handlers if required
                if (_options.Options.SaveState)
                {
                    _logger.LogDebug(TraceMessages.SettingStageRunnerStateEventHandlers);

                    runner.BeforeStageRunner += (s, e) =>
                    {
                        var state = ((IRunner)s).RunState;
                        SaveState(BuildStatePath(_options.Options.StatePath, false, false, state), state);
                    };

                    runner.AfterStageRunner += (s, e) =>
                    {
                        var state = ((IRunner)s).RunState;
                        SaveState(BuildStatePath(_options.Options.StatePath, true, false, state), state);
                    };
                }

                if (_options.Options.SaveStageState)
                {
                    _logger.LogDebug(TraceMessages.SettingStageStateEventHandlers);

                    runner.BeforeStage += (s, e) =>
                    {
                        var state = ((IRunner)s).RunState;
                        SaveState(BuildStatePath(_options.Options.StatePath, false, true, state), state);
                    };

                    runner.AfterStage += (s, e) =>
                    {
                        var state = ((IRunner)s).RunState;
                        SaveState(BuildStatePath(_options.Options.StatePath, true, true, state), state);
                    };
                }

                return runner;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Builds a working path using the working root path supplied and adding a subdirectory for the process.
        /// </summary>
        /// <param name="workingPath">The base path.</param>
        /// <returns>A fully qualified path.</returns>
        private static string BuildWorkingPath(string workingPath)
        {
            var process = Process.GetCurrentProcess();
            var processInfo = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", process.ProcessName, process.Id);
            var path = Path.Join(workingPath, processInfo);

            // Ensure path exists
            Directory.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// Builds a state path for execution state.
        /// </summary>
        /// <param name="statePath">The state path.</param>
        /// <param name="after">True if event raised after execution, other False for before execution.</param>
        /// <param name="stage">True to build path for a stage, otherwise False for a stage runner.</param>
        /// <param name="runState">The execution state.</param>
        /// <returns>A fully qualified path</returns>
        private static string BuildStatePath(string statePath, bool after, bool stage, IRunState runState)
        {
            var stageState = runState.ExecutionState.Values.Where(s => s.IsCurrent).First();
            var stageName = stageState.Stage.ToString("G");

            var dateTime = DateTimeOffset.Now.ToString("yyyy-MM-dd.HH-mm-ss.fff", CultureInfo.InvariantCulture);

            if (stage)
            {
                var stageFile = string.Format(CultureInfo.InvariantCulture, "{0}-{1}.json", stageName, dateTime);

                // Stage
                if (!after)
                {
                    // Before execution
                    statePath = Path.Join(statePath, PathResources.BeforeStage, stageFile);
                }
                else
                {
                    // After execution
                    statePath = Path.Join(statePath, PathResources.AfterStage, stageFile);
                }
            }
            else
            {
                var stageRunnerState = stageState.ExecutionState.Where(s => s.IsCurrent).First();
                var stageRunnerName = stageRunnerState.StageRunner.Name ?? stageRunnerState.StageRunner.GetType().Name;
                var stageRunnerFile = string.Format(CultureInfo.InvariantCulture, "{0}-{1}.json", stageRunnerName, dateTime);

                // Stage runner
                if (!after)
                {
                    // Before execution
                    statePath = Path.Join(statePath, PathResources.BeforeStageRunner, stageName, stageRunnerFile);
                }
                else
                {
                    // After execution
                    statePath = Path.Join(statePath, PathResources.AfterStageRunner, stageName, stageRunnerFile);
                }
            }

            // Ensure path exists
            var dirs = new FileInfo(statePath).Directory.FullName;
            Directory.CreateDirectory(dirs);

            return statePath;
        }

        /// <summary>
        /// Saves the state to disk.
        /// </summary>
        /// <param name="filePath">The file path for the execution state file.</param>
        /// <param name="state">The execution state.</param>
        private void SaveState(string filePath, IRunState state)
        {
            _logger.LogDebug(TraceMessages.SavingState, filePath);

            // Serialize to JSON.
            // Note: Using Newtonsoft JSON serializer because the new System.Text.Json is not fit for purpose with
            // reference loop handling, which won't be supported until .NET 5.
            // https://github.com/dotnet/runtime/issues/30820
            // https://github.com/dotnet/runtime/issues/29900
            var json = JsonConvert.SerializeObject(state, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize, Formatting = Formatting.Indented });
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Saves the model to disk.
        /// </summary>
        /// <param name="runner">The runner.</param>
        private void SaveModel(IRunner runner)
        {
            // Should we save model?
            if (runner.RunState.Model != null && _options.Options.OutputModel != null)
            {
                _logger.LogDebug(TraceMessages.SavingModel, _options.Options.OutputModel);

                // Ensure path exists
                var fileInfo = new FileInfo(_options.Options.OutputModel);
                var dirs = fileInfo.Directory.FullName;
                Directory.CreateDirectory(dirs);

                // Serialize model using Newtonsoft for now (need to preserve type information).  May arrive in .NET 5.
                // https://github.com/dotnet/runtime/issues/30969
                var json = JsonConvert.SerializeObject(runner.RunState.Model, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Serialize, Formatting = Formatting.Indented });
                File.WriteAllText(fileInfo.FullName, json);
            }
        }

        /// <summary>
        /// Loads the model from disk.
        /// </summary>
        private IApplicationModel LoadModel()
        {
            // Should we save model?
            if (_options.Options.Model != null)
            {
                _logger.LogDebug(TraceMessages.LoadingModel, _options.Options.Model.FullName);

                // Read model file content
                var json = File.ReadAllText(_options.Options.Model.FullName);

                try
                {
                    // Deserialize model using Newtonsoft for now (has type information).  This may change in .NET 5.
                    // https://github.com/dotnet/runtime/issues/30969
                    var model = JsonConvert.DeserializeObject(json, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

                    // Is it of the right type?
                    if (!(model is IApplicationModel appModel))
                    {
                        throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.ModelFileDoesNotContainAnApplicationModel, _options.Options.Model.FullName));
                    }
                    else
                    {
                        return appModel;
                    }
                }
                catch (Exception e) when (e is JsonSerializationException || e is SerializationException)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogError(e, ErrorMessages.FailedDeserializingModel, _options.Options.Model.FullName, e.Message);
                    }
                    else
                    {
                        _logger.LogError(ErrorMessages.FailedDeserializingModel, _options.Options.Model.FullName, e.Message);
                    }
                }

                return null;

            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Event handler to resolve assemblies against the plugin host.
        /// </summary>
        /// <param name="context">The calling assembly load context.</param>
        /// <param name="assemblyName">The name of the assembly that is unresolved.</param>
        /// <returns>An assembly reference, or null if not able to resolve.</returns>
        private Assembly ResolveAssemblyHandler(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            _logger.LogTrace(TraceMessages.ResolvingAssemblyLoadingModel, assemblyName.FullName);

            // Avoid recursive resolving with assembly load context resolution fallback logic to default context
            AssemblyLoadContext.Default.Resolving -= ResolveAssemblyHandler;

            try
            {
                // Resolve against custom assembly load contexts used for plugins
                return _pluginHost.ResolveAssembly(assemblyName);
            }
            finally
            {
                // Re-attach event handler
                AssemblyLoadContext.Default.Resolving += ResolveAssemblyHandler;
            }
        }
    }
}
