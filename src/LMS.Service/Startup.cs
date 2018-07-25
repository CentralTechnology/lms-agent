using LMS;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace LMS
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using Abp.Owin;
    using Core;
    using Core.Extensions;
    using Core.Logging;
    using Hangfire;
    using Owin;
    using Serilog.Core;
    using Serilog.Events;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LMSCoreModule.CurrentLogLevel = new LoggingLevelSwitch();
            bool debug = ConfigurationManager.AppSettings.Get("Debug").To<bool>();
            if (debug)
            {
                LMSCoreModule.CurrentLogLevel.MinimumLevel = LogEventLevel.Verbose;
            }

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