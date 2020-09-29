// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Tool.Options;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Tool.Commands
{
    /// <summary>
    /// Defines a command builder for the root command.
    /// </summary>
    public class RootCommandBuilder : ICommandBuilder
    {
        /// <summary>
        /// Defines the list of builders for commands to add to the root command.
        /// </summary>
        private readonly IEnumerable<ICommandBuilder> _builders;

        /// <summary>
        /// Defines the logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructs a new instance of the <see cref="RootCommandBuilder"/> class with an app instance.
        /// </summary>
        /// <param name="builders">The list of sub-command builders.</param>
        /// <param name="logger">The logger instance.</param>
        public RootCommandBuilder(IEnumerable<ICommandBuilder> builders, ILogger<RootCommandBuilder> logger)
        {
            // Validate and set members
            _builders = builders ?? throw new ArgumentNullException(nameof(builders));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region ICommandBuilder Interface Implementation

        /// <summary>
        /// Build the root command.
        /// </summary>
        /// <returns>A configured root command.</returns>
        public Command Build()
        {
            var command = new RootCommand();

            // Add sub-commands
            foreach (var builder in _builders)
            {
                _logger.LogDebug(TraceMessages.AddingSubCommand, builder.GetType().Name);

                command.AddCommand(builder.Build());
            }

            // Add root level options (available to all sub-commands)
            var verboseAliases = new string[] { OptionsResources.VerboseOptionShort, OptionsResources.VerboseOptionLong };
            var verboseOption = new Option<bool>(verboseAliases, OptionsResources.VerboseOptionDescription);
            command.AddGlobalOption(verboseOption);

            var verboseLevelOption = new Option<char>(OptionsResources.VerboseLevelOptionLong, () => '-', OptionsResources.VerboseLevelOptionDescription);
            command.AddGlobalOption(verboseLevelOption);

            var noAbortOption = new Option<bool>(OptionsResources.NoAbortOptionLong, OptionsResources.NoAbortOptionDescription);
            command.AddGlobalOption(noAbortOption);

            var abortStageOption = new Option<Stages[]>(OptionsResources.AbortStageOptionLong, OptionsResources.AbortStageOptionDescription);
            command.AddGlobalOption(abortStageOption);

            var findPathAliases = new string[] { OptionsResources.FindPathsOptionShort, OptionsResources.FindPathsOptionLong };
            var findPathOption = new Option<DirectoryInfo[]>(findPathAliases, OptionsResources.FindPathsOptionDescription);
            command.AddGlobalOption(findPathOption);

            var findPatternAliases = new string[] { OptionsResources.FindPatternOptionShort, OptionsResources.FindPatternOptionLong };
            var findPatternOption = new Option<string>(findPatternAliases, OptionsResources.FindPatternOptionDescription);
            command.AddGlobalOption(findPatternOption);

            var parseFunc = new ParseArgument<Dictionary<string, string>>(argResult => argResult.Tokens.Select(t => t.Value.Split('=')).ToDictionary(v => v[0], v => v.Length > 1 ? v[1] : ""));
            var argumentAliases = new string[] { OptionsResources.ArgumentsOptionShort, OptionsResources.ArgumentsOptionLong };
            var argumentOption = new Option<Dictionary<string, string>>(argumentAliases, parseFunc, description: OptionsResources.ArgumentsOptionDescription);
            command.AddGlobalOption(argumentOption);

            var argumentDelimiterOption = new Option<string>(OptionsResources.ArgumentDelimiterOptionLong, OptionsResources.ArgumentDelimiterOptionDescription);
            command.AddGlobalOption(argumentDelimiterOption);

            var workingDirAliases = new string[] { OptionsResources.WorkingPathOptionShort, OptionsResources.WorkingPathOptionLong };
            var workingDirOption = new Option<DirectoryInfo>(workingDirAliases, OptionsResources.WorkingPathOptionDescription);
            command.AddGlobalOption(workingDirOption);

            var stateDirAliases = new string[] { OptionsResources.StatePathOptionShort, OptionsResources.StatePathOptionLong };
            var stateDirOption = new Option<string>(stateDirAliases, OptionsResources.StatePathOptionDescription);
            command.AddGlobalOption(stateDirOption);

            var saveStateOption = new Option<bool>(OptionsResources.SaveStageRunnerStateOptionLong, OptionsResources.SaveStageRunnerStateOptionDescription);
            command.AddGlobalOption(saveStateOption);

            var saveStageStateOption = new Option<bool>(OptionsResources.SaveStageStateOptionLong, OptionsResources.SaveStageStateOptionDescription);
            command.AddGlobalOption(saveStageStateOption);

            var azureTargetOption = new Option<AzureIntegrationServicesTarget>(OptionsResources.AzureTargetOptionLong, OptionsResources.AzureTargetOptionDescription);
            command.AddGlobalOption(azureTargetOption);

            var subscriptionIdOption = new Option<string>(OptionsResources.AzureSubscriptionIdOptionLong, OptionsResources.AzureSubscriptionIdOptionDescription);
            command.AddGlobalOption(subscriptionIdOption);

            var primaryRegionOption = new Option<string>(OptionsResources.AzurePrimaryRegionOptionLong, OptionsResources.AzurePrimaryRegionOptionDescription);
            command.AddGlobalOption(primaryRegionOption);

            var secondaryRegionOption = new Option<string>(OptionsResources.AzureSecondaryRegionOptionLong, OptionsResources.AzureSecondaryRegionOptionDescription);
            command.AddGlobalOption(secondaryRegionOption);

            var deploymentEnvAliases = new string[] { OptionsResources.DeploymentEnvOptionShort, OptionsResources.DeploymentEnvOptionLong };
            var deploymentEnvOption = new Option<string>(deploymentEnvAliases, OptionsResources.DeploymentEnvOptionDescription);
            command.AddGlobalOption(deploymentEnvOption);

            var uniqueDeploymentIdOption = new Option<string>(OptionsResources.UniqueDeploymentIdOptionLong, string.Format(CultureInfo.CurrentCulture, OptionsResources.UniqueDeploymentIdOptionDescription, AppOptionsService.UniqueDeploymentIdLengthLimit));
            command.AddGlobalOption(uniqueDeploymentIdOption);

            // Set handler
            command.Handler = CommandHandler.Create(async () =>
            {
                _logger.LogError(ErrorMessages.SpecifyVerb, OptionsResources.AssessVerb, OptionsResources.MigrateVerb, OptionsResources.ConvertVerb, OptionsResources.VerifyVerb);

                return await Task.FromResult<int>(ReturnCodeConstants.ArgsError).ConfigureAwait(false);
            });

            return command;
        }

        #endregion
    }
}
