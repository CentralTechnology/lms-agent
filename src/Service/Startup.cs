using LMS;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace LMS
{
    using System.Diagnostics.CodeAnalysis;
    using Abp.Owin;
    using Core;
    using Core.Logging;
    using Hangfire;
    using Owin;
    using Serilog.Core;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LMSCoreModule.CurrentLogLevel = new LoggingLevelSwitch();

            app.UseAbp<LMSServiceModule>(a =>
            {
                a.IocManager.IocContainer.Register(
                    LoggingConfiguration.GetConfiguration(LMSCoreModule.CurrentLogLevel)
                );
            });
            app.UseHangfireDashboard("");
        }
    }
}