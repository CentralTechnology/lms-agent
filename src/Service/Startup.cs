using LMS.Service;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace LMS.Service
{
    using System.Diagnostics.CodeAnalysis;
    using Abp.Owin;
    using Castle.Facilities.Logging;
    using Castle.Services.Logging.SerilogIntegration;
    using global::Hangfire;
    using Owin;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseAbp<LMSServiceModule>(a =>
            {
                a.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.LogUsing<SerilogFactory>());
            });
            app.UseHangfireDashboard("");
        }
    }
}