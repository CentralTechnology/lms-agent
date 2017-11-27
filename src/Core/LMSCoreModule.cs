using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    using System.Reflection;
    using Abp.Configuration;
    using Abp.Modules;
    using Abp.WebApi;
    using Configuration;
    using LMS.Configuration;

    [DependsOn(typeof(AbpWebApiModule))]
    public class LMSCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.MultiTenancy.IsEnabled = false;
            Configuration.Settings.Providers.Add<AppSettingProvider>();
            
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
