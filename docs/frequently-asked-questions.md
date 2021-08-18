# Frequently Asked Questions:

- **Is there a complete list of everything that is/isn't supported.**  
We're working on it, and this will appear soon after the public preview. You'll find it in this documentation.  
  
- **Why no WCF support?**  
 There is no WCF support in *Logic Apps* or *Azure Functions* or even *API Management* (although there is SOAP support). Implementing some of the *WCF-WebHttp* standard should be possible. However, we do have an idea of how to add support, and this is on the backlog, although we have to look at the feasibility/practicality of this. In case you're curious, it would involve implementing a custom App Service using the .NET Framework. Although not publicized, you can deploy WCF services via App Services (at least whilst .NET Framework is supported - there is no WCF support in .NET Core).
 Another possible solution (for simple WCF-BasicHttp bindings) is to use APIM and the SOAP-to-REST functionality. Again, we are looking at this.  
   
- **How do I use an existing Azure App Config or Integration Account resource?**  
 If you wish to use an existing *Azure App Configuration* or *Integration Account* then you have two choices:  

 1. Edit the JSON state file (using the --save-state option - you would run the assess stage only, edit the JSON state file to change the name of the resources, and then run the convert stage only, specifying the name of the state file to use).
 2. Edit the ARM template parameter files. These files are generated as part of the convert stage, and are located in the *conversion* folder in your output location.  
 Specifically these are the files to edit:
     -    `\conversion\messagebus\configmanager\configmanager.apim.dev.parameters.json`  
          Change the *configurationManagerAppConfigName* and *configurationManagerResourceGroupName* values  
     -    `\conversion\messagebus\routingstore\routingstore.apim.dev.parameters.json`  
          Change the *routingStoreAppConfigName* and *routingStoreResourceGroupName* values  
     -    `\conversion\applications\*\config\configurationentries\*\configurationentry.appcfg.dev.psparameters.json`  
          Change the *configStoreName* value  
     -    `\conversion\applications\*\config\routingproperties\*\routingproperties.appcfg.dev.psparameters.json`  
          Change the *configStoreName* value  
     -    `\conversion\applications\*\config\routingslips\*\routingslip.appcfg.dev.psparameters.json`  
          Change the *configStoreName* value  
  
- **How do I use an existing Azure API Management resource?**  
  At the moment, you'd need to work this out yourself, although we will provide a guide for this if enough people ask.
  Given the tool outputs a series of ARM templates, you're able to edit these to use whichever named resource you want.
  An easy way to do this would be to perform a global search-and-replace in all .json files, looking for the name of the APIM instance (`apim-aimmsgbussvc-dev-<dep>`) and replacing it with your APIM instance name. It's a bit more complicated than this, as chances are your APIM instance would be in a different Resource Group.
    
- **What's the Unique Deployment ID for?**  
  Certain resources in Azure need to have a name that is unique across all resources of that type globally - this is usually because they form part of a URL. e.g. a *Storage Account Name*, or a *Service Bus Namespace*. To ensure your resources won't conflict with the names used by any other user's resources, we append a unique id to each resource name. You can either supply this when you run the tool, or not supply it (and the tool will generate a random 5-char value for you).
    
- **What's with the naming convention?**  
  We use the *Azure Cloud Adoption Framework*'s (CAF) set of standards and best practices for naming - see more here: https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging.  
  Where we use a resource that isn't mentioned, we assume a standardized name e.g. "apic" for *API Connections*.
    
- **What's up with the performance of the converted application?**  
  For this initial release, we focussed on capability and feature completeness, without considering performance. We're aware that performance might be sub-optimal for certain scenarios (e.g. HTTP Request-Response). We're working on tuning the generated output application components to improve performance. However, we're also planning improvements to the Logic Apps runtime engine which will help with this e.g. Stateless Logic apps. In the meantime, give us your feedback, and we'll see what can be done.  
    
  One important thing to remember is that the performance of a converted app will behave differently to BizTalk as load increases: the end-to-end latency should not change much as load increases, in direct contrast to BizTalk. For example, a simple HTTP request-response in BizTalk might take 5 seconds to execute, and 30 seconds in the converted application. But scale that up to 100 (or 1000) concurrent requests - BizTalk will slow down and you'll start getting timeouts (depending on your BizTalk server setup), whereas the converted application should still have the same latency it had when there was only a single request.
  
  The question to ask is: what latency do I need? And then: will my converted application meet this? There are multiple ways to make the converted applications more efficient and we'll be looking at those along with the community.  
  
