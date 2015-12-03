using IdentityAdmin.Configuration;
using Owin;
using Serilog;
using WebHost.Config;

namespace Host
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Trace()
               .CreateLogger();

            app.Map("/adm", adminApp =>
            {
                var factory = new IdentityAdminServiceFactory();
                factory.Configure();
                adminApp.UseIdentityAdmin(new IdentityAdminOptions
                {
                    Factory = factory
                });
            });
        }
    }
}