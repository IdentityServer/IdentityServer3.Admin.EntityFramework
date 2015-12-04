/*
 * Copyright 2015 Bert Hoorne,Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
                    AccessTokenType = AccessTokenType.Jwt,
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