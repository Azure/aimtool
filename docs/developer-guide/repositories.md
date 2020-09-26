# Repositories

The BizTalk Migrator is built from a number of repositories in GitHub. Each is prefixed with "aim",
which is an abbreviation of "Azure Integration Migration".

Here is a list of the repositories and an explanation of the purpose for each:

## aimtool

This repo contains the CLI tool and the implementation of the plug-in architecture. 
It converts command line options into the relevant commands needed to build the runner 
and execute it.  If we were to create a GUI instead of a CLI, it would do the same thing 
fundamentally, that is, translate user input into runner build and execution, using a 
plug-in approach to discovering stage components.

## aimcore

This repo contains the core runner that runs the stage components.  It sorts and orders by 
priority the stage components, then executes them.  It also acts as a "shared assembly" 
containing common interfaces used by the plug-in architecture.

## aimmodel

This repo contains the source and target model entities.  It also contains the template 
configuration and Liquid rendering functionality that populates the target model with the 
AIS templates and assets discovered on disk.

## aimbiztalk

This repo contains all of the stage components to discover, parse, analyze, report and convert 
BizTalk applications.  It also contains the template configuration to map target model 
artifacts created by the BizTalk Analyzers to AIS template assets.  If we were to tackle 
an alternative EAI vendor product, we would create a separate repo for that product and build similar stage components 
to discover and parse that product's artifacts into the source model and then map those artifacts 
over as part of the analysis to the target model, which is then used to generate AIS templates.

## aimazure

This repo contains all of the AIS related artifacts, such as Azure Function projects, ARM 
templates, snippets for building Logic Apps and so on.  Some of these templates are in Liquid 
format so they can be transformed as part of the convert stage.
