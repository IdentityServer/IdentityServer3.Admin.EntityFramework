/*
 * Copyright 2015 Bert Hoorne, Dominick Baier, Brock Allen
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
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using IdentityServer3.Admin.EntityFramework.Entities;
using IdentityServer3.Core.Models;

namespace IdentityServer3.Admin.EntityFramework.Extensions
{
    public static class ModelMap
    {
        private static readonly MapperConfiguration Config;
        static ModelMap()
        {
            Config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Scope, IdentityScope>(MemberList.Source)
               .ForSourceMember(x => x.Claims, opts => opts.Ignore())
               .ForMember(x => x.ScopeClaims, opts => opts.MapFrom(src => src.Claims.Select(x => x)))
               .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
                cfg.CreateMap<ScopeClaim, IdentityServer3.EntityFramework.Entities.ScopeClaim>(MemberList.Source);
                cfg.CreateMap<Secret, IdentityServer3.EntityFramework.Entities.ScopeSecret>(MemberList.Source);
                cfg.CreateMap<Secret, IdentityServer3.EntityFramework.Entities.ClientSecret>(MemberList.Source);
                cfg.CreateMap<Client, IdentityClient>(MemberList.Source)
                    .ForMember(x => x.UpdateAccessTokenOnRefresh, opt => opt.MapFrom(src => src.UpdateAccessTokenClaimsOnRefresh))
                    .ForMember(x => x.AllowAccessToAllGrantTypes, opt => opt.MapFrom(src => src.AllowAccessToAllCustomGrantTypes))
                    .ForMember(x => x.AllowedCustomGrantTypes, opt => opt.MapFrom(src => src.AllowedCustomGrantTypes.Select(x => new IdentityServer3.EntityFramework.Entities.ClientCustomGrantType { GrantType = x })))
                    .ForMember(x => x.RedirectUris, opt => opt.MapFrom(src => src.RedirectUris.Select(x => new IdentityServer3.EntityFramework.Entities.ClientRedirectUri { Uri = x })))
                    .ForMember(x => x.PostLogoutRedirectUris, opt => opt.MapFrom(src => src.PostLogoutRedirectUris.Select(x => new IdentityServer3.EntityFramework.Entities.ClientPostLogoutRedirectUri { Uri = x })))
                    .ForMember(x => x.IdentityProviderRestrictions, opt => opt.MapFrom(src => src.IdentityProviderRestrictions.Select(x => new IdentityServer3.EntityFramework.Entities.ClientIdPRestriction { Provider = x })))
                    .ForMember(x => x.AllowedScopes, opt => opt.MapFrom(src => src.AllowedScopes.Select(x => new IdentityServer3.EntityFramework.Entities.ClientScope { Scope = x })))
                    .ForMember(x => x.AllowedCorsOrigins, opt => opt.MapFrom(src => src.AllowedCorsOrigins.Select(x => new IdentityServer3.EntityFramework.Entities.ClientCorsOrigin { Origin = x })))
                    .ForMember(x => x.Claims, opt => opt.MapFrom(src => src.Claims.Select(x => new IdentityServer3.EntityFramework.Entities.ClientClaim { Type = x.Type, Value = x.Value })));
            });
               
        }

        public static IdentityScope ToEntity(this Core.Models.Scope s)
        {
            if (s == null) return null;

            if (s.Claims == null)
            {
                s.Claims = new List<ScopeClaim>();
            }
            if (s.ScopeSecrets == null)
            {
                s.ScopeSecrets = new List<Secret>();
            }

            return Config.CreateMapper().Map<Core.Models.Scope, IdentityScope>(s);
        }

        public static IdentityClient ToEntity(this Core.Models.Client s)
        {
            if (s == null) return null;

            if (s.ClientSecrets == null)
            {
                s.ClientSecrets = new List<Secret>();
            }
            if (s.RedirectUris == null)
            {
                s.RedirectUris = new List<string>();
            }
            if (s.PostLogoutRedirectUris == null)
            {
                s.PostLogoutRedirectUris = new List<string>();
            }
            if (s.AllowedScopes == null)
            {
                s.AllowedScopes = new List<string>();
            }
            if (s.IdentityProviderRestrictions == null)
            {
                s.IdentityProviderRestrictions = new List<string>();
            }
            if (s.Claims == null)
            {
                s.Claims = new List<Claim>();
            }
            if (s.AllowedCustomGrantTypes == null)
            {
                s.AllowedCustomGrantTypes = new List<string>();
            }
            if (s.AllowedCorsOrigins == null)
            {
                s.AllowedCorsOrigins = new List<string>();
            }

            return Config.CreateMapper().Map<Core.Models.Client, IdentityClient>(s);
        }
    }
}
