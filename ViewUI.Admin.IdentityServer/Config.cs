using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ViewUI.Admin.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource{ Name = "location", DisplayName="location", Emphasize=true, ShowInDiscoveryDocument= true,UserClaims = new Collection<string>(new List<string>{"location"}) },
                new IdentityResource{ Name = "power", DisplayName="power", Emphasize=true, ShowInDiscoveryDocument= true,UserClaims = new Collection<string>(new List<string>{"power"}) }
            };
        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // machine to machine client
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },

                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =           { "http://localhost:5003/signin_redirect", "http://localhost:5003/silent_signin" },
                    PostLogoutRedirectUris = { "http://localhost:5003/login" },
                    AllowedCorsOrigins =     { "http://localhost:5003" },


                    AccessTokenLifetime = 10,
                    AllowOfflineAccess=true,
                    RequireConsent=false,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "location",
                        "power",
                        "api1"
                    }
                },

                // Flutter Client
                new Client
                {
                    ClientId = "native.code",
                    ClientName = "Flutter Client",
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =           { "io.identityserver.demo:/oauthredirect" },
                    //AllowedCorsOrigins =     { "https://notused" },

                    AccessTokenLifetime = 30,
                    AllowOfflineAccess=true,
                    RequireConsent=false,
                    RefreshTokenUsage = TokenUsage.ReUse,

                    //AllowedScopes =
                    //{
                    //    IdentityServerConstants.StandardScopes.OpenId,
                    //    IdentityServerConstants.StandardScopes.Profile,
                    //    "offline_access",
                    //    "api1",
                    //}
                    AllowedScopes = { "openid", "profile", "email", "api1" }
                }
            };
    }
}
