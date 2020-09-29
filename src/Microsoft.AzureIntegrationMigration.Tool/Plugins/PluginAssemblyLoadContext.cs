// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.AzureIntegrationMigration.Runner.Core;

namespace Microsoft.AzureIntegrationMigration.Tool.Plugins
{
    /// <summary>
    /// Defines a custom assembly load context for finding stage runner plugins.
    /// </summary>
    public class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        /// <summary>
        /// Defines an assembly resolver.
        /// </summary>
        private readonly AssemblyDependencyResolver _resolver;

        /// <summary>
        /// Defines the list of shared assemblies.
        /// </summary>
        private readonly IList<string> _sharedAssemblies = new List<string>();

        /// <summary>
        /// Constructs a new instance of the <see cref="PluginAssemblyLoadContext"/> class with a plugin path.
        /// </summary>
        /// <param name="pluginPath">Path to the plugin.</param>
        /// <param name="sharedTypes">The types that should be loaded from default context.</param>
        /// <param name="isCollectible">True, the default, if the context can be collected, otherwise False.</param>
        public PluginAssemblyLoadContext(string pluginFilePath, Type[] sharedTypes, bool isCollectible = true)
            : base(isCollectible)
        {
            _ = pluginFilePath ?? throw new ArgumentNullException(nameof(pluginFilePath));
            _ = sharedTypes ?? throw new ArgumentNullException(nameof(sharedTypes));

            _resolver = new AssemblyDependencyResolver(pluginFilePath ?? throw new ArgumentNullException(nameof(pluginFilePath)));

            // Get assemblies from shared types
            foreach (var type in sharedTypes)
            {
                var sharedAssembly = type.Assembly.GetName().Name;
                if (!_sharedAssemblies.Contains(sharedAssembly))
                {
                    _sharedAssemblies.Add(sharedAssembly);
                }
            }
        }

        /// <summary>
        /// Loads an assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <returns>An assembly.</returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            _ = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));

            // Load shared assemblies from the default context, otherwise the IsAssignableFrom won't return a match, because the same
            // assembly in different contexts have types that are not the same as each other when comparing in .NET Core.
            if (_sharedAssemblies.Contains(assemblyName.Name))
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
            }
            else
            {
                var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath != null)
                {
                    return LoadFromAssemblyPath(assemblyPath);
                }
            }

            return null;
        }

        /// <summary>
        /// Loads an unmanaged library.
        /// </summary>
        /// <param name="unmanagedDllName">The unmanaged DLL name.</param>
        /// <returns>A pointer to the DLL.</returns>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
