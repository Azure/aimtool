# Contributing to the Project

The BizTalk Migrator has been envisaged, right from the start, to be a project driven by the Integration
community.  All of your contributions are valued, and here are some of the ways you can contribute:

## Discussion

We are aiming to build an online hub to facilitate discussion and feedback on the BizTalk Migrator. 
This is where you'll go to ask for advice, discuss any problems you're having and share solutions you
have discovered.  Please check back - we will provide a link here when available.

## GitHub issues

When you are migrating BizTalk applications you will encounter limitations in how the tool is able to
perform.  This will broadly fall into two categories:

- Bugs - errors encountered when running the tool or deploying the output.
- Features - BizTalk features that are not supported for conversion.

Please check to see if anyone has already raised these as issues in GitHub.  If not - feel free to raise your
own issues.  

Please avoid raising issues for advice or discussion.

## Test scenarios

The BizTalk Migrator ships with a limited number of test scenarios including some basic messaging (Passthough, XML)
and simple orchestration. 

If you identify other scenarios you feel would be good test cases please feel free to code these
and create a pull request into the codebase.

## Exemplar applications

Each of the above test scenarios has a companion Azure application.  This allows developers to 
see what the target application will look like. 

These exemplar applications are also used as the basis for creating templates

## Templates

The target (Azure) applications are built from Liquid templates that will usually generate either
an ARM template, a PowerShell script, a JSON file or a YAML file.

Supporting additional features in BizTalk is largely dependent on creating the templates to render these
resources.  

For example, we currently support conversion of the BizTalk FILE adapter by rendering the 
corresponding Logic Apps trigger.  For the BizTalk Migrator to support a different BizTalk adapter, e.g. POP3, 
the main work involved would be to create a new Logic App representing the trigger plus updates
to the config and Liquid templates.

## Stage Runners

The stage runners are the code that makes the BizTalk Migrator work.  Features and fixes that require
changes to the logic will require changes to the stage runner code.

To work on stage runners you will need to fork the repos and get the code running locally.  You can then make 
your changes and create a pull request to submit your code into the main repo.

