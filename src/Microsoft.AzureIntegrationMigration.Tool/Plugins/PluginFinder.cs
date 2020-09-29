// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Tool.Options;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AzureIntegrationMigration.Tool.Plugins
{
    /// <summary>
    /// Defines a class that finds plugins on a specified type on disk.
    /// </summary>
    public class PluginFinder<TPlugin> : IPluginFinder<TPlugin> where TPlugin : class
    {
        /// <summary>
        /// Defines the app command line options.
        /// </summary>
        private readonly AppOptions _options;

        /// <summary>
        /// Defines a logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs a new instance of the <see cref="PluginFinder{TPlugin}"/> class with command line options and a logger.
        /// </summary>
        /// <param name="options">The command line options.</param>
        /// <param name="logger">A logger.</param>
        public PluginFinder(IOptions<AppOptions> options, ILogger<PluginFinder<TPlugin>> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IPluginFinder Interface Implementation

        /// <summary>
        /// Finds the assembly locations for all assemblies containing plugins of the specified type.
        /// </summary>
        /// <param name="pluginPath">The path to plugins.</param>
        /// <param name="sharedTypes">A list of shared types to be used when loading plugins into custom assembly contexts.</param>
        /// <returns>A list of assembly locations.</returns>
        public IEnumerable<string> FindPluginAssemblies(string pluginPath, Type[] sharedTypes)
        {
            _logger.LogInformation(InformationMessages.SearchingPluginPath, typeof(TPlugin).Name, pluginPath);

            var assemblyPaths = new List<string>();

            // Check directory
            var path = new DirectoryInfo(pluginPath);
            if (path.Exists)
            {
                var files = path.GetFiles(_options.FindPattern, SearchOption.AllDirectories);
                if (files.Any())
                {
                    _logger.LogInformation(InformationMessages.LibraryCountFoundInPath, files.Length, path.FullName);
                    _logger.LogInformation(InformationMessages.ScanningLibraries);

                    foreach (var libPath in files)
                    {
                        _logger.LogTrace(TraceMessages.CheckingLibraryForPlugins, libPath.FullName);

                        // Create plugin assembly load context to hold the assemblies and dependencies per library
                        var pluginContext = new PluginAssemblyLoadContext(libPath.FullName, sharedTypes);

                        // Don't load the library containing the plugin type itself into the plugin AssemblyLoadContext,
                        // otherwise the IsAssignableFrom won't return a match, because the same assembly in different
                        // contexts have types that are not the same as each other when comparing in .NET Core.
                        if (libPath.Name != new FileInfo(typeof(TPlugin).Assembly.Location).Name)
                        {
                            try
                            {
                                var assembly = pluginContext.LoadFromAssemblyPath(libPath.FullName);

                                foreach (var type in assembly.GetExportedTypes().Where(t => !t.IsAbstract))
                                {
                                    var interfaceType = typeof(TPlugin);
                                    if (interfaceType.IsAssignableFrom(type))
                                    {
                                        _logger.LogTrace(TraceMessages.FoundPlugin, type.FullName, libPath.FullName);

                                        assemblyPaths.Add(type.Assembly.Location);
                                    }
                                }
                            }
                            catch (FileNotFoundException e)
                            {
                                _logger.LogDebug(TraceMessages.IgnoringLibraryMissingDependencies, libPath.FullName);
                                _logger.LogDebug(e.ToString());
                            }
                            catch (BadImageFormatException e)
                            {
                                _logger.LogDebug(TraceMessages.IgnoringLibraryNotDotNetImage, libPath.FullName);
                                _logger.LogDebug(e.ToString());
                            }
                            catch (FileLoadException e)
                            {
                                _logger.LogDebug(TraceMessages.IgnoringLibraryFileLoadFailed, libPath.FullName);
                                _logger.LogDebug(e.ToString());
                            }
                        }

                        // Unload context
                        pluginContext.Unload();

                        // Without collecting deterministically, some directories with lots of DLL's can
                        // eventually cause an Internal CLR error (segmentation fault).  This may still occur
                        // but this alleviates the issue.  Always use a search pattern (--find-pattern arg) to
                        // reduce number of DLL's to scan.
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                else
                {
                    _logger.LogInformation(InformationMessages.LibrariesNotFoundInPath, path.FullName);
                }
            }
            else
            {
                _logger.LogError(ErrorMessages.PluginPathDoesNotExist, path.FullName);
            }

            return assemblyPaths;
        }

        #endregion
    }
}
