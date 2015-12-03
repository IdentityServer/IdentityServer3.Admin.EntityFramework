using System.Collections.Generic;
using System.Linq;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework;

namespace Host.Config
{
    internal class Factory
    {
        public static IdentityServerServiceFactory Configure(string connString)
        {
            var efConfig = new EntityFrameworkServiceOptions
            {
                ConnectionString = connString,
            };
            var factory = new IdentityServerServiceFactory();
            //Clients
            ConfigureClients(Clients.Get(), efConfig);
            //scopes
            ConfigureScopes(Scopes.Get(), efConfig);
            factory.RegisterConfigurationServices(efConfig);
            factory.RegisterOperationalServices(efConfig);
            return factory;
        }

        private static void ConfigureClients(IEnumerable<Client> clients, EntityFrameworkServiceOptions options)
        {
            using (var db = new ClientConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Clients.Any())
                {
                    foreach (var c in clients)
                    {
                        var e = c.ToEntity();
                        db.Clients.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

        private static void ConfigureScopes(IEnumerable<Scope> scopes, EntityFrameworkServiceOptions options)
        {
            using (var db = new ScopeConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Scopes.Any())
                {
                    foreach (var s in scopes)
                    {
                        var e = s.ToEntity();
                        db.Scopes.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}