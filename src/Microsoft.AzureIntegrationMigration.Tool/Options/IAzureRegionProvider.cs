// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Microsoft.AzureIntegrationMigration.Tool.Logging
{
    /// <summary>
    /// Defines an interface for a provider that maps Azure regions.
    /// </summary>
    public interface IAzureRegionProvider
    {
        /// <summary>
        /// Gets the paired region.
        /// </summary>
        /// <param name="region">The region name.</param>
        /// <returns>The paired region display name, or null if the specified region does not exist.</returns>
        string GetPairedRegion(string region);

        /// <summary>
        /// Validates that the supplied region exists.
        /// </summary>
        /// <param name="region">The region name.</param>
        /// <returns>True if the region exists, otherwise False.</returns>
        bool RegionExists(string region);
    }
}
