// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AzureIntegrationMigration.Runner.Core;
using Microsoft.AzureIntegrationMigration.Runner.Engine;
using Microsoft.AzureIntegrationMigration.Tool.Commands;
using Microsoft.AzureIntegrationMigration.Tool.Options;
using Microsoft.AzureIntegrationMigration.Tool.Resources;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// Defines the interface for the application.
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// Run the app with specific command line options.
        /// </summary>
        /// <param name="token">A token used to cancel the operation.</param>
        /// <returns>A task used to await the operation with a return code for the CLI.</returns>
        Task<int> RunAsync(CancellationToken token);
    }
}
