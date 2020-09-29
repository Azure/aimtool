// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AzureIntegrationMigration.Runner;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Model;
using Microsoft.AzureIntegrationMigration.Tool.Options;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Microsoft.AzureIntegrationMigration.Tool.Plugins
{
    /// <summary>
    /// Defines a class that searches directories for stage runners.
    /// </summary>
    public class PluginStageComponentProvider : IStageComponentProvider, IDisposable
    {
        /// <summary>
        /// Determines if the object has been disposed or not.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Defines the app command line options.
        /// </summary>
        private readonly AppOptions _options;

        /// <summary>
        /// Defines a logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Defines a plugin finder.
        /// </summary>
        private readonly IPluginFinder<IStageRunner> _pluginFinder;

        /// <summary>
        /// Defines a plugin host.
        /// </summary>
        private readonly IPluginHost<IRunnerComponent> _pluginHost;

        /// <summary>
        /// Defines the types that exist in shared assemblies that should be loaded from default context
        /// rather than loaded into custom context (where the same types are not equivalent across contexts).
        /// </summary>
        private readonly Type[] _sharedTypes = new Type[]
        {
            typeof(ILogger),                        // Dotnet Core logging
            typeof(IRunnerComponent),               // Runner types (for plugins)
            typeof(IApplicationModel),              // Model used by stage runners
            typeof(Uri),                            // Type in System.Private.CoreLib (should be shared)
            typeof(JsonConvert),                    // Newtonsoft JSON serializer
            typeof(System.Data.DataSet),            // Used by XML serialization and Newtonsoft for model saving/loading
            typeof(System.Xml.XmlDocument)          // Used by XML serialization and Newtonsoft for model saving/loading
        };

        /// <summary>
        /// Constructs an instance of the <see cref="PluginStageComponentProvider"/> class with plugin objects,
        /// command line options and a logger.
        /// </summary>
        /// <param name="pluginFinder">The plugin finder.</param>
        /// <param name="pluginHost">The plugin host.</param>
        /// <param name="options">The command line options.</param>
        /// <param name="logger"></param>
        public PluginStageComponentProvider(
            IPluginFinder<IStageRunner> pluginFinder,
            IPluginHost<IRunnerComponent> pluginHost,
            IOptions<AppOptions> options,
            ILogger<PluginStageComponentProvider> logger)
        {
            _pluginFinder = pluginFinder ?? throw new ArgumentNullException(nameof(pluginFinder));
            _pluginHost = pluginHost ?? throw new ArgumentNullException(nameof(pluginHost));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        /// <param name="disposing">True if the object is being disposed of deterministically, otherwise False.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_pluginHost is IDisposable h)
                    {
                        h.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        #region IStageComponentProvider Interface Implementation

        /// <summary>
        /// Finds components by recursing one or more directories.
        /// </summary>
        /// <param name="config">The runner configuration.</param>
        /// <returns>A list of stage runners.</returns>
        public IEnumerable<IStageRunner> FindComponents(IRunnerConfiguration config)
        {
            _logger.LogInformation(InformationMessages.FindingStageRunners);

            if (_options.FindPath != null && _options.FindPath.Any())
            {
                var stageRunnerAssemblies = new List<FileInfo>();

                foreach (var path in _options.FindPath)
                {
                    // Find plugin assembly locations
                    var assemblies = _pluginFinder.FindPluginAssemblies(path.FullName, _sharedTypes);

                    // Remove duplicates (based on filename, without path), if any
                    var distinctAssemblies = assemblies.Select(a => new FileInfo(a)).GroupBy(f => f.Name).Select(f => f.First());

                    if (distinctAssemblies.Any())
                    {
                        _logger.LogInformation(InformationMessages.AssemblyCountInPath, distinctAssemblies.Count(), path.FullName);

                        foreach (var assembly in distinctAssemblies)
                        {
                            stageRunnerAssemblies.Add(assembly);
                        }
                    }
                    else
                    {
                        _logger.LogDebug(TraceMessages.StageRunnerAssembliesNotFoundInPath, path.FullName);
                    }
                }

                if (stageRunnerAssemblies.Count > 0)
                {
                    // Load stage runners into separate plugin host
                    var distinctStageRunnerAssemblies = stageRunnerAssemblies.GroupBy(f => f.Name).Select(f => f.First().FullName);
                    _pluginHost.LoadPlugins(distinctStageRunnerAssemblies, _sharedTypes);

                    return _pluginHost.GetPlugins().Where(p => p is IStageRunner).Select(p => (IStageRunner)p);
                }
                else
                {
                    _logger.LogWarning(WarningMessages.StageRunnerAssembliesNotFound);
                }
            }

            return Enumerable.Empty<IStageRunner>();
        }

        #endregion

        #region IDisposable Interface Implementation

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
