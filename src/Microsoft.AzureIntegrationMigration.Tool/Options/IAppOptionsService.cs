// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
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
    /// Defines an interface for handling command line and file based options.
    /// </summary>
    public interface IAppOptionsService
    {
        /// <summary>
        /// Gets or sets the verb used on the command line.
        /// </summary>
        CommandVerb Verb { get; set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        AppOptions Options { get; }

        /// <summary>
        /// Merges all of the configuration from the appsettings.json file to the parsed command line
        /// options, where options on the command line take precedence.
        /// </summary>
        void MergeConfig();

        /// <summary>
        /// Prints the options out to the logger.
        /// </summary>
        void PrintOptions();

        /// <summary>
        /// Applies default values to options that haven't been set in the config file or overridden
        /// on the command line.
        /// </summary>
        void ApplyDefaults();

        /// <summary>
        /// Converts command line arguments to arbitrary arguments for the runner.
        /// </summary>
        /// <returns>Parsed and converted args.</returns>
        IDictionary<string, object> ParseArgs();

        /// <summary>
        /// Validates the command line options provided to the app.
        /// </summary>
        /// <returns>A list of errors, if any.</returns>
        IEnumerable<string> Validate();
    }
}
