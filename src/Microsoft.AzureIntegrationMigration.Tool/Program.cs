// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// Defines the entrypoint for the CLI tool.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entrypoint for the CLI tool.
        /// </summary>
        /// <param name="args">The arguments passed from the command line.</param>
        public static async Task<int> Main(string[] args)
        {
            // Build host, parse and run
            var (result, parseResult) = Host.ParseArgs(args);
            return result == ReturnCodeConstants.Success ?
                await parseResult.InvokeAsync().ConfigureAwait(false) :
                await Task.FromResult(result).ConfigureAwait(false);
        }
    }
}
