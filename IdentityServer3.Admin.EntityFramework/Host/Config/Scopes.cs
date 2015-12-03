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