// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                // example code
                //new ApiResource("dataEventRecords")
                //{
                //    ApiSecrets =
                //    {
                //        new Secret("dataEventRecordsSecret".Sha256())
                //    },
                //    Scopes =
                //    {
                //        new Scope
                //        {
                //            Name = "dataeventrecords",
                //            DisplayName = "Scope for the dataEventRecords ApiResource"
                //        }
                //    },
                //    UserClaims = { "role", "admin", "user", "dataEventRecords", "dataEventRecords.admin", "dataEventRecords.user" }
                //}
            };
        }

        public static IEnumerable<Client> GetClients(IConfigurationSection stsConfig)
        {
            // TODO use configs in app
            //var yourConfig = stsConfig["ClientUrl"];

            return new List<Client>
            {
                // example code
                //new Client
                //{
                //    ClientName = "angularclient",
                //    ClientId = "angularclient",
                //    AccessTokenType = AccessTokenType.Reference,
                //    AccessTokenLifetime = 330,// 330 seconds, default 60 minutes
                //    IdentityTokenLifetime = 30,
                //    AllowedGrantTypes = GrantTypes.Implicit,
                //    AllowAccessTokensViaBrowser = true,
                //    RedirectUris = new List<string>
                //    {
                //        "https://localhost:44311",
                //        "https://localhost:44311/silent-renew.html"

                //    },
                //    PostLogoutRedirectUris = new List<string>
                //    {
                //        "https://localhost:44311/unauthorized",
                //        "https://localhost:44311"
                //    },
                //    AllowedCorsOrigins = new List<string>
                //    {
                //        "https://localhost:44311",
                //        "http://localhost:44311"
                //    },
                //    AllowedScopes = new List<string>
                //    {
                //        "openid",
                //        "role",
                //        "profile",
                //        "email"
                //    }
                //}
            };
        }
    }
}