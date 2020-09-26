# Introduction

## What is the BizTalk Migrator?

The BizTalk Migrator is a command-line interface (CLI) for migrating BizTalk assets to Azure Integration Services.  
It uses a plug-in architecture, allowing new functionality/features to be added, so you can extend the tool to cover your own migration scenarios.
This is an open-source project run by Microsoft, delivered for the benefit of BizTalk customers.

## Problem space - BizTalk Migration

BizTalk to Azure migration is hard, because there is no direct equivalence between BizTalk artefacts and the equivalent in Azure.
BizTalk forces developers to adopt certain patterns, with the BizTalk platform providing the framework to run the code generated.
In Azure there is much more choice, and much more design freedom, yet some things that are available out-of-the-box with BizTalk
require developer effort to support in Azure.

BizTalk is a large product - a developer platform in its own right - and it has been around for 20 years.
Over those 20 years, developers have found myriad ways to use BizTalk, not all of these are supported in Azure.
This is especally the case with orchestrations, where developers have been able to create their own patterns and
plug in custom code.

### What the BizTalk Migrator aims to do

This tool is a first step – to begin with, it will only fully convert a limited number of scenarios.
However, the tool is designed for extensibility.  Combine this with an open source community and you can see that the
number of supported scenarios will grow over time.

The tool will convert as much as it can, so even when your scenario isn't fully converted you should still get some value from it.
If it doesn’t do what you want, then get involved – vote for additional features, or help write additional plugins.

This is a complex tool, as it’s a complex problem to solve – don’t expect it to work miracles!

### Limits on what the BizTalk Migrator will achieve

Owing to the differences between BizTalk and Azure, and the endless innovation of developers on 
the BizTalk platform, it’s cost prohibitive to automatically convert everything.  

The BizTalk Migrator will not convert complex orchestration scenarios and patterns, although it will 
attempt to convert what is can.  It will not convert your custom pipline components, nor will it 
handle inline C# in expression shapes.
