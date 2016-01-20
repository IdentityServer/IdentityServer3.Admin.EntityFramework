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

using System.Linq;
using System.Security.Claims;
using AutoMapper;
using IdentityServer3.Admin.EntityFramework.Entities;

namespace IdentityServer3.Admin.EntityFramework.Extensions
{
    public static class EntitiesMap
    {
        static EntitiesMap()
        {
            Mapper.CreateMap<IdentityScope, IdentityServer3.Core.Models.Scope>(MemberList.Destination)
                .ForMember(x => x.Claims, opts => opts.MapFrom(src => src.ScopeClaims.Select(x => x)))
                .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
            Mapper.CreateMap<IdentityServer3.EntityFramework.Entities.ScopeClaim, IdentityServer3.Core.Models.ScopeClaim>(MemberList.Destination);
            Mapper.CreateMap<IdentityServer3.EntityFramework.Entities.ScopeSecret, IdentityServer3.Core.Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => !srs.IsSourceValueNull));

            Mapper.CreateMap<IdentityServer3.EntityFramework.Entities.ClientSecret, IdentityServer3.Core.Models.Secret>(MemberList.Destination)
                 .ForMember(dest => dest.Type, opt => opt.Condition(srs => !srs.IsSourceValueNull));
            Mapper.CreateMap<IdentityClient, IdentityServer3.Core.Models.Client>(MemberList.Destination)
                .ForMember(x => x.UpdateAccessTokenClaimsOnRefresh, opt => opt.MapFrom(src => src.UpdateAccessTokenOnRefresh))
                .ForMember(x => x.AllowAccessToAllCustomGrantTypes, opt => opt.MapFrom(src => src.AllowAccessToAllGrantTypes))
                .ForMember(x => x.AllowedCustomGrantTypes, opt => opt.MapFrom(src => src.AllowedCustomGrantTypes.Select(x => x.GrantType)))
                .ForMember(x => x.RedirectUris, opt => opt.MapFrom(src => src.RedirectUris.Select(x => x.Uri)))
                .ForMember(x => x.PostLogoutRedirectUris, opt => opt.MapFrom(src => src.PostLogoutRedirectUris.Select(x => x.Uri)))
                .ForMember(x => x.IdentityProviderRestrictions, opt => opt.MapFrom(src => src.IdentityProviderRestrictions.Select(x => x.Provider)))
                .ForMember(x => x.AllowedScopes, opt => opt.MapFrom(src => src.AllowedScopes.Select(x => x.Scope)))
                .ForMember(x => x.AllowedCorsOrigins, opt => opt.MapFrom(src => src.AllowedCorsOrigins.Select(x => x.Origin)))
                .ForMember(x => x.Claims, opt => opt.MapFrom(src => src.Claims.Select(x => new Claim(x.Type, x.Value))));
        }

        public static IdentityServer3.Core.Models.Scope ToModel(this IdentityScope s)
        {
            if (s == null) return null;
            return Mapper.Map<IdentityScope, IdentityServer3.Core.Models.Scope>(s);
        }

        public static IdentityServer3.Core.Models.Client ToModel(this IdentityClient s)
        {
            if (s == null) return null;
            return Mapper.Map<IdentityClient, IdentityServer3.Core.Models.Client>(s);
        }
    }
}
