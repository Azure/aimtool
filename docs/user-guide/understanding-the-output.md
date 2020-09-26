# Understanding the Output

Depending on the options you have selected, the BizTalk Migrator will generate the following output on your local file system:

- [Unpack](#unpack) - this contains the unpacked BizTalk MSIs.
- [Report](#report) - a series of HTML files reporting on the migration.
- [State](#state) - the underying state model of the tool that describes the source and target applications as a JSON object.
- Output - the scripts and templates that will create your target applications in Azure.

## Unpack

The BizTalk Migrator opens up your BizTalk MSI files and extracts the contents to the local file system.
The MSIs in turn contain CAB files that are also unzipped.  Browsing through the unpack folder
allows you to view the BizTalk artefacts including:

- Application Definition File
- Binding Info file
- BizTalk assemblies
- Other .Net assemblies
- Web applications

This is for information:  you don't need to do anything with these files, and they can be 
deleted after you have run the tool.

## Report

The BizTalk Migrator generates a report in HTML format.  You will find the following report files
in the folder where the report is generated:

- Summary page
- Source application reports - one per BizTalk application analyzed
- Target application report - one per target application (each corresponding to a BizTalk application)
- System application report - contains common functionality comparable to the BizTalk system app
- Message Bus application report - core functionality replacing elements of the BizTalk infrsatructure

### Viewing the Report

Here is an example of the summary report:

![Sample Report](../images/sample-report.png)

## State

If you have run the tool with the *Save State* option you will find the tool state saved in a local folder.
The tool is able to save state before and after each stage of processing (Discover, Parse, Analyze etc).
This allows you to look at the underlying data model representing your BizTalk applications.

The state contains the following elements:

- Execution state - this tracks the execution of the Stage Runners (plugin components) that are invoked during a run.
- Configuration - the data passed to the tool at the start of execution.
- Model - the data representing the application(s).
    - Source Model - this represents your BizTalk applications.
    - Target Model - this represents your applications as they would appear in Azure.

You don't need to use the state unless you are re-running the conversion to Azure artefacts.
However, you may need the state if you are going to request assistance.

## Output

The output is the generated code, templates and scripts that represent your migrated application.

The output will be comprised of the following:

- PowerShell scripts - these are to deploy to Azure
- ARM templates - these represent Azure resources, including Logic Apps
- JSON files - these contain configuration to be used by the templates during deployment
- XSLT transforms - these are generated from BizTalk maps
- XSD schemas - these are extracted from BizTalk assemblies
