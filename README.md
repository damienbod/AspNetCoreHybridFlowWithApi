
[![.NET](https://github.com/damienbod/AspNetCoreHybridFlowWithApi/workflows/.NET/badge.svg)](https://github.com/damienbod/AspNetCoreHybridFlowWithApi/actions?query=workflow%3A.NET) 


## Blogs: 

<ul>
	<li><a href="https://damienbod.com/2018/02/02/securing-an-asp-net-core-mvc-application-which-uses-a-secure-api/">Securing an ASP.NET Core MVC application which uses a secure API</a></li>
	<li><a href="https://damienbod.com/2019/05/10/handling-access-tokens-for-private-apis-in-asp-net-core/">Handling Access Tokens for private APIs in ASP.NET Core</a></li>
	<li><a href="https://damienbod.com/2018/02/08/adding-http-headers-to-improve-security-in-an-asp-net-mvc-core-application/">Adding HTTP Headers to improve Security in an ASP.NET MVC Core application</a></li>
	<li><a href="https://damienbod.com/2019/02/20/asp-net-core-oauth-device-flow-client-with-identityserver4/">ASP.NET Core OAuth Device Flow Client with IdentityServer4</a></li>
	<li><a href="https://damienbod.com/2019/10/11/securing-an-asp-net-core-razor-page-app-using-openid-connect-code-flow-with-pkce/">Securing an ASP.NET Core Razor Page App using OpenID Connect Code flow with PKCE</a></li>
	<li><a href="https://damienbod.com/2019/12/16/force-asp-net-core-openid-connect-client-to-require-mfa/">Force ASP.NET Core OpenID Connect client to require MFA</a></li>
	<li><a href="https://damienbod.com/2019/12/18/send-mfa-signin-requirement-to-openid-connect-server-using-asp-net-core-identity-and-identityserver4/">Send MFA signin requirement to OpenID Connect server using ASP.NET Core Identity and IdentityServer4</a></li>
	<li><a href="https://damienbod.com/2020/01/03/requiring-mfa-for-admin-pages-in-an-asp-net-core-identity-application/">Requiring MFA for Admin Pages in an ASP.NET Core Identity application</a></li>
	<li><a href="https://damienbod.com/2021/02/19/require-user-password-verification-with-asp-net-core-identity-to-access-razor-page/">Require user password verification with ASP.NET Core Identity to access Razor Page</a></li>

</ul>

## Database migrations

```
Add-Migration InitialCreate -c ApplicationDbContext
```

```
Update-Database
```


## History

- 2022-01-28 Updated packages

- 2021-11-08 Update .NET 6 release

- 2021-11-07 Update .NET 6 

- 2021-11-06 Update .NET 5

- 2021-08-19 improved security headers

- 2021-08-18 Updated packages, improved security headers STS

- 2021-05-28 Updated packages, added example for IClaimsTransformation

- 2021-05-15 Updated packages, fix identity email bug

- 2021-04-17 Updated nuget packages, improving API calls

- 2021-03-17 Updated nuget packages

- 2021-03-05 Updated nuget packages

- 2021-02-25 Updated nuget packages, small clean up

- 2021-02-17 Updated nuget packages 

- 2021-01-19 Switching to Azure.Extensions.AspNetCore.Configuration.Secrets

- 2021-01-17 Updated nuget packages .NET 5.0.2

- 2020-12-11 Updated to .NET 5

- 2020-11-08 Added swagger to the API, moved to Azure.Security.KeyVault.Secrets

- 2020-11-06 Updated nuget packages, npm packages

- 2020-08-23 Updated nuget packages

- 2020-07-03 Update IdentityServer4 to V4, Updated nuget packages, update npm packages

- 2020-05-03 Updated nuget packages

- 2020-03-02 Support FIDO2 and updated nuget packages

- 2020-01-03 Added ASP.NET Core Identity App with MFA force

- 2019-12-18 Added STS acr_values parameters logic

- 2019-12-14 Added Require MFA client

- 2019-12-13 Updated to .NET Core 3.1

- 2019-10-11 Added example of Code Flow with PKCE for ASP.NET Core Razor Page App

- 2019-10-06 Updated to .NET Core 3.0

- 2019-05-10 Improving token handling

- 2019-04-30 Switch to in-process, add token expired check, Updating nuget packages, updating npm packages

- 2019-02-24 Updating obsolete API call code, updating npm packages

- 2019-02-20 Updating STS, added the OAuth Device Flow

- 2018-11-11 Updating Nuget packages, added feauture-policy

- 2018-11-10 Updated to .NET Core 2.2

- 2018-08-03 Updated to .NET Core 2.1.2

- 2018-05-08 Updated to .NET Core 2.1 rc1

- 2018-05-07 Updated to .NET Core 2.1 preview 2, new Identity Views, 2FA Authenticator, IHttpClientFactory, bootstrap 4.1.0


## Links

https://github.com/aspnet/Docs/tree/master/aspnetcore/security/authentication/cookie/samples/2.x/CookieSample

https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.2

https://leastprivilege.com/2019/02/08/try-device-flow-with-identityserver4/

https://tools.ietf.org/wg/oauth/draft-ietf-oauth-device-flow/

https://github.com/leastprivilege/AspNetCoreSecuritySamples/tree/aspnetcore21/DeviceFlow

https://hajekj.net/2017/03/06/forcing-reauthentication-with-azure-ad/

https://tools.ietf.org/html/draft-ietf-oauth-amr-values-04

https://openid.net/specs/openid-connect-core-1_0.html
