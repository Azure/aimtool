using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.AzureIntegrationMigration.Tool.Plugins
{
    /// <summary>
    /// Defines an interface to host plugins.
    /// </summary>
    public interface IPluginHost<TPlugin>
    {
        /// <summary>
        /// Resolves the assembly from an already used plugin assembly load context, if possible.
        /// </summary>
        /// <param name="assemblyName">The assembly to resolve.</param>
        /// <returns>The assembly it has resolved to, or null if none found.</returns>
        Assembly ResolveAssembly(AssemblyName assemblyName);

        /// <summary>
        /// Get the loaded plugins.
        /// </summary>
        /// <returns>A list of loaded plugins.</returns>
        IEnumerable<TPlugin> GetPlugins();

        /// <summary>
        /// Finds the assembly locations for all assemblies containing a plugin interface.
        /// </summary>
        /// <param name="assemblyPaths">A list of assembly paths containing plugins.</param>
        /// <param name="sharedTypes">A list of shared types to be used when loading plugins into custom assembly contexts.</param>
        void LoadPlugins(IEnumerable<string> assemblyPaths, Type[] sharedTypes);
    }
}
