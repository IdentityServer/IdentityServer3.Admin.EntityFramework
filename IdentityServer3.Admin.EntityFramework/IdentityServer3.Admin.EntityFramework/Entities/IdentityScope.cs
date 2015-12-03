using System.Collections.Generic;
using IdentityServer3.Admin.EntityFramework.Interfaces;
using IdentityServer3.EntityFramework.Entities;

namespace IdentityServer3.Admin.EntityFramework.Entities
{
    public class IdentityScope : IScope<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClaimsRule { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool Emphasize { get; set; }
        public bool Enabled { get; set; }
        public bool IncludeAllClaimsForUser { get; set; }
        public bool Required { get; set; }
        public ICollection<ScopeClaim> ScopeClaims { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public int Type { get; set; }
    }
}