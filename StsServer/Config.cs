// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace StsServerIdentity
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("my_identity_scope",new []{ "role", "admin", "user" } )
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("scope_used_for_hybrid_flow")
                {
                    ApiSecrets =
                    {
                        new Secret("hybrid_flow_secret".Sha256())
                    },
                    UserClaims = { "role", "admin", "user" }
                },
                new ApiResource("ProtectedApi")
                {
                    DisplayName = "API protected",
                    ApiSecrets =
                    {
                        new Secret("api_in_protected_zone_secret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "scope_used_for_api_in_protected_zone",
                            ShowInDiscoveryDocument = false
                        }
                    },
                    UserClaims = { "role", "admin", "user" }
                }
            };
        }

        public static IEnumerable<Client> GetClients(IConfigurationSection authConfigurations)
        {
            var hybridClientUrl = authConfigurations["HybridClientUrl"];
            var codeFlowClientUrl = authConfigurations["CodeFlowClientUrl"];

            return new List<Client>
            {
                new Client
                {
                    ClientName = "hybridclient",
                    ClientId = "hybridclient",
                    ClientSecrets = {new Secret("hybrid_flow_secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowOfflineAccess = true,
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris = {
                        "https://localhost:44329/signin-oidc",
                        $"{hybridClientUrl}/signin-oidc"
                    },
                    PostLogoutRedirectUris = {
                        "https://localhost:44329/signout-callback-oidc",
                        $"{hybridClientUrl}/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "scope_used_for_hybrid_flow",
                        "role"
                    }
                },
                new Client
                {
                    ClientId = "CC_FOR_API",
                    ClientName = "CC_FOR_API",
                    ClientSecrets = new List<Secret> { new Secret { Value = "cc_for_api_secret".Sha256() } },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new List<string> { "scope_used_for_api_in_protected_zone" }
                },
                new Client
                {
                    ClientId = "deviceFlowWebClient",
                    ClientName = "Device Flow Client",

                    AllowedGrantTypes = GrantTypes.DeviceFlow,
                    RequireClientSecret = false,

                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email
                    }
                },
                new Client
                {
                    ClientName = "codeflowpkceclient",
                    ClientId = "codeflowpkceclient",
                    ClientSecrets = {new Secret("codeflow_pkce_client_secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = true,
                    AllowOfflineAccess = true,
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    //AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris = {
                        "https://localhost:44330/signin-oidc",
                        $"{codeFlowClientUrl}/signin-oidc"
                    },
                    PostLogoutRedirectUris = {
                        "https://localhost:44330/signout-callback-oidc",
                        $"{codeFlowClientUrl}/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "role"
                    }
                },
                new Client
                {
                    ClientName = "AspNetCoreRequireMfaOidc",
                    ClientId = "AspNetCoreRequireMfaOidc",
                    ClientSecrets = {new Secret("AspNetCoreRequireMfaOidcSecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowOfflineAccess = true,
                    AlwaysSendClientClaims = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris = {
                        "https://localhost:44389/signin-oidc"
                    },
                    PostLogoutRedirectUris = {
                        "https://localhost:44389/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "role"
                    }
                }
            };
        }
    }
}