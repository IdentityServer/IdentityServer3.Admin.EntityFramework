using System.Collections.Generic;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;

namespace Host.Config
{
    public static class Clients
    {
        /// <summary>
        /// These are all the initial clients that are persisted in the database if these are empty
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "SpeedClient",
                    ClientName = " Speed Client",
                    Enabled = true,
                    Flow = Flows.Implicit,
                    RequireConsent = true,
                    AllowRememberConsent = true,
                    RedirectUris = new List<string>(),
                    PostLogoutRedirectUris = new List<string>(),
                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Email
                    },
                    AccessTokenType = AccessTokenType.Jwt
                },
                new Client
                {
                    ClientId = "Client",
                    ClientName = " Client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("5595B967-F04A-4644-94FA-1B066C7D9F8F".Sha256())
                    },
                    Enabled = true,
                    Flow = Flows.Hybrid,
                    RequireConsent = true,
                    AllowRememberConsent = true,  
                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.OfflineAccess,
                        "read", 
                        "write",
                    },
                    AccessTokenType = AccessTokenType.Jwt
                },
                new Client 
                {
                    ClientName = " IdentityServer",
                    ClientId = "IdentityServer",
                    Flow = Flows.Implicit,
                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.OfflineAccess,
                        Constants.StandardScopes.Roles,
                        "role"
                    }
                },
            };
        }
    }
}