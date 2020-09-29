// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AzureIntegrationMigration.Tool.Options
{
    /// <summary>
    /// Defines an enum of the allowable command verbs.
    /// </summary>
    public enum CommandVerb
    {
        /// <summary>
        /// The default verb is assess.
        /// </summary>
        Assess = 0,

        /// <summary>
        /// The verb is migrate.
        /// </summary>
        Migrate = 1,

        /// <summary>
        /// The verb is convert.
        /// </summary>
        Convert = 2,

        /// <summary>
        /// The verb is verify.
        /// </summary>
        Verify = 3
    }
}
