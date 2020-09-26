using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// Defines a set of return code constants.
    /// </summary>
    public static class ReturnCodeConstants
    {
        /// <summary>
        /// Defines a success return code.
        /// </summary>
        public const int Success = 0;

        /// <summary>
        /// Defines an error return code, where arguments to command line are incorrect or not available.
        /// </summary>
        public const int ArgsError = 1;

        /// <summary>
        /// Defines an error return code, for application errors.
        /// </summary>
        public const int AppError = 2;

        /// <summary>
        /// Defines a cancelled return code, when the command line is cancelled with CTRL+C.
        /// </summary>
        public const int Cancelled = 3;
    }
}
