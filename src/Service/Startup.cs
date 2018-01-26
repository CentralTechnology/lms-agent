using LMS.Service;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace LMS.Service
{
    using System.Diagnostics.CodeAnalysis;
    using Abp.Owin;
    using global::Hangfire;
    using Hangfire;
    using Owin;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseAbp<LMSServiceModule>();
            app.UseHangfireDashboard("");
            GlobalJobFilters.Filters.Add(new DisableMultipleQueuedItemsFilter());
        }
    }
}