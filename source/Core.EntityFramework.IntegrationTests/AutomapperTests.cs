using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IdentityServer3.Admin.EntityFramework.Entities;
using IdentityServer3.Admin.EntityFramework.Extensions;
using IdentityServer3.EntityFramework.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EntitiesMap = IdentityServer3.Admin.EntityFramework.Extensions.EntitiesMap;

namespace Core.EntityFramework.IntegrationTests
{
    [TestClass]
    public class AutomapperTests
    {
        [TestMethod]
        public void AutomapperConfigurationIsValid()
        {
            IdentityServer3.Core.Models.Scope s = new IdentityServer3.Core.Models.Scope()
            {
            };
            var e = s.ToEntity();

            IdentityServer3.Core.Models.Client c = new IdentityServer3.Core.Models.Client()
            {
            };
            var e2 = c.ToEntity();

            IdentityScope s2 = new IdentityScope()
            {
                ScopeClaims = new HashSet<IdentityServer3.EntityFramework.Entities.ScopeClaim>(),
                ScopeSecrets = new HashSet<IdentityServer3.EntityFramework.Entities.ScopeSecret>(),
            };

            var m = s2.ToModel();

            EntitiesMap.Config.AssertConfigurationIsValid();
        }
    }
}
