using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// This interface defines a unique identifier generator.
    /// </summary>
    public interface IUniqueIdGenerator
    {
        /// <summary>
        /// Generates a unique identifier.
        /// </summary>
        /// <param name="numberBase">Optional base to use when encoding the identifier.</param>
        /// <param name="truncate">Optional number of characters to return.</param>
        /// <returns>The unqiue identifier.</returns>
        string Generate(int numberBase = 36, int truncate = 0);
    }
}
