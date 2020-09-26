# Known Issues and Workarounds

## Maximum string lengths for names

Azure resources have maximum name lengths, which can cause issues when deploying migrated code.  See
the following article for more information on maximum lengths: [Click here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/resource-name-rules)

Schema names have a maximum length of 80 characters.  BizTalk applications can breach this limit, 
especially when they have been auto-generated.

Azure resource names are generated from a number of sub-components such as the region name, the 
resource type, the name of the artefact and the unique deployment ID.  When concatenated together these
can also result in generated templates that fail to deploy in Azure.  (Region names alone can be up to 18 characters).

Once the BizTalk Migrator has generated the output you can make edits on your local file system. 
For example, if the local region name is causing an issue you can perform a search & replace with an abbreviated 
region name (e.g. "UK South" vs "uksouth" vs "uks").

The BizTalk Migrator is not opinionated about how you should abbreviate these names, that's up to you.

**Workaround:** Manually edit the names in the deployment templates so they they are within
the maximum length limits.

## Duplicate references when multiple applications contain the same .Net types or port names

The BizTalk migrator finds references between maps and schemas from .Net types.  Similarly, 
references from send port groups (distribution lists) to send ports is done on name.  
Likewise with orchestration port bindings.

When you process multiple BizTalk applications together, and these applications contain the same 
.Net types or port names there is no way to distinguish between the duplicates across applications
when resolving dependencies.

As such, the migration report may show duplicate references for the source applications.

**Workaround:** Run the MSIs through the BizTalk Migrator separately.

## Limits in Logic Apps that may cause deployment issues

Azure Logic Apps have limits that a converted orchestration may exceed.  Examples are:

- Actions per workflow
- Nesting depth for actions
- Variables per workflow

For example, an orchestration can contain many nested Group, Decision, Scope and Delay shapes.  These
all generate nested actions within the generated Logic App.  A nesting depth greater than 8 will
cause a deployment failure in the Logic App.

For a full list of the limits please refer to the following article: [Click here](https://docs.microsoft.com/en-us/azure/logic-apps/logic-apps-limits-and-config)

**Workaround:** Refactor the generated Logic App into smaller units until the limits are no longer exceeded.

## Existing resources/SKU levels and deployment errors

We had to make a number of decisions about the environment into which the migrated resources would be deployed, and how much it would cost.

For this reason, we made the following assumptions:

- We use the cheapest SKU for any given resource – this means that for Azure App Configuration, and Integration Accounts, we use the Free SKU
- For Azure API Management, we use the Developer SKU
- We assume that you are deploying the resources into a clean subscription, with no other resources that could conflict

This means that you could find some conflicts/errors during deployment if any of the following are true:

- You have an existing _Azure App Configuration_ instance in the same subscription, using the Free SKU: you are only allowed one of these per subscription
- You have an existing _Integration Account_ in the same region in the same subscription, using the Free SKU: you are only allowed one of these per region.
- You have an existing resource with the same name in the same scope e.g. a storage account with the same name

The deployment scripts created by the tool are designed to handle simple scenarios, but we assume that you'll need to modify them for your own particular requirements.

We'll update this documentation with instructions on how to handle the most common scenarios e.g. how do I use my own _Azure App Configuration_ instance or my own existing _Integration Account_ instance.

## Unsupported scenarios

If a BizTalk orchestration contains a scenario that is currently not supported by the BizTalk Migrator then an error 
will be displayed during migration similar to the following example:

```
[12:21:39 ERR] Unable to find the resource with the type microsoft.workflows.azurelogicapp and scenario step name of xmlMessageFilter in the target model.
```

In this case the output will be missing some functionality as templates are not yet available
to cover the feature.

**Workaround:** You will need to manually edit the converted application to include the 
missing functionality.