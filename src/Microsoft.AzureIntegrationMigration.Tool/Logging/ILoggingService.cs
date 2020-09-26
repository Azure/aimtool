using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Tool.Logging
{
    /// <summary>
    /// Defines an interface for a logging service.
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Sets the logging level to the specified level.
        /// </summary>
        /// <param name="level">The level to set.</param>
        void SetLoggingLevel(LogLevel level);

        /// <summary>
        /// Builds a logger.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>Returns the logging builder.</returns>
        ILoggingBuilder BuildLogger(ILoggingBuilder builder);
    }
}
