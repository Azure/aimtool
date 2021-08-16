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
        /// The target is the multi-tenant Logic Apps Consumption environment.
        /// </summary>
        Consumption = 0,

        /// <summary>
        /// The target is the single-tenant Logic Apps Standard environment.
        /// </summary>
        Standard = 1,

        /// <summary>
        /// The target is the lite-version of the multi-tenant Logic Apps Consumption environment.
        /// </summary>
        ConsumptionLite = 2,

        /// <summary>
        /// The target is the lite-version of the single-tenant Logic Apps Standard environment.
        /// </summary>
        StandardLite = 3
    }
}
