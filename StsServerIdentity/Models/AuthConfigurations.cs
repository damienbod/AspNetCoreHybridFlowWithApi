﻿namespace IdentityProvider.Models;

public class AuthConfigurations
{
    public string IdentityProviderUrl { get; set; }
    public string WebHybridClientUrl { get; set; }
    public string WebCodeFlowPkceClientUrl { get; set; }
    public string AspNetCoreRequireMfaOidcUrl { get; set; }
}
