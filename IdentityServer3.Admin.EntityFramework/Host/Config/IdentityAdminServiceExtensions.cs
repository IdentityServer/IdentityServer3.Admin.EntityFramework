using Host.Config;
using IdentityAdmin.Configuration;
using IdentityAdmin.Core;

namespace WebHost.Config
{
    public static class IdentityAdminServiceExtensions
    {
        public static void Configure(this IdentityAdminServiceFactory factory)
        {
            factory.IdentityAdminService = new Registration<IIdentityAdminService, IdentityAdminManagerService>();
        }
    }
}