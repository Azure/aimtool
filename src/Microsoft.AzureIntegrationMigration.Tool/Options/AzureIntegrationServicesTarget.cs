// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AzureIntegrationMigration.Tool.Options
{
    /// <summary>
    /// Defines an enum of the AIS target environments for an assessment and conversion.
    /// </summary>
    public enum AzureIntegrationServicesTarget
    {
        /// <summary>
        /// The default target is the AIS consumption service .
        /// </summary>
        Consumption = 0,

        /// <summary>
        /// The target is the Azure Integration Service Environment.
        /// </summary>
        Ise = 1
    }
}
