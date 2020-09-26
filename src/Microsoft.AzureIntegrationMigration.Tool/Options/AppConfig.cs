using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Tool.Resources;

namespace Microsoft.AzureIntegrationMigration.Tool.Options
{
    /// <summary>
    /// Defines the app configuration options.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Gets or sets the working directory any temp output of the tool.
        /// </summary>
        public string WorkingPath { get; set; }

        /// <summary>
        /// Gets or sets the state path for exported execution state.
        /// </summary>
        public string StatePath { get; set; }

        /// <summary>
        /// Gets or sets the find pattern to find stage runners.
        /// </summary>
        public string FindPattern { get; set; }

        /// <summary>
        /// Gets or sets one or more paths to find stage runners.
        /// </summary>
        public IEnumerable<string> FindPaths { get; set; }

        /// <summary>
        /// Gets or sets the path for template configuration files.
        /// </summary>
        public string TemplateConfigPath { get; set; }

        /// <summary>
        /// Gets or sets one or more paths for template files.
        /// </summary>
        public IEnumerable<string> TemplatePaths { get; set; }

        /// <summary>
        /// Gets or sets the Azure primary region.
        /// </summary>
        public string PrimaryRegion { get; set; }

        /// <summary>
        /// Gets or sets the Azure secondary region.
        /// </summary>
        public string SecondaryRegion { get; set; }
    }
}
