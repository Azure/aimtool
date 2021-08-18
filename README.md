# aimtool
[![Build Status](https://github.com/azure/aimtool/workflows/CI%20Build/badge.svg)](https://github.com/azure/aimtool/actions)

# Check the [Change Log](./CHANGELOG.md) for the latest updates

```
$$$$$$$\  $$\        $$$$$$$$\        $$\ $$\                              
$$  __$$\ \__|       \__$$  __|       $$ |$$ |                             
$$ |  $$ |$$\ $$$$$$$$\ $$ | $$$$$$\  $$ |$$ |  $$\                        
$$$$$$$\ |$$ |\____$$  |$$ | \____$$\ $$ |$$ | $$  |                       
$$  __$$\ $$ |  $$$$ _/ $$ | $$$$$$$ |$$ |$$$$$$  /                        
$$ |  $$ |$$ | $$  _/   $$ |$$  __$$ |$$ |$$  _$$<                         
$$$$$$$  |$$ |$$$$$$$$\ $$ |\$$$$$$$ |$$ |$$ | \$$\                        
\_______/ \__|\________|\__| \_______|\__|\__|  \__|                       
                                                                           
                                                                           
                                                                           
$$\      $$\ $$\                               $$\                         
$$$\    $$$ |\__|                              $$ |                        
$$$$\  $$$$ |$$\  $$$$$$\   $$$$$$\  $$$$$$\ $$$$$$\    $$$$$$\   $$$$$$\  
$$\$$\$$ $$ |$$ |$$  __$$\ $$  __$$\ \____$$\\_$$  _|  $$  __$$\ $$  __$$\ 
$$ \$$$  $$ |$$ |$$ /  $$ |$$ |  \__|$$$$$$$ | $$ |    $$ /  $$ |$$ |  \__|
$$ |\$  /$$ |$$ |$$ |  $$ |$$ |     $$  __$$ | $$ |$$\ $$ |  $$ |$$ |      
$$ | \_/ $$ |$$ |\$$$$$$$ |$$ |     \$$$$$$$ | \$$$$  |\$$$$$$  |$$ |      
\__|     \__|\__| \____$$ |\__|      \_______|  \____/  \______/ \__|      
                 $$\   $$ |                                                
                 \$$$$$$  |                                                
                  \______/                                                 
```

[![Survey](./docs/images/biztalk-migrator-questionnaire.png)](https://aka.ms/biztalkmigrationsurvey)
# BizTalk Migrator at-a-glance
IMPORTANT: This repository was opened publicly on October 26 - the chocolatey package may take a day or two to become available on the chocolatey site.

This repository contains the core CLI implementation for the BizTalk Migrator.

The BizTalk Migrator is a command-line tool that helps migrate BizTalk applications to Azure Integration Services (AIS).
This is implemented across several GitHub repositories - aimcore, aimmodel, aimtool, aimazure, and aimbiztalk.

Full documentation is available here: [Documentation](./docs/README.md).

- If you just want to run the tool, or find out why something went wrong, refer to the [Quick Start Guide](./docs/quick-start-guide.md).  
- If you want to run the sample scenarios, look here: [Example Scenarios](./docs/user-guide/scenarios/README.md).  
- For answers to commonly asked questions, check the [Frequently Asked Questions](./docs/frequently-asked-questions.md).  

## What is the BizTalk Migrator?

- The BizTalk Migrator is a command-line interface (CLI) for migrating BizTalk assets to Azure.  
- It uses a plug-in architecture, allowing new functionality/features to be added, so the tool can be extended to cover additional migration scenarios.
- This is an open-source project run by Microsoft, delivered for the benefit of BizTalk (and AIS) customers.

- The tool runs through a number of phases:
    - Discover/Parse - pull out the BizTalk resources (currently BizTalk exported MSI(s) only).
    - Analyse - look at what BizTalk resources we have, and what can be migrated and build a model of the AIS target.
    - Report - Generate a report outlining what BizTalk resources were found, and what can be migrated.
    - Convert - Generates a series of ARM templates and Azure CLI scripts, generates Logic Apps representing orchestrations.

## What everyone should know

- BizTalk migration is complex, and it is cost prohibitive to automatically convert everything.
- Over 20 years, developers have found myriad ways to use BizTalk; not all of these are currently supported in Azure.
- This tool is a first step: to begin with, it will only completely convert a limited number of scenarios.
- The number of supported scenarios will grow over time.
- The tool will convert as much as it can, so even when your scenario isn't completely converted you should still get some value from it and manually convert the rest.
- If it doesn't do what you want, then get involved - vote for additional features, or help write additional plugins.
- This is a complex tool, as it's a complex problem to solve - don't expect it to work miracles!

## What were the design goals?

- Successfully migrate the FTP/File Mover scenario (used by over 80&percnt; of BizTalk customers).
- Successfully migrate a simple HTTP request-response scenario.
- Build a complete model of a BizTalk application, using Enterprise Patterns.
- Put in place the building blocks to allow customers to complete the migration themselves.
- Provide a concrete foundation for a tool that will increase in capability over time.

## What can it do?

- Parse BizTalk exported MSI files, build a model of all BizTalk artifacts.
- Report on everything found in MSI files.
- Handle these adapters: File; FTP; HTTP; SFTP.
- Handle these pipelines:
    - XmlReceive
    - XmlTransmit
    - PassthruReceive
    - PassthruTransmit
- Handle these pipeline components:
    - XmlDisassembler*
    - XmlAssembler*
    - XmlValidator
    - JsonDecoder
    - JsonEncoder
    - FlatFileDecoder
- Handle these orchestration entities:
    - Variables
    - Ports/Port Types
    - Receive Shapes
    - Send Shapes
    - Construct Shapes
    - Transform Shapes
- Handle Transforms in Receive Ports/Send Ports.
- Handle Property Promotion and Demotion in pipelines.
- Generate placeholders for unsupported orchestration shapes.
- Create a library of common services and Logic Apps that can be used by any integration application.

*The current implementation covers the most common usage of these BizTalk features.

## What can't it do (yet)?

- Any adapter not listed above (e.g. WCF Adapters).
- Envelope handling (XmlAssembler/XmlDisassembler).
- Recoverable Interchange handling (XmlDisassembler).
- Inline C#.
- Complex Orchestrations.
- Correlation/Convoys.
- EDI/Accelerators.

## When will it do more?

- This is up to you - vote on what you want it to do or contribute to enhance the capabilities!
- We need to work out what features are most important.
    - e.g. should we convert a complex orchestration, or give you the building blocks so you can do it?
- We're working on a time-frame for additional features that we will add.
- We welcome community members to be involved on adding additional features.

## What use is it to me?
- Remember: this is the initial step in the tool.
    - It's not meant to do everything to begin with.
    - We need your feedback to make it more useful.
- The tool generates a comprehensive report on your BizTalk estate.
- It deploys a lot of common services/resources that can be used to build your own integration applications (even if you're not migrating from BizTalk).

## How can I contribute?

- Provide feedback directly to us or via GitHub.
- Review the code, run the tool, raise issues on GitHub.
- Get involved in maintaining the code - if you find a bug, raise an issue or fork and create a PR.
- Talk to us about writing additional plugins or templates.

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
