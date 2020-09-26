using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Tool.Plugins
{
    /// <summary>
    /// Defines a class that hosts plugin assemblies in their own context.
    /// </summary>
    public class PluginHost<TPlugin> : IPluginHost<TPlugin>, IDisposable where TPlugin : class
    {
        /// <summary>
        /// Determines if the object has been disposed or not.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Defines a logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Defines the logger factory used to get loggers for plugins.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Defines an assembly load context for the plugins.
        /// </summary>
        /// <remarks>
        /// There is one context per plugin path.
        /// </remarks>
        private readonly IDictionary<string, PluginAssemblyLoadContext> _pluginContexts = new Dictionary<string, PluginAssemblyLoadContext>();

        /// <summary>
        /// Defines a list of plugins.
        /// </summary>
        private readonly IDictionary<string, IList<TPlugin>> _plugins = new Dictionary<string, IList<TPlugin>>();

        /// <summary>
        /// Constructs a new instance of the <see cref="PluginHost{TPlugin}"/> class with a logger.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public PluginHost(ILogger<PluginHost<TPlugin>> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
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
                    if (_pluginContexts != null)
                    {
                        foreach (var pluginContext in _pluginContexts)
                        {
                            if (pluginContext.Value.IsCollectible)
                            {
                                pluginContext.Value.Unload();
                            }
                        }

                        _pluginContexts.Clear();
                    }
                }

                _disposed = true;
            }
        }

        #region IPluginFinder Interface Implementation

        /// <summary>
        /// Resolves the assembly from an already used plugin assembly load context, if possible.
        /// </summary>
        /// <param name="assemblyName">The assembly to resolve.</param>
        /// <returns>The assembly it has resolved to, or null if none found.</returns>
        public Assembly ResolveAssembly(AssemblyName assemblyName)
        {
            _ = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));

            Assembly assembly = null;
            foreach (var kvp in _pluginContexts)
            {
                var path = Path.Combine(kvp.Key, $"{assemblyName.Name}.dll");
                assembly = kvp.Value.LoadFromAssemblyPath(path);
                if (assembly != null)
                {
                    break;
                }
            }

            return assembly;
        }

        /// <summary>
        /// Get the loaded plugins.
        /// </summary>
        /// <returns>A list of loaded plugins.</returns>
        public IEnumerable<TPlugin> GetPlugins()
        {
            return _plugins.Values.SelectMany(p => p);
        }

        /// <summary>
        /// Finds the assembly locations for all assemblies containing a plugin interface.
        /// </summary>
        /// <param name="assemblyPaths">A list of assembly paths containing plugins.</param>
        /// <param name="sharedTypes">A list of shared types to be used when loading plugins into custom assembly contexts.</param>
        public void LoadPlugins(IEnumerable<string> assemblyPaths, Type[] sharedTypes)
        {
            _ = assemblyPaths ?? throw new ArgumentNullException(nameof(assemblyPaths));

            // Create a plugin context per assembly (in case any weird dependent assembly version requirements)
            foreach (var assemblyPath in assemblyPaths)
            {
                var assemblyFile = new FileInfo(assemblyPath);

                // Don't scan the library containing the plugin type itself into the AssemblyLoadContext,
                // otherwise the IsAssignableFrom won't return a match, because the same assembly in different
                // contexts have types that are not the same as each other when comparing in .NET Core.
                if (assemblyFile.Name != new FileInfo(typeof(TPlugin).Assembly.Location).Name)
                {
                    var pluginPath = assemblyFile.Directory.FullName;

                    // Find context if one already exists for this path
                    if (!_pluginContexts.TryGetValue(pluginPath, out var pluginContext))
                    {
                        _logger.LogDebug(TraceMessages.CreatingNewPluginAssemblyLoadContext, pluginPath);

                        // Create plugin assembly load context to hold the assembly (with shared types).
                        // NOTE: This has to be a non-collectible AssemblyLoadContext because of a bug in .NET Core 3
                        // due to the use of XmlSerializer in a collectible context which creates a dynamic assembly
                        // and puts it in the default context.  This will be fixed in .NET 5 timeframe.
                        // https://github.com/dotnet/runtime/issues/1388
                        pluginContext = new PluginAssemblyLoadContext(assemblyFile.FullName, sharedTypes, false);

                        // Save context
                        _pluginContexts.Add(pluginPath, pluginContext);
                    }

                    // Has it already been loaded?
                    if (!_plugins.ContainsKey(assemblyPath))
                    {
                        _logger.LogDebug(TraceMessages.LoadingPluginAssembly, typeof(TPlugin).Name, assemblyPath);

                        var plugins = new List<TPlugin>();

                        // Load assembly from the path (will put it into the new context)
                        var assembly = ((PluginAssemblyLoadContext)pluginContext).LoadFromAssemblyPath(assemblyPath);

                        foreach (var type in assembly.GetExportedTypes().Where(t => !t.IsAbstract))
                        {
                            var interfaceType = typeof(TPlugin);
                            if (interfaceType.IsAssignableFrom(type))
                            {
                                _logger.LogDebug(TraceMessages.LoadingPluginFromType, type.FullName);

                                // Create instance of plugin
                                plugins.Add(GetPlugin(type));
                            }
                        }

                        _plugins.Add(assemblyPath, plugins);
                    }
                    else
                    {
                        _logger.LogDebug(TraceMessages.PluginAssemblyAlreadyLoaded, assemblyPath);
                    }
                }
            }

            _logger.LogInformation(InformationMessages.LoadedPlugins, _plugins.Values.SelectMany(p => p).Count());
        }

        #endregion

        /// <summary>
        /// Gets an instance of the plugin based on the provided type.
        /// </summary>
        /// <param name="type">The plugin type.</param>
        /// <returns>An instance of the plugin.</returns>
        private TPlugin GetPlugin(Type type)
        {
            TPlugin pluginInstance;

            // Get generic logger
            var pluginLogger = _loggerFactory.CreateLogger(type.Name);

            try
            {
                // Try getting plugin with logger first
                pluginInstance = (TPlugin)Activator.CreateInstance(type, pluginLogger);
            }
            catch (MissingMethodException)
            {
                // Try with parameterless constructor
                pluginInstance = (TPlugin)Activator.CreateInstance(type);
            }


            return pluginInstance;
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
