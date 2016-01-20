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