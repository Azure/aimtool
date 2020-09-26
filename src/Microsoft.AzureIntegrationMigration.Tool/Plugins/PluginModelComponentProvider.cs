using System;
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
    /// Defines a class that searches directories for model provider.
    /// </summary>
    public class PluginModelComponentProvider : IDisposable
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
        private readonly IPluginFinder<IApplicationModelProvider> _pluginFinder;

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
            typeof(JsonConvert),                    // Newtonsoft JSON serializer
            typeof(System.Data.DataSet),            // Used by XML serialization and Newtonsoft for model saving/loading
            typeof(System.Xml.XmlDocument)          // Used by XML serialization and Newtonsoft for model saving/loading
        };

        /// <summary>
        /// Constructs an instance of the <see cref="PluginModelComponentProvider"/> class with plugin objects,
        /// command line options and a logger.
        /// </summary>
        /// <param name="pluginFinder">The plugin finder for model provider.</param>
        /// <param name="pluginHost">The plugin host.</param>
        /// <param name="options">The command line options.</param>
        /// <param name="logger"></param>
        public PluginModelComponentProvider(
            IPluginFinder<IApplicationModelProvider> pluginFinder,
            IPluginHost<IRunnerComponent> pluginHost,
            IOptions<AppOptions> options,
            ILogger<PluginModelComponentProvider> logger)
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

        /// <summary>
        /// Finds provider by recursing one or more directories.
        /// </summary>
        /// <returns>An application model provider.</returns>
        public IApplicationModelProvider FindModelProvider()
        {
            _logger.LogInformation(InformationMessages.FindingModelProvider);

            if (_options.FindPath != null && _options.FindPath.Any())
            {
                FileInfo providerAssembly = null;

                foreach (var path in _options.FindPath)
                {
                    // Find plugin assembly locations
                    var assemblies = _pluginFinder.FindPluginAssemblies(path.FullName, _sharedTypes);

                    // Remove duplicates (based on filename, without path), if any
                    var distinctAssemblies = assemblies.Select(a => new FileInfo(a)).GroupBy(f => f.Name).Select(f => f.First());

                    if (distinctAssemblies.Any())
                    {
                        _logger.LogInformation(InformationMessages.AssemblyCountInPath, distinctAssemblies.Count(), path.FullName);

                        // There should be only one, so break after the first one found
                        providerAssembly = distinctAssemblies.First();
                        break;
                    }
                    else
                    {
                        _logger.LogDebug(TraceMessages.ModelProviderAssemblyNotFoundInPath, path.FullName);
                    }
                }

                if (providerAssembly != null)
                {
                    // Load provider into separate plugin host
                    _pluginHost.LoadPlugins(new string[1] { providerAssembly.FullName }, _sharedTypes);

                    return _pluginHost.GetPlugins().Where(p => p is IApplicationModelProvider).Select(p => (IApplicationModelProvider)p).First();
                }
                else
                {
                    _logger.LogWarning(WarningMessages.ModelProviderAssemblyNotFound);
                }
            }

            return null;
        }

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
