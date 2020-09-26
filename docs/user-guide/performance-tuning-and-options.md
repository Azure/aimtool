# Performance, Tuning and Options

## Summary

The BizTalk Migrator generates resources in Azure that have a different performance profile from 
the BizTalk application you are converting.  

Since Azure Integration Services are serverless, you have the advantage of scale and parallel execution
built into the platform.  Unlike BizTalk, performance will not be affected by scale because the Azure
platform will scale resources according to demand.

Against this, you also have resources that are loosely coupled, which will usually be communicating with each other
via HTTP.  

## Design intent - functionality over performance

We're aware that latency in the generated resources isn't as good as we'd like it to be.

If you have tuned your BizTalk setup for low-latency, then you'll notice that your BizTalk application is slower in AIS once migrated.

For this initial release, we focussed on functionality, without considering latency or per-action cost.
 For example, we implemented pipelines as a series of disparate Logic Apps, linked by a Routing Slip: this allows us to build pipelines without any development work, and to change the components in a pipeline at runtime with relative ease. But this (currently) comes at the cost of increased latency.

Unlike with BizTalk, latency won't increase as throughput increases - it will remain relatively stable.

The question to ask is: is the latency acceptable in your scenario? Unless you have a need for sub-second latency, you may find that it is.

## Causes of high latency

There are a number of reasons for the increased latency:

- Cold-start: The core solution uses a number of Azure Functions and Integration Account Connectors, plus Azure App Service. All of these have a cold-start penalty (i.e. initial compilation/interpretation + resource host creation) which can add up to over a minute of latency. There's not a lot that can be done about this, and this same issue affects most Integration scenarios in Azure today. This latency cost is inly occurred don the first request after a period of inactivity. There are a number of possible solutions:
  - For Azure Functions, use a Premium SKU or a non-consumption App Service Plan – this allows the user of always-on functionality, meaning no cold-start – see here for more info: [https://microsoft.github.io/AzureTipsAndTricks/blog/tip260.html](https://microsoft.github.io/AzureTipsAndTricks/blog/tip260.html)
  - Implement a scheduled task (Logic app or Function) that executes all the resources that suffer from cold-start (Azure Functions, Azure App Config, and certain Integration Account Connectors at this time)
- State: Current consumption-based Logic Apps rely on State, and make multiple calls to Azure Storage to achieve this. This adds to the latency of a single Logic App execution. Stateless Logic Apps (part of the new _Logic Apps on Functions_ engine) will address some of this.
- HTTP Request/Response Latency: we're working on this, but there is a certain amount of latency in a simple HTTP Request/Response scenario today, even without adding any extra actions to a Logic App. When you have a chain of 7-8 Logic Apps all calling each other, this can add up
- Polling: We use Service Bus to mimic the BizTalk MessageBox and pub-sub. The Service Bus connectors are polling connectors: once a message appears on a topic and is available to a subscription, there is latency added due to the polling period. The best solution is to move to an event driven model – either using Event Grid, or Azure Functions with Service Bus Triggers, or for us to move away from Polling for the Service Bus connectors. We're looking into this internally

If you have concerns about latency, please let us know. We have a number of competing priorities for this tooling, and we need you (the community) to help us decide what to focus on.

## Performance vs cost

As tool developers, we cannot dictate to you the BizTalk Customer what trade-off you need between cost and performance.

For example, many Logic App triggers utilise a polling mechanism to check for new work. 
If you have a consumption plan you are charged per action - the shorter the polling interval the
better the performance - but the higher the cost.  You will need to decide for yourself what the best balance is.

## Azure SKUs

A similar problem arises with Azure SKUs.  There are trade-offs between cost and performance in the following areas:

- Integration Accounts
- Azure Service Bus
- Azure API Management
- Azure Functions

## Loosely vs tightly coupled applications

The BizTalk migrator works by creating reusable components that would otherwise be provided for you by BizTalk.
These include things such as XML validation, XSLT transform and property promotion.

These building blocks are generated for you as Logic Apps and Azure Functions.  In order to make these reusable
they are connected together via additional components that implement the Routing Slip Router integratio pattern.

The result of this is that there are many hand-offs between components through the execution of your application.
You have a choice as to whether to keep the loosely coupled design, or start to combine multiple steps into
the same Logic App.  You may gain on performance, but you may lose on maintainability.

## Effect of cold start

Your application will be hosted on serverless technology.  This means that the Azure platform
handles the allocation of resources, and these resources may be withdrawn if your application
is idle for a length of time.

Many of the components in your application will experience "cold start" behaviour

## Tuning for performance

We recommend you execute your applications and measure the performance of your applications by subjecting them to 
a consistent performance test regime.

Measure the time to execute of the performance and look for bottlenecks.  

When you find a bottleneck in performance you should then try to understand the cause and what you can do to improve performance.

Some of the options are:

- Reduce the polling interval of your triggers
- Upgrade the SKU of your Integration Account 
- Upgrade the SKU for API Management
- Upgrade the SKU for Azure Functions
- Modify your Logic Apps to combine actions
