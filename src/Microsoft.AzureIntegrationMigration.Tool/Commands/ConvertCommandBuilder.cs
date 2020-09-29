// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
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
    /// Defines the command line options for the convert verb.
    /// </summary>
    public class ConvertCommandBuilder : ICommandBuilder
    {
        /// <summary>
        /// Defines the logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs a new instance of the <see cref="ConvertCommandBuilder"/> class with a logger.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public ConvertCommandBuilder(ILogger<ConvertCommandBuilder> logger)
        {
            // Validate and set members
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region ICommandBuilder Interface Implementation

        /// <summary>
        /// Build the command for the convert verb.
        /// </summary>
        /// <returns>A configured command.</returns>
        public Command Build()
        {
            // Create command
            var command = new Command(OptionsResources.ConvertVerb, OptionsResources.ConvertVerbDescription);

            // Add options (switches)
            var modelAliases = new string[] { OptionsResources.ModelPathOptionShort, OptionsResources.ModelPathOptionLong };
            var modelOption = new Option<FileInfo>(modelAliases, OptionsResources.ModelPathOptionDescription)
            {
                IsRequired = true
            };
            command.AddOption(modelOption);

            var conversionDirAliases = new string[] { OptionsResources.ConversionPathOptionShort, OptionsResources.ConversionPathOptionLong };
            var conversionDirOption = new Option<string>(conversionDirAliases, OptionsResources.ConversionPathOptionDescription);
            command.AddGlobalOption(conversionDirOption);

            var templateDirAliases = new string[] { OptionsResources.TemplatePathOptionShort, OptionsResources.TemplatePathOptionLong };
            var templateDirOption = new Option<DirectoryInfo[]>(templateDirAliases, OptionsResources.TemplatePathOptionDescription);
            command.AddGlobalOption(templateDirOption);

            // Set handler
            command.Handler = CommandHandler.Create(async (IHost host, CancellationToken token) =>
            {
                // Get app
                var app = host.Services.GetRequiredService<IApp>();

                // Set verb
                var options = host.Services.GetRequiredService<IAppOptionsService>();
                options.Verb = CommandVerb.Convert;

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
            });

            return command;
        }

        #endregion
    }
}
