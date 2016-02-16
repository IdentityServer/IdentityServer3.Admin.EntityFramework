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
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework.Entities;

namespace IdentityServer3.Admin.EntityFramework.Interfaces
{
    // Summary:
    //     Minimal interface for a client
    //
    // Type parameters:
    //   TKey:
    public interface IClient<out TKey>
    {
        // Summary:
        //     Unique key for the client
        TKey Id { get; }
        //
        // Summary:
        //     Unique ClientId
        string ClientId { get; set; }

        //
        // Summary:
        //     Client name
        string ClientName { get; set; }
        int AbsoluteRefreshTokenLifetime { get; set; }
        int AccessTokenLifetime { get; set; }
        bool AllowAccessToAllGrantTypes { get; set; }
        bool AllowAccessToAllScopes { get; set; }
        bool AllowClientCredentialsOnly { get; set; }
        bool AllowRememberConsent { get; set; }
        bool AlwaysSendClientClaims { get; set; }
        int AuthorizationCodeLifetime { get; set; }
        string ClientUri { get; set; }
        bool Enabled { get; set; }
        bool EnableLocalLogin { get; set; }
        int IdentityTokenLifetime { get; set; }
        bool IncludeJwtId { get; set; }
        string LogoUri { get; set; }
        bool PrefixClientClaims { get; set; }
        bool RequireConsent { get; set; }
        int SlidingRefreshTokenLifetime { get; set; }
        bool UpdateAccessTokenOnRefresh { get; set; }
        bool LogoutSessionRequired { get; set; }
        string LogoutUri { get; set; }
        TokenExpiration RefreshTokenExpiration { get; set; }
        TokenUsage RefreshTokenUsage { get; set; }
        AccessTokenType AccessTokenType { get; set; }
        Flows Flow { get; set; }
        ICollection<ClientClaim> Claims { get; set; }
        ICollection<ClientSecret> ClientSecrets { get; set; }
        ICollection<ClientIdPRestriction> IdentityProviderRestrictions { get; set; }
        ICollection<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }
        ICollection<ClientRedirectUri> RedirectUris { get; set; }
        ICollection<ClientCorsOrigin> AllowedCorsOrigins { get; set; }
        ICollection<ClientCustomGrantType> AllowedCustomGrantTypes { get; set; }
        ICollection<ClientScope> AllowedScopes { get; set; }
        bool RequireSignOutPrompt { get; set; }
    }
}