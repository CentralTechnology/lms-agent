namespace Service
{
    using System.Reflection;
    using Abp.Modules;
    using Abp.Topshelf;
    using Core;

    [DependsOn(typeof(AbpTopshelfModule), typeof(CoreModule))]
    public class ServiceModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}