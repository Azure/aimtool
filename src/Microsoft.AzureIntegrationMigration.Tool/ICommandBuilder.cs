// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// Defines an interface for a command builder for a command line verb.
    /// </summary>
    public interface ICommandBuilder
    {
        /// <summary>
        /// Builds the command.
        /// </summary>
        /// <returns>A new command representing the root command or a sub-command (verb).</returns>
        Command Build();
    }
}
