using LMS;
using LMS.Service;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace LMS.Service
{
    using Abp.Owin;
    using Hangfire;
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseAbp<LMSServiceModule>();
            app.UseHangfireDashboard("");
        }
    }
}