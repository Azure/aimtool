// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.CommandLine.Hosting;
using System.Linq;
using Microsoft.AzureIntegrationMigration.Runner;
using Microsoft.AzureIntegrationMigration.Runner.Builder;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Model;
using Microsoft.AzureIntegrationMigration.Tool.Commands;
using Microsoft.AzureIntegrationMigration.Tool.Logging;
using Microsoft.AzureIntegrationMigration.Tool.Options;
using Microsoft.AzureIntegrationMigration.Tool.Plugins;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// Defines a class that builds an IoC container with services for the app.
    /// </summary>
    public static class DependencyContainer
    {
        /// <summary>
        /// Build the app container with all services, used to build commands and start the app.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A service provider.</returns>
        public static IServiceProvider BuildAppContainer(string[] args)
        {
            // Create container
            var services = new ServiceCollection();

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            services.AddSingleton<IConfigurationRoot>(configuration);

            // Create and add logging
            var loggingService = new SerilogLoggingService(configuration);
            loggingService.SetLoggingLevel(args.Any(a => a == OptionsResources.VerboseOptionShort || a == OptionsResources.VerboseOptionLong) ? LogLevel.Debug : LogLevel.Information);
            services.AddSingleton<ILoggingService>(loggingService);
            services.AddLogging(builder => loggingService.BuildLogger(builder));

            // Add command builders
            services.AddTransient<ICommandBuilder, AssessCommandBuilder>();
            services.AddTransient<ICommandBuilder, MigrateCommandBuilder>();
            services.AddTransient<ICommandBuilder, ConvertCommandBuilder>();
            services.AddTransient<ICommandBuilder, VerifyCommandBuilder>();
            services.AddTransient<RootCommandBuilder>();

            // Build provider
            var provider = services.BuildServiceProvider();
            return provider;
        }

        /// <summary>
        /// Configures the .NET generic host container with services required to handle command line commands.
        /// </summary>
        /// <param name="container">The container to add services to.</param>
        /// <param name="args">The command line arguments.</param>
        public static void ConfigureHostContainer(IServiceCollection container, string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Add unique ID generator
            container.AddTransient<IUniqueIdGenerator, UniqueIdGenerator>();

            // Create and add logging
            var loggingService = new SerilogLoggingService(configuration);
            loggingService.SetLoggingLevel(args.Any(a => a == OptionsResources.VerboseOptionShort || a == OptionsResources.VerboseOptionLong) ? LogLevel.Debug : LogLevel.Information);
            container.AddSingleton<ILoggingService>(loggingService);
            container.AddLogging(builder => loggingService.BuildLogger(builder));

            // Add options
            container.AddOptions<AppOptions>().BindCommandLine();
            container.AddSingleton<IAppOptionsService, AppOptionsService>();

            // Add region provider
            container.AddSingleton<IAzureRegionProvider, AzureRegionResourceProvider>();

            // Add plugin finders
            container.AddTransient<IPluginFinder<IStageRunner>, PluginFinder<IStageRunner>>();
            container.AddTransient<IPluginFinder<IApplicationModelProvider>, PluginFinder<IApplicationModelProvider>>();

            // Add plugin host
            container.AddSingleton<IPluginHost<IRunnerComponent>, PluginHost<IRunnerComponent>>();

            // Add stage component provider
            container.AddTransient<IStageComponentProvider, PluginStageComponentProvider>();

            // Add model component provider
            container.AddTransient<PluginModelComponentProvider>();

            // Add migration runner builder
            container.AddTransient<IRunnerBuilder, RunnerBuilder>();

            // Add application service
            container.AddTransient<IApp, App>();
        }
    }
}
