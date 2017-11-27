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
    using Abp.Zero;
    using Configuration;

    [DependsOn(typeof(AbpZeroCoreModule))]
    public class LMSCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<ISettingStore, SettingStore>();
            Configuration.MultiTenancy.IsEnabled = false;
            Configuration.Settings.Providers.Add<AppSettingProvider>();
            
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
