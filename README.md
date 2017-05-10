# XrmLazyLoading
Wrappers with lazy loading supports and code generation for Microsoft Xrm Sdk without using deprecated `Microsoft.Xrm.Client`.

To use it, build the solution, copy `XrmLazyLoading.CodeGeneration.dll` and `XrmLazyLoading.dll` to where your `CrmSvcUtil.exe` is located. Then run the `CrmSvcUtil.exe` as usual but with the `codeCustomization` parameter like this:

`CrmSvcUtil.exe /codeCustomization:"XrmLazyLoading.CodeGeneration.CodeCustomization,XrmLazyLoading.CodeGeneration" ...`

The generated class would be almost identical to the one without the codeCustomization, except the base class for entities is now `XrmLazyLoading.EntityWrapper` instead of `Microsoft.Xrm.Sdk.Entity` and the base class for service context is `XrmLazyLoading.OrganizationServiceContextWrapper` instead of `Microsoft.Xrm.Sdk.Client.OrganizationServiceContext`. The wrapper classes supports the same [lazy loading as the old `CrmOrganizationServiceContext` and `CrmEntity` classes from `Microsoft.Xrm.Client.dll`][1] but much lighter weighted.

[1]: https://msdn.microsoft.com/en-us/library/gg695791(v=crm.7).aspx
