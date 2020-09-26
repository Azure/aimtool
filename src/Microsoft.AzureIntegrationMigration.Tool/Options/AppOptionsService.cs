using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Tool.Logging;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AzureIntegrationMigration.Tool.Options
{
    /// <summary>
    /// Defines the common app command line options.
    /// </summary>
    public class AppOptionsService : IAppOptionsService
    {
        /// <summary>
        /// Defines the prefix for CLI specific args.
        /// </summary>
        private const string ArgCliPrefix = "microsoft.cli.";

        /// <summary>
        /// Defines the prefix for core args used by stage runners.
        /// </summary>
        private const string ArgCorePrefix = "microsoft.core.";

        /// <summary>
        /// Defines the prefix for core args used by stage runners.
        /// </summary>
        private const string UniqueDeploymentIdRegex = "^[-\\w\\._\\(\\)]+$";

        /// <summary>
        /// Defines the limit on the length of a unique deployment ID.
        /// </summary>
        public const int UniqueDeploymentIdLengthLimit = 5;

        /// <summary>
        /// Defines the command line configuration from appsettings.json file.
        /// </summary>
        private readonly AppConfig _config;

        /// <summary>
        /// Defines the options from the command line.
        /// </summary>
        private readonly IOptions<AppOptions> _options;

        /// <summary>
        /// Defines an Azure region provider.
        /// </summary>
        private readonly IAzureRegionProvider _regionProvider;

        /// <summary>
        /// Defines the logger to use.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Defines a flag that specifies whether the primary region was specified on the command line.
        /// </summary>
        private bool _userProvidedPrimaryRegion;

        /// <summary>
        /// Defines a flag that specifies whether the primary region was specified on the command line.
        /// </summary>
        private bool _userProvidedSecondaryRegion;

        /// <summary>
        /// Constructs an instance of the <see cref="AppOptionsService"/> class with its dependencies.
        /// </summary>
        /// <param name="config">The command line configuration.</param>
        /// <param name="options">The application options from the command line.</param>
        /// <param name="regionProvider">The region provider.</param>
        /// <param name="logger">The logger for the app.</param>
        public AppOptionsService(IConfiguration config, IOptions<AppOptions> options, IAzureRegionProvider regionProvider, ILogger<AppOptions> logger)
        {
            // Validate and set members
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _regionProvider = regionProvider ?? throw new ArgumentNullException(nameof(regionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Get app configuration
            var appConfig = config ?? throw new ArgumentNullException(nameof(config));
            _config = appConfig.GetSection(nameof(AppConfig)).Get<AppConfig>();
        }

        /// <summary>
        /// Gets or sets the verb used on the command line.
        /// </summary>
        public CommandVerb Verb { get; set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public AppOptions Options => _options.Value;

        /// <summary>
        /// Merges all of the configuration from the appsettings.json file to the parsed command line
        /// options, where options on the command line take precedence.
        /// </summary>
        public void MergeConfig()
        {
            if (_config != null)
            {
                // Working path
                if (_options.Value.WorkingPath == null && !string.IsNullOrWhiteSpace(_config.WorkingPath))
                {
                    _options.Value.WorkingPath = new DirectoryInfo(_config.WorkingPath);
                }

                // State path
                if (_options.Value.StatePath == null && !string.IsNullOrWhiteSpace(_config.StatePath))
                {
                    _options.Value.StatePath = new DirectoryInfo(_config.StatePath).FullName;
                }

                // Find paths
                if (_config.FindPaths != null && _config.FindPaths.Any())
                {
                    // Merge existing paths if they exist
                    var findPaths = new List<DirectoryInfo>();
                    if (_options.Value.FindPath != null && _options.Value.FindPath.Any())
                    {
                        findPaths.AddRange(_options.Value.FindPath);
                    }

                    // Add new paths
                    findPaths.AddRange(_config.FindPaths.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => new DirectoryInfo(p)));

                    // Replace with new appended collection
                    _options.Value.FindPath = findPaths;
                }

                // Find pattern
                if (string.IsNullOrWhiteSpace(_options.Value.FindPattern) && !string.IsNullOrWhiteSpace(_config.FindPattern))
                {
                    _options.Value.FindPattern = _config.FindPattern;
                }

                // Template config path
                if (_options.Value.TemplateConfigPath == null && !string.IsNullOrWhiteSpace(_config.TemplateConfigPath))
                {
                    _options.Value.TemplateConfigPath = new DirectoryInfo(_config.TemplateConfigPath);
                }

                // Template path
                if (_config.TemplatePaths != null && _config.TemplatePaths.Any())
                {
                    // Merge existing paths if they exist
                    var templatePaths = new List<DirectoryInfo>();
                    if (_options.Value.TemplatePath != null && _options.Value.TemplatePath.Any())
                    {
                        templatePaths.AddRange(_options.Value.TemplatePath);
                    }

                    // Add new paths
                    templatePaths.AddRange(_config.TemplatePaths.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => new DirectoryInfo(p)));

                    // Replace with new appended collection
                    _options.Value.TemplatePath = templatePaths;
                }

                // Primary region
                if (!string.IsNullOrWhiteSpace(_options.Value.PrimaryRegion))
                {
                    _userProvidedPrimaryRegion = true;
                }

                if (string.IsNullOrWhiteSpace(_options.Value.PrimaryRegion) && !string.IsNullOrWhiteSpace(_config.PrimaryRegion))
                {
                    _options.Value.PrimaryRegion = _config.PrimaryRegion;
                }

                // Secondary region
                if (!string.IsNullOrWhiteSpace(_options.Value.SecondaryRegion))
                {
                    _userProvidedSecondaryRegion = true;
                }

                if (string.IsNullOrWhiteSpace(_options.Value.SecondaryRegion) && !string.IsNullOrWhiteSpace(_config.SecondaryRegion))
                {
                    _options.Value.SecondaryRegion = _config.SecondaryRegion;
                }

                // Check paired regions
                if (_userProvidedPrimaryRegion && !_userProvidedSecondaryRegion)
                {
                    var pairedRegion = _regionProvider.GetPairedRegion(_options.Value.PrimaryRegion);
                    if (pairedRegion != null && _options.Value.SecondaryRegion != pairedRegion)
                    {
                        // Secondary region not specified on command line but isn't the paired region for the primary region, replace it
                        _logger.LogInformation(InformationMessages.SecondaryRegionChangedToMatchPairedRegionOfPrimary, _options.Value.SecondaryRegion, _options.Value.PrimaryRegion, pairedRegion);

                        _options.Value.SecondaryRegion = pairedRegion;
                    }
                }

                if (!_userProvidedPrimaryRegion && _userProvidedSecondaryRegion)
                {
                    var pairedRegion = _regionProvider.GetPairedRegion(_options.Value.SecondaryRegion);
                    if (pairedRegion != null && _options.Value.PrimaryRegion != pairedRegion)
                    {
                        // Primary region not specified on command line but isn't the paired region for the secondary region, replace it
                        _logger.LogInformation(InformationMessages.PrimaryRegionChangedToMatchPairedRegionOfSecondary, _options.Value.PrimaryRegion, _options.Value.SecondaryRegion, pairedRegion);

                        _options.Value.PrimaryRegion = pairedRegion;
                    }
                }

                if (_userProvidedPrimaryRegion && _userProvidedSecondaryRegion)
                {
                    var pairedRegion = _regionProvider.GetPairedRegion(_options.Value.PrimaryRegion);
                    if (_options.Value.SecondaryRegion != pairedRegion)
                    {
                        // Secondary region is not the paired region, just output a warning in case this was not intended
                        _logger.LogWarning(WarningMessages.SecondaryRegionIsNotPairedWithPrimary, _options.Value.SecondaryRegion, _options.Value.PrimaryRegion, pairedRegion);
                    }
                }
            }
        }

        /// <summary>
        /// Prints the options out to the logger.
        /// </summary>
        public void PrintOptions()
        {
            // Print verb
            _logger.LogInformation(InformationMessages.Verb, Verb);

            // Common app options
            _logger.LogInformation(InformationMessages.Verbose, _options.Value.Verbose);

            if (_options.Value.Verbose)
            {
                _logger.LogInformation(InformationMessages.VerboseLevel, _options.Value.VerboseLevel);
            }

            _logger.LogInformation(InformationMessages.Abort, !_options.Value.NoAbort);

            if (_options.Value.AbortStage != null)
            {
                foreach (var stage in _options.Value.AbortStage)
                {
                    _logger.LogInformation(InformationMessages.AbortStage, stage);
                }
            }

            if (_options.Value.Arg != null)
            {
                foreach (var arg in _options.Value.Arg)
                {
                    _logger.LogInformation(InformationMessages.Arg, arg.Key, arg.Value);
                }
            }

            _logger.LogInformation(InformationMessages.ArgDelimiter, _options.Value.ArgDelimiter);

            if (_options.Value.FindPath != null)
            {
                foreach (var path in _options.Value.FindPath)
                {
                    _logger.LogInformation(InformationMessages.FindPath, path.FullName);
                }
            }

            _logger.LogInformation(InformationMessages.FindPattern, _options.Value.FindPattern);
            _logger.LogInformation(InformationMessages.WorkingPath, _options.Value.WorkingPath?.FullName);
            _logger.LogInformation(InformationMessages.StatePath, _options.Value.StatePath);
            _logger.LogInformation(InformationMessages.SaveState, _options.Value.SaveState);
            _logger.LogInformation(InformationMessages.SaveStageState, _options.Value.SaveStageState);
            _logger.LogInformation(InformationMessages.Target, _options.Value.Target);
            _logger.LogInformation(InformationMessages.DeploymentEnv, _options.Value.DeploymentEnv);
            _logger.LogInformation(InformationMessages.UniqueDeploymentId, _options.Value.UniqueDeploymentId);

            if (_options.Value.ConversionPath != null)
            {
                _logger.LogInformation(InformationMessages.ConversionPath, _options.Value.ConversionPath);
            }

            if (_options.Value.SubscriptionId != null)
            {
                _logger.LogInformation(InformationMessages.AzureSubscriptionId, _options.Value.SubscriptionId);
            }

            if (_options.Value.PrimaryRegion != null)
            {
                _logger.LogInformation(InformationMessages.AzurePrimaryRegion, _options.Value.PrimaryRegion);
            }

            if (_options.Value.SecondaryRegion != null)
            {
                _logger.LogInformation(InformationMessages.AzureSecondaryRegion, _options.Value.SecondaryRegion);
            }

            if (_options.Value.OutputModel != null)
            {
                _logger.LogInformation(InformationMessages.OutputModelFile, _options.Value.OutputModel);
            }

            if (_options.Value.TemplateConfigPath != null)
            {
                _logger.LogInformation(InformationMessages.TemplateConfigPath, _options.Value.TemplateConfigPath.FullName);
            }

            if (_options.Value.Model != null)
            {
                _logger.LogInformation(InformationMessages.ModelFile, _options.Value.Model.FullName);
            }

            if (_options.Value.TemplatePath != null)
            {
                foreach (var path in _options.Value.TemplatePath)
                {
                    _logger.LogInformation(InformationMessages.TemplatePath, path.FullName);
                }
            }
        }

        /// <summary>
        /// Applies default values to options that haven't been set in the config file or overridden
        /// on the command line.
        /// </summary>
        public void ApplyDefaults()
        {
            // Working path
            if (_options.Value.WorkingPath == null)
            {
                _options.Value.WorkingPath = new DirectoryInfo(Environment.CurrentDirectory);
            }

            // Verbose level
            if (_options.Value.VerboseLevel != '-' && _options.Value.VerboseLevel != '+')
            {
                _options.Value.VerboseLevel = '-';
            }

            // Argument delimiter
            if (string.IsNullOrEmpty(_options.Value.ArgDelimiter))
            {
                _options.Value.ArgDelimiter = "|";
            }

            // Abort stages
            if (_options.Value.AbortStage == null || !_options.Value.AbortStage.Any())
            {
                _options.Value.AbortStage = new Stages[] { Stages.None };
            }

            // Find pattern
            if (string.IsNullOrWhiteSpace(_options.Value.FindPattern))
            {
                _options.Value.FindPattern = "*.dll";
            }

            // Template config path
            if (_options.Value.TemplateConfigPath == null)
            {
                _options.Value.TemplateConfigPath = new DirectoryInfo(Environment.CurrentDirectory);
            }

            // Deployment environment
            if (string.IsNullOrEmpty(_options.Value.DeploymentEnv))
            {
                _options.Value.DeploymentEnv = "dev";
            }
        }

        /// <summary>
        /// Converts command line arguments to arbitrary arguments for the runner.
        /// </summary>
        /// <returns>Parsed and converted args.</returns>
        public IDictionary<string, object> ParseArgs()
        {
            var parsedArgs = new Dictionary<string, object>
            {
                // Add named args from command line
                { string.Concat(ArgCliPrefix, "verbose"), _options.Value.Verbose },
                { string.Concat(ArgCliPrefix, "verboselevel"), _options.Value.VerboseLevel },
                { string.Concat(ArgCliPrefix, "abort"), !_options.Value.NoAbort },
                { string.Concat(ArgCliPrefix, "abortstages"), _options.Value.AbortStage.Select(s => s.ToString("G")).ToArray() },
                { string.Concat(ArgCliPrefix, "findpaths"), _options.Value.FindPath?.Select(p => p.FullName).ToArray() },
                { string.Concat(ArgCliPrefix, "findpatterns"), _options.Value.FindPattern },
                { string.Concat(ArgCliPrefix, "argdelimiter"), _options.Value.ArgDelimiter },
                { string.Concat(ArgCliPrefix, "workingpath"), _options.Value.WorkingPath?.FullName },
                { string.Concat(ArgCliPrefix, "statepath"), _options.Value.StatePath },
                { string.Concat(ArgCliPrefix, "savestate"), _options.Value.SaveState },
                { string.Concat(ArgCliPrefix, "savestagestate"), _options.Value.SaveStageState },
                { string.Concat(ArgCliPrefix, "outputmodel"), _options.Value.OutputModel },
                { string.Concat(ArgCliPrefix, "model"), _options.Value.Model?.FullName },
                { string.Concat(ArgCorePrefix, "target"), _options.Value.Target.ToString("G") },
                { string.Concat(ArgCorePrefix, "subscriptionid"), _options.Value.SubscriptionId },
                { string.Concat(ArgCorePrefix, "primaryregion"), _options.Value.PrimaryRegion },
                { string.Concat(ArgCorePrefix, "secondaryregion"), _options.Value.SecondaryRegion },
                { string.Concat(ArgCorePrefix, "deploymentenv"), _options.Value.DeploymentEnv },
                { string.Concat(ArgCorePrefix, "uniquedeploymentid"), _options.Value.UniqueDeploymentId },
                { string.Concat(ArgCorePrefix, "templateconfigpath"), _options.Value.TemplateConfigPath?.FullName },
                { string.Concat(ArgCorePrefix, "templatepaths"), _options.Value.TemplatePath?.Select(p => p.FullName).ToArray() },
                { string.Concat(ArgCorePrefix, "conversionpath"), _options.Value.ConversionPath }
            };

            // Add arbitrary args from command line
            if (_options.Value.Arg != null && _options.Value.Arg.Any())
            {
                foreach (var kvp in _options.Value.Arg)
                {
                    // Is arg value empty?
                    if (string.IsNullOrWhiteSpace(kvp.Value))
                    {
                        parsedArgs.Add(kvp.Key, null);
                        continue;
                    }

                    // Is arg value a multi-value?
                    var argValues = kvp.Value.Split(_options.Value.ArgDelimiter);
                    if (argValues.Length > 1)
                    {
                        parsedArgs.Add(kvp.Key, argValues);
                        continue;
                    }

                    try
                    {
                        // Is arg value a JSON structure?
                        using var jsonDocument = JsonDocument.Parse(kvp.Value);

                        // If it didn't throw, it is JSON so add the root element object to the args
                        parsedArgs.Add(kvp.Key, jsonDocument.RootElement.Clone());
                    }
                    catch (JsonException)
                    {
                        // Not a valid JSON structure, use value as a string instead
                        parsedArgs.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return parsedArgs;
        }

        /// <summary>
        /// Validates the command line options provided to the app.
        /// </summary>
        /// <returns>A list of errors, if any.</returns>
        public IEnumerable<string> Validate()
        {
            // Common options
            if (_options.Value.FindPath != null && _options.Value.FindPath.Any())
            {
                foreach (var path in _options.Value.FindPath)
                {
                    if (!path.Exists)
                    {
                        yield return string.Format(CultureInfo.CurrentCulture, ErrorMessages.FindPathDoesNotExist, path.FullName);
                    }
                }
            }

            if (_options.Value.WorkingPath != null && !_options.Value.WorkingPath.Exists)
            {
                yield return string.Format(CultureInfo.CurrentCulture, ErrorMessages.WorkingPathDoesNotExist, _options.Value.WorkingPath.FullName);
            }

            if (!string.IsNullOrWhiteSpace(_options.Value.UniqueDeploymentId) && _options.Value.UniqueDeploymentId.Length > UniqueDeploymentIdLengthLimit)
            {
                yield return string.Format(CultureInfo.CurrentCulture, ErrorMessages.UniqueDeploymentIdLengthExceedsLimit, UniqueDeploymentIdLengthLimit);
            }

            if (!string.IsNullOrWhiteSpace(_options.Value.UniqueDeploymentId) && !Regex.IsMatch(_options.Value.UniqueDeploymentId, UniqueDeploymentIdRegex))
            {
                yield return string.Format(CultureInfo.CurrentCulture, ErrorMessages.UniqueDeploymentIdIncorrectFormat, _options.Value.UniqueDeploymentId);
            }

            if (!string.IsNullOrWhiteSpace(_options.Value.PrimaryRegion) && !_regionProvider.RegionExists(_options.Value.PrimaryRegion))
            {
                yield return string.Format(CultureInfo.CurrentCulture, ErrorMessages.RegionDoesNotExist, _options.Value.PrimaryRegion);
            }

            if (!string.IsNullOrWhiteSpace(_options.Value.SecondaryRegion) && !_regionProvider.RegionExists(_options.Value.SecondaryRegion))
            {
                yield return string.Format(CultureInfo.CurrentCulture, ErrorMessages.RegionDoesNotExist, _options.Value.SecondaryRegion);
            }

            if (_options.Value.PrimaryRegion == _options.Value.SecondaryRegion)
            {
                yield return ErrorMessages.PrimaryAndSecondaryRegionsIdentical;
            }

            // Specific command options
            if (Verb == CommandVerb.Assess || Verb == CommandVerb.Migrate)
            {
                if (_options.Value.TemplateConfigPath != null && !_options.Value.TemplateConfigPath.Exists)
                {
                    yield return string.Format(CultureInfo.CurrentCulture, ErrorMessages.TemplateConfigPathDoesNotExist, _options.Value.TemplateConfigPath.FullName);
                }
            }

            if (Verb == CommandVerb.Convert)
            {
                if (_options.Value.Model == null)
                {
                    yield return ErrorMessages.ModelFileMustBeSpecified;
                }
                else if (!_options.Value.Model.Exists)
                {
                    yield return string.Format(CultureInfo.CurrentCulture, ErrorMessages.ModelFileDoesNotExist, _options.Value.Model.FullName);
                }
            }

            if (Verb == CommandVerb.Convert || Verb == CommandVerb.Migrate)
            {
                if (_options.Value.TemplatePath != null && _options.Value.TemplatePath.Any())
                {
                    foreach (var path in _options.Value.TemplatePath)
                    {
                        if (!path.Exists)
                        {
                            yield return string.Format(CultureInfo.CurrentCulture, ErrorMessages.TemplatePathDoesNotExist, path.FullName);
                        }
                    }
                }
            }

            yield break;
        }
    }
}
