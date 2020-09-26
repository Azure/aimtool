using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Tool.Logging;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AzureIntegrationMigration.Tool.Options
{
    /// <summary>
    /// Defines the common app command line options.
    /// </summary>
    public class AppOptions
    {
        #region Common Options

        /// <summary>
        /// Gets or sets a value indicating whether the app should log verbosely or not.
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets the level of verbose logging.
        /// </summary>
        public char VerboseLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the main runner should not abort (fail fast)
        /// if an exception occurred with a stage runner.  By default, always abort.
        /// </summary>
        public bool NoAbort { get; set; }

        /// <summary>
        /// Gets or sets one or more stages indicating whether to abort at the end of that stage
        /// if an exception occurred within that stage.
        /// </summary>
        public IEnumerable<Stages> AbortStage { get; set; }

        /// <summary>
        /// Gets or sets one or more paths to find stage runners.
        /// </summary>
        public IEnumerable<DirectoryInfo> FindPath { get; set; }

        /// <summary>
        /// Gets or sets the find pattern to find stage runners.
        /// </summary>
        public string FindPattern { get; set; }

        /// <summary>
        /// Gets or sets an arbitrary set of arguments that can be used by stage runners.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Set from command builder.")]
        public IDictionary<string, string> Arg { get; set; }

        /// <summary>
        /// Gets or sets the delimiter for a multi-value argument.
        /// </summary>
        public string ArgDelimiter { get; set; }

        /// <summary>
        /// Gets or sets the working directory any temp output of the tool.
        /// </summary>
        public DirectoryInfo WorkingPath { get; set; }

        /// <summary>
        /// Gets or sets the state path for exported execution state.  Default is working directory.
        /// </summary>
        public string StatePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the app should save execution state before and after
        /// each stage runner.
        /// </summary>
        public bool SaveState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the app should save execution state before and after
        /// each stage.
        /// </summary>
        public bool SaveStageState { get; set; }

        /// <summary>
        /// Gets or sets the target for the migration to Azure.
        /// </summary>
        public AzureIntegrationServicesTarget Target { get; set; }

        /// <summary>
        /// Gets or sets the Azure subscription ID.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the Azure primary region.
        /// </summary>
        public string PrimaryRegion { get; set; }

        /// <summary>
        /// Gets or sets the Azure secondary region.
        /// </summary>
        public string SecondaryRegion { get; set; }

        /// <summary>
        /// Gets or sets the target deployment environment.
        /// </summary>
        public string DeploymentEnv { get; set; }

        /// <summary>
        /// Gets or sets the unique deployment ID.
        /// </summary>
        public string UniqueDeploymentId { get; set; }

        #endregion

        #region Assess Options

        /// <summary>
        /// Gets or sets the output path to a file for a saved model.
        /// </summary>
        public string OutputModel { get; set; }

        #endregion

        #region Migrate and Assess Options

        /// <summary>
        /// Gets or sets the path where the template resource configuration files are stored.  Default
        /// is working directory.
        /// </summary>
        public DirectoryInfo TemplateConfigPath { get; set; }

        #endregion

        #region Convert Options

        /// <summary>
        /// Gets or sets a file path to an exported model file.
        /// </summary>
        public FileInfo Model { get; set; }

        #endregion

        #region Migrate and Convert Options

        /// <summary>
        /// Gets or sets the directory for conversion output of the tool.
        /// </summary>
        public string ConversionPath { get; set; }

        /// <summary>
        /// Gets or sets one or more paths where the template resource files are stored.
        /// </summary>
        public IEnumerable<DirectoryInfo> TemplatePath { get; set; }

        #endregion
    }
}
