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
using IdentityServer3.Admin.EntityFramework.Interfaces;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework.Entities;

namespace IdentityServer3.Admin.EntityFramework.Entities
{
    public class IdentityClient : IClient<int>
    {
        public int Id { get; set; }
        public int AbsoluteRefreshTokenLifetime { get; set; }
        public int AccessTokenLifetime { get; set; }
        public bool AllowAccessToAllGrantTypes { get; set; }
        public bool AllowAccessToAllScopes { get; set; }
        public bool AllowClientCredentialsOnly { get; set; }
        public bool AllowRememberConsent { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public int AuthorizationCodeLifetime { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public bool Enabled { get; set; }
        public bool EnableLocalLogin { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public bool IncludeJwtId { get; set; }
        public string LogoUri { get; set; }
        public bool PrefixClientClaims { get; set; }
        public bool RequireConsent { get; set; }
        public int SlidingRefreshTokenLifetime { get; set; }
        public bool UpdateAccessTokenOnRefresh { get; set; }
        public bool LogoutSessionRequired { get; set; }
        public string LogoutUri { get; set; }
        public TokenExpiration RefreshTokenExpiration { get; set; }
        public TokenUsage RefreshTokenUsage { get; set; }
        public AccessTokenType AccessTokenType { get; set; }
        public Flows Flow { get; set; }
        public ICollection<ClientClaim> Claims { get; set; }
        public ICollection<ClientSecret> ClientSecrets { get; set; }
        public ICollection<ClientIdPRestriction> IdentityProviderRestrictions { get; set; }
        public ICollection<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }
        public ICollection<ClientRedirectUri> RedirectUris { get; set; }
        public ICollection<ClientCorsOrigin> AllowedCorsOrigins { get; set; }
        public ICollection<ClientCustomGrantType> AllowedCustomGrantTypes { get; set; }
        public ICollection<ClientScope> AllowedScopes { get; set; }
        public bool RequireSignOutPrompt { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
    }
}