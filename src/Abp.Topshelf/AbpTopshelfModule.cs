namespace Abp.Topshelf
{
    using System.Reflection;
    using Modules;

    [DependsOn(typeof(AbpKernelModule))]
    public class AbpTopshelfModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
