// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.AzureIntegrationMigration.Tool.Logging
{
    /// <summary>
    /// Implements a region provider using an embedded JSON resource.
    /// </summary>
    public class AzureRegionResourceProvider : IAzureRegionProvider
    {
        /// <summary>
        /// Defines the name of the file containing the Azure regions.
        /// </summary>
        private const string RegionFile = "regions.json";

        /// <summary>
        /// Defines a JSON array of regions.
        /// </summary>
        private readonly JArray _regions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureRegionResourceProvider"/> class.
        /// </summary>
        public AzureRegionResourceProvider()
        {
            // Load region resource
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(RegionFile)))
            {
                var json = reader.ReadToEnd();
                _regions = JArray.Parse(json);
            }
        }

        /// <summary>
        /// Gets the paired region.
        /// </summary>
        /// <param name="region">The region name.</param>
        /// <returns>The paired region display name, or null if the specified region does not exist.</returns>
        public string GetPairedRegion(string region)
        {
            // Returns the paired region display name, or returns null if region doesn't exist
            var jsonDisplayNamePath = $"$[?(@.DisplayName == '{region}')]";
            var selectedRegion = _regions.SelectToken(jsonDisplayNamePath);
            if (selectedRegion != null)
            {
                var pairedRegionName = selectedRegion["Pair"].Value<string>();
                var jsonNamePath = $"$[?(@.Name == '{pairedRegionName}')]";
                var pairedRegion = _regions.SelectToken(jsonNamePath);
                if (pairedRegion != null)
                {
                    return pairedRegion["DisplayName"].Value<string>();
                }
            }

            return null;
        }

        /// <summary>
        /// Validates that the supplied region exists.
        /// </summary>
        /// <param name="region">The region name.</param>
        /// <returns>True if the region exists, otherwise False.</returns>
        public bool RegionExists(string region)
        {
            var jsonPath = $"$[?(@.DisplayName == '{region}')]";
            var selectedRegion = _regions.SelectToken(jsonPath);
            if (selectedRegion != null)
            {
                return true;
            }

            return false;
        }
    }
}
