// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using Microsoft.AzureIntegrationMigration.Tool.Commands;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// Defines a class that builds the command line parser.
    /// </summary>
    public static class Host
    {
        /// <summary>
        /// Builds the host and parses the command line args.
        /// </summary>
        /// <param name="args">The arguments to parse.</param>
        /// <returns>A return code and the parsing result.</returns>
        public static (int, ParseResult) ParseArgs(string[] args)
        {
            // Configure dependency container
            var provider = DependencyContainer.BuildAppContainer(args);
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(Host));
            var config = provider.GetRequiredService<IConfigurationRoot>();

            try
            {
                // Get root command
                var command = provider.GetRequiredService<RootCommandBuilder>().Build();

                // Build command line host
                var parser = new CommandLineBuilder(command)
                    .UseHost(configureHost: delegate (IHostBuilder h)
                    {
                        h = h.ConfigureServices((context, services) =>
                        {
                            DependencyContainer.ConfigureHostContainer(services, args);
                        });

                        h = h.ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            var env = hostingContext.HostingEnvironment;

                            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false);
                        });
                    })
                    .UseDefaults()
                    .UseHelpBuilder(context => new FileHelperBuilder(context.Console, config))
                    .Build();

                // Parse command line args
                var parseResult = parser.Parse(args);

                return (ReturnCodeConstants.Success, parseResult);
            }
            catch (ArgumentException e)
            {
                logger.LogError(ErrorMessages.FailedParsingCommandLineArgs, e.Message);

                return (ReturnCodeConstants.ArgsError, null);
            }
        }
    }
}
