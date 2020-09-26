using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Help;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// Defines a class that takes the content of a help file and appends to the command
    /// line help text, if the file exists.
    /// </summary>
    public class FileHelperBuilder : HelpBuilder
    {
        /// <summary>
        /// Defines a constant for the filename for displaying additional help.
        /// </summary>
        private const string HelpFile = "additionalhelp.txt";

        /// <summary>
        /// Defines the application configuration.
        /// </summary>
        private readonly IConfigurationRoot _config;

        /// <summary>
        /// Constructs a new instance of the <see cref="FileHelperBuilder"/> class with the console to write text to.
        /// </summary>
        /// <param name="console">The console to write text to.</param>
        /// <param name="config">The application configuration.</param>
        public FileHelperBuilder(IConsole console, IConfigurationRoot config)
            : base(console)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Finds the help file if it exists and appends contents to regular command line help.
        /// </summary>
        /// <param name="command">The existing command.</param>
        public override void Write(ICommand command)
        {
            base.Write(command);

            // The main help file will be in the same directory as the assembly.
            var appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var mainHelpFile = Path.Combine(appDirectory, HelpFile);

            // Find main help file to add to command line
            if (File.Exists(mainHelpFile))
            {
                var helpContent = File.ReadAllText(mainHelpFile);
                base.Console.Out.Write(helpContent);
            }

            // Find help files to add to command line
            var findPaths = _config.GetSection("AppConfig:FindPaths").Get<string[]>();
            if (findPaths != null)
            {
                foreach (var findPath in findPaths)
                {
                    var helpFiles = Directory.GetFiles(findPath, HelpFile, SearchOption.AllDirectories);
                    if (helpFiles != null && helpFiles.Any())
                    {
                        foreach (var helpFile in helpFiles)
                        {
                            if (File.Exists(helpFile))
                            {
                                var helpContent = File.ReadAllText(helpFile);
                                base.Console.Out.Write(helpContent);
                            }
                        }
                    }
                }
            }
        }
    }
}
