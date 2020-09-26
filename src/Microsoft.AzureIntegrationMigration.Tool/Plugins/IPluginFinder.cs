using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AzureIntegrationMigration.Tool.Plugins
{
    /// <summary>
    /// Defines an interface to search for plugins.
    /// </summary>
    public interface IPluginFinder<TPlugin>
    {
        /// <summary>
        /// Finds the assembly locations for all assemblies containing a plugin interface.
        /// </summary>
        /// <param name="pluginPath">The path to plugin assemblies.</param>
        /// <param name="sharedTypes">A list of shared types to be used when loading plugins into custom assembly contexts.</param>
        /// <returns>A list of assembly locations.</returns>
        IEnumerable<string> FindPluginAssemblies(string pluginPath, Type[] sharedTypes);
    }
}