- **What are all the extra resources the tool generates e.g. MessageBus?**  
  When the tool runs, regardless of how many MSIs it processes, it generates 3 groups of resource:
   - MessageBus: this is the core set of resources used by all converted applications e.g. Azure API Management; Azure App Configuration, Azure Functions, KeyVault, Storage etc. Every deployment that shares the same *Unique Deployment Id* will have a single shared MessageBus. The MessageBus resources are deployed in a single Resource Group called `rg-aimmsgbus-dev-<region>-<dep>` (where &lt;dep&gt; is your *Unique Deployment Id*). If you run the tool once with 5 MSIs, you'll end up with a single MessageBus Resource Group. If you run the tool 5 times with one MSI each, and use the same *Unique Deployment Id*, you'll end up with a single MessageBus Resource Group.
   - SystemApplication: this mimics the BizTalk.System application. It contains the MessageBox service bus topic, the service bus subscriptions, and common Logic Apps used by all deployed applications. As with the MessageBus, you'll only have a single SystemApplication, and all the resources for it are contained in a single Resource Group called `rg-aimapp-systemapplication-dev-<region>-<dep>` (where &lt;dep&gt; is your *Unique Deployment Id*).
   - Applications: Each MSI will result in a separate Application being created. Each application will be in it's own resource group e.g. `rg-aimapp-aim-httpjsonorch-dev-<region>-<dep>`. The name will be based on the name of the BizTalk Application contained in the MSI. If the BizTalk Application contained no resources that could be migrated, the Resource Group will be empty. Otherwise it might contain *Process Manager* Logic Apps (one per orchestration); *Receive Adapter* Logic Apps (the starting point for a Receive Port); *Topic Subscriber* Logic Apps (the starting point for a Send Port), and configuration: *Config Entries* (contain the configuration for each Logic App); *Routing Properties* (list of properties to promote out of or into a message); and *Routing Slips* (the path the message should take for a Scenario)

- **What's the JSON 'envelope' message I can see being passed around?**  
  Every message received via a *Receive Adapter* Logic App is wrapped in an Envelope: this is not the same XML Envelope used by the XMlDisassembler/Assembler components, but rather a JSON object that wraps the received message (regardless of content type) and adds support for promoted properties, and message properties. It's similar in concept to the BizTalk *XLANGMessage* type, and provides a wrapper around an arbitrary message. It provides support for multiple body parts (although only one part can be the root body part), and allows for a header that contains promoted properties (in the *routing* section) along with general message properties.

- **How do I add the converted application to Visual Studio/Visual Studio Code/DevOps etc.?**  
  When the tool runs, it outputs a series of ARM templates, ARM Template Parameter files, and deployment scripts.
  You can open the `conversion` folder directly in Visual Studio Code and look through the templates.
  For Visual Studio or DevOps, you'd need to copy out the templates and put them in a structure of your own devising - there are many ways that people store their templates, so a decision was made early on to supply just the ARM templates and parameter files (and a simple way to deploy them), and then it is up to you to decide how you store those in source control or deployment pipelines.
  
- **How do I edit a Configuration Entry/Routing Property/Routing Slip?**  
  You may have noticed that if you go into *Azure App Configuration* and edit one of the entries, it doesn't seem to have taken effect. This is because we cache the entries in API Management. In a later release we'll add a Logic App that uses an Event Grid trigger to refresh the cache when a change is detected, but for now the best way to do it is use the APIM Developer Portal, select the correct api e.g. *ConfigurationManager/GetConfigurationEntryForStep* and call it, passing it the Scenario and StepName you edited.

- **What's a Scenario?**  
  A *Scenario* is the name we use for a discrete set of operations. For example, a Receive Location that is linked to a receive Port that has maps is a scenario: we'll receive a message, run a pipeline, transform it, and then publish it. A Scenario either starts with an endpoint (e.g. File Receive Adapter) and ends with publishing to the messagebox; or it starts with a subscription, and publishes to the messagebox (e.g. an Orchestration); or it starts with a subscription and ends with an endpoint (e.g. File Send Adapter). A Scenario is used as the key to look up configuration entries, routing properties and routing slips in the configuration manager.

- **How do a I debug my converted application?**  
  We're working on some documentation to help with this, along with some App Insight improvements to help you track the flow of messages. At the moment you have to look at the run history for each Logic App in the route. We appreciate that this isn't ideal.  
  
- **A LogicApp failed, why does it show as successful?**  
  We use ACKs and NACKs to track success/failure states (as does BizTalk). Only the initiating Logic App will show a Success or Failure state e.g. the *File Receive Adapter* for a Receive port; and a *Topic Subscriber* for a Send Port. All other Logic Apps are intermediaries, and oif they have either successfully generated an ACK/NACK or successfully passed one to the caller, then they will be marked as a successful run.  
  
- **Do you support the new Standard Logic Apps runtime?**  
  Yes we do, check the [Change Log](..\CHANGELOG.md) for more information.
    
- **Can I run a converted application locally using the new runtime?**  
  Possibly yes, although we haven't tested it. The biggest issue is lack of *Azure App Configuration* support in a local environment. Everything else (Logic Apps, Functions, API Management) has a local equivalent. We're looking at what the best options are for this.
