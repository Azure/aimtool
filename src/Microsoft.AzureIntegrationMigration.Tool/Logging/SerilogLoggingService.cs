using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Microsoft.AzureIntegrationMigration.Tool.Logging
{
    /// <summary>
    /// Defines a logging service based on Serilog.
    /// </summary>
    public class SerilogLoggingService : ILoggingService
    {
        /// <summary>
        /// Defines the application configuration.
        /// </summary>
        private readonly IConfigurationRoot _config;

        /// <summary>
        /// Defines a log switch.
        /// </summary>
        private readonly LoggingLevelSwitch _logSwitch;

        /// <summary>
        /// Constructs a new instance of the <see cref="SerilogLoggingService"/> class with the application configuration.
        /// </summary>
        /// <param name="config">The application configuration.</param>
        public SerilogLoggingService(IConfigurationRoot config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logSwitch = new LoggingLevelSwitch(LogEventLevel.Information);
        }

        /// <summary>
        /// Sets the logging level to the specified level.
        /// </summary>
        /// <param name="level">The level to set.</param>
        public void SetLoggingLevel(LogLevel level)
        {
            // Map level
            switch (level)
            {
                case LogLevel.Critical:
                    _logSwitch.MinimumLevel = LogEventLevel.Fatal;
                    break;

                case LogLevel.Error:
                    _logSwitch.MinimumLevel = LogEventLevel.Error;
                    break;

                case LogLevel.Warning:
                    _logSwitch.MinimumLevel = LogEventLevel.Warning;
                    break;

                case LogLevel.Information:
                    _logSwitch.MinimumLevel = LogEventLevel.Information;
                    break;

                case LogLevel.Debug:
                    _logSwitch.MinimumLevel = LogEventLevel.Debug;
                    break;

                case LogLevel.Trace:
                    _logSwitch.MinimumLevel = LogEventLevel.Verbose;
                    break;

                default:
                    break;
            }
        }


        /// <summary>
        /// Builds a logger.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>Returns the logging builder.</returns>
        public ILoggingBuilder BuildLogger(ILoggingBuilder builder)
        {
            // Create logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_config)
                .MinimumLevel.ControlledBy(_logSwitch)
                .CreateLogger();

            return builder.AddSerilog(dispose: true);
        }
    }
}
