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

namespace Host.Config
{
    public static class Scopes
    {   
        /// <summary>
        /// These are all the initial scopes that are persisted in the database if these are empty
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Scope> Get()
        {
            return new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.Email,
                StandardScopes.OfflineAccess,
                StandardScopes.Roles,
                new Scope
                {
                    Enabled = true,
                    Name = "read",
                    Type = ScopeType.Resource,
                    
                }, new Scope
                {
                    Enabled = true,
                    Name = "write", 
                    Type = ScopeType.Resource
                }
            };
        }


    }
}