using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AzureIntegrationMigration.Tool.Options;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AzureIntegrationMigration.Tool.Commands
{
    /// <summary>
    /// Defines the command line options for the verify verb.
    /// </summary>
    public class VerifyCommandBuilder : ICommandBuilder
    {
        /// <summary>
        /// Defines the logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs a new instance of the <see cref="VerifyCommandBuilder"/> class with a logger.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public VerifyCommandBuilder(ILogger<VerifyCommandBuilder> logger)
        {
            // Validate and set members
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region ICommandBuilder Interface Implementation

        /// <summary>
        /// Build the command for the verify verb.
        /// </summary>
        /// <returns>A configured command.</returns>
        public Command Build()
        {
            // Create command
            var command = new Command(OptionsResources.VerifyVerb, OptionsResources.VerifyVerbDescription)
            {
                // Set handler
                Handler = CommandHandler.Create(async (IHost host, CancellationToken token) =>
                {
                    // Get app
                    var app = host.Services.GetRequiredService<IApp>();

                    // Set verb
                    var options = host.Services.GetRequiredService<IAppOptionsService>();
                    options.Verb = CommandVerb.Verify;

                    try
                    {
                        _logger.LogDebug(TraceMessages.InvokingApp, options.Verb);

                        // Invoke app
                        return await app.RunAsync(token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation(InformationMessages.AppCancelled);

                        return ReturnCodeConstants.Cancelled;
                    }
                })
            };

            return command;
        }

        #endregion
    }
}
