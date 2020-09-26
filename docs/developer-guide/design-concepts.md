# Design Concepts

## VISION
For the past 20 years, Microsoft BizTalk Server has provided the best supported and most widely 
adopted on-premises integration tooling.  With Azure Integration Services (AIS), Microsoft have 
created a best-in-class integration solution for customers in the Cloud and (soon) on-premises.  
Using the proposed migration tooling, BizTalk customers have the opportunity to upgrade to AIS, 
future-proofing their applications and taking advantage of modern ownership.

**Maintain confidence:** We implicitly understand that the story provided around tooling must 
engender trust in BizTalk customers: they need to feel supported at all steps of the migration 
process, through guidance and documentation.

**Ease of use:**  Our tooling is designed to be used by all IT professionals, requiring minimal 
BizTalk knowledge to get started and to migrate the most common scenarios.

**Unlocking the Integration community:**  Our approach is designed to unleash a wave of innovation 
from the integration community to solve some of the thornier integration problems.  

**Beyond BizTalk:** We see the core of our tooling stretching beyond BizTalk.  In the future the 
tooling will incorporate features to migrate from other integration stacks (such as MuleSoft).

## SOLUTION HIGHLIGHTS
We have delivered a solution that forms the core of a living project.

**Comprehensive analysis:**  Quickly allow BizTalk customers to understand their applications and migration options through listing BizTalk artefacts and their migration path.  This allows solution architects to plan their migration work to AIS.

**Core scenarios covered:** We appreciate that the problem space is broad, as developers have been innovating solutions on BizTalk for the past 2 decades.  We will deliver migration for the most common flows, such as messaging and simple orchestrations, so that the maximum number of solutions can be migrated quickly.

**Extensible framework:** Our tooling is a pluggable framework that is intended to support contributions from the community and from other vendors.  We encourage wide participation in the project: as an Integration community we can help each other support our customers to best effect.

## Pluggable Architecture

The aim with the migration toolset is to support a pluggable architecture that allows each stage of conversion to be represented by different components.  This is configurable and allows the toolset to be extended easily with new components, which can support the conversion of different features in BizTalk to the relevant capabilities in AIS.  Additionally, where AIS differs in capability from the cloud to the on-premises version, this is taken into account too and is supported via an option that allows the user to target the relevant platform and version for them: the cloud or on-premises.

Whilst this RFQ response focuses on the conversion of BizTalk assets to AIS, a benefit of a pluggable architecture also provides the potential to build components to convert integration solutions implemented on competing platforms, such as Mulesoft or Dell Boomi, to AIS.  It also provides the ability to move customers from other legacy Microsoft technologies such as Workflow Foundation and AppFabric to AIS.

The key to supporting this many-to-one conversion capability is to internally represent the result of the analysis phase of source assets as a canonical object model, for easier conversion to AIS artefacts.

