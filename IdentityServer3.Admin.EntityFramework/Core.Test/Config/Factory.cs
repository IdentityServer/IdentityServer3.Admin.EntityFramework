using IdentityServer3.Core.Configuration;
using IdentityServer3.EntityFramework;

namespace Core.Test.Config
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

            factory.RegisterConfigurationServices(efConfig);
            factory.RegisterOperationalServices(efConfig);
            return factory;
        }

    }
}