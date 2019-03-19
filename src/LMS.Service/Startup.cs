using LMS;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace LMS
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using Abp.Owin;
    using Core.Extensions;
    using Hangfire;
    using Owin;
    using Serilog.Core;
    using Serilog.Events;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Program.CurrentLogLevel = new LoggingLevelSwitch();
            bool debug = ConfigurationManager.AppSettings.Get("Debug").To<bool>();
            if (debug)
            {
                Program.CurrentLogLevel.MinimumLevel = LogEventLevel.Verbose;
            }

            app.UseAbp<LMSServiceModule>();
            app.UseHangfireDashboard("");
        }
    }
}