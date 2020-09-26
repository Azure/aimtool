# Packages, CI and Distribution

- [Types of package](#types-of-package)
- [Chocolatey packages](#chocolatey-packages)
- [NuGet packages](#nuget-packages)
- [Continuous Integration](#continuous-integration)

## Types of package

The BizTalk Migrator is distributed via NuGet packages, with Chocolatey being the preferred platform
for distributing and downloading the tool.

These are the packages that make up the distribution:

## Chocolatey packages

### biztalkmigrator

This is the core install for the BizTalk Migrator, and installing this will install the dependent packages below.

### biztalkmigrator-azure

This installs the Azure templates that the Migrator uses to build your Azure application.

### biztalkmigrator-biztalk

This is the BizTalk-specific conversion components. 

### biztalkmigrator-cli

This is the Command-Line Interface (CLI) for the BizTalk Migrator.

## NuGet packages

### Microsoft.AzureIntegrationMigration.Runner

This is the core components that run the migration process.

### Microsoft.AzureIntegrationMigration.BizTalk.StageRunners

These are the logic components that perform the migration, and are used by the Runner
in the execution pipeline.

### Microsoft.AzureIntegrationMigration.ApplicationModel

This is the object model representing the target solution in Azure.

### Microsoft.AzureIntegrationMigration.BizTalk.Types

This is the type library representing BizTalk objects.

### Microsoft.AzureIntegrationMigration.BizTalk.Discover

This contains components spcific to discovering BizTalk artefacts, for example unpacking MSIs.

### Microsoft.AzureIntegrationMigration.BizTalk.Parse

This contains the components specific to parsing BizTalk artefacts, for example deserializing ODX files.

### Microsoft.AzureIntegrationMigration.BizTalk.Analyze

This contains the logic for analyzing BizTalk applications and building a model representation
of those applications in Azure.

### Microsoft.AzureIntegrationMigration.BizTalk.Report

This contains components that generate a report on the source and target applications.

### Microsoft.AzureIntegrationMigration.BizTalk.Convert

This contains the components for converting the model of the target application into 
Azure artefacts such as ARM templates and deployment scripts.

## Continuous Integration

The BizTalk Migrator uses GitHub Actions as the platform for Continuous Integration (CI).

The CI build will publish NuGet packages to the GitHub package repository.

Chocolatey packages are published to the Chocolatery repository (https://chocolatey.org/).