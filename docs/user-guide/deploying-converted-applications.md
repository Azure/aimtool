# Deploying Converted Applications

The BizTalk Migrator outputs ARM templates and deployment scripts so you can deploy your application in Azure.

## Intended Use of the Converted Applications

The BizTalk Migrator will convert as much of your applications as possible. This is not
guaranteed to be perfect - we get you as far along the conversion track as possible.
This means your migration will be faster, cheaper and more reliable than if you do it unaided.

We expect that you will need to modify some of the files after conversion.  With this in mind, 
we expect you to load the output into your own source control repositories and work with it from there.

## ARM Templates

The tool will generate ARM templates for Logic Apps and some other Azure resources.  These are
application source code and you can manage them as such.

## Deployment Scripts

The tool also outputs deployment scripts for the ARM Templates, and there are other scripts that
will deploy resources via the Azure CLI.

These deployment scripts will form part of your DevOps process.  You can leave them as they are, 
combine them or modify them to suit the tooling and release processes appropriate for your organisation.

