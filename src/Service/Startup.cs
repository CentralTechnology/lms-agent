using LMS;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace LMS
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Abp.Owin;
    using Castle.Core.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Services.Logging.SerilogIntegration;
    using Core;
    using Core.Logging;
    using global::Hangfire;
    using Owin;
    using Serilog;
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