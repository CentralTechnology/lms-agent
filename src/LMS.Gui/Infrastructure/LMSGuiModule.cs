using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Gui.Infrastructure
{
    using System.Reflection;
    using Abp.Modules;
    using Core;

    [DependsOn(typeof(LMSEntityFrameworkModule))]
    public class LMSGuiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
