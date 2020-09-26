# Frequently Asked Questions:

- **Is there a complete list of everything that is/isn't supported.**
We're working on it, and this will appear soon after the public preview. You'll find it in this documentation.
- **Why no WCF support?**
 There is no WCF support in Logic Apps or Azure Functions or even APIM (although there is SOAP support). Implementing some of the WCF-WebHttp standard should be possible. However, we do have an idea of how to add support, and this is on the backlog, although we have to look at the feasibility/practicality of this. In case you're curious, it would involve implementing a custom App Service using .NET Framework. Although not publicised, you can deploy WCF services via App Services (at least whilst .NET Framework is supported – there is no WCF support in .NET Core).
 Another possible solution (for simple WCF-BasicHttp bindings) is to use APIM and the SOAP-to-REST functionality. Again, we are looking at this.