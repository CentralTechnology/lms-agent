namespace Core
{
    using System.Reflection;
    using Abp.Modules;
    using Abp.WebApi;

    [DependsOn(typeof(AbpWebApiModule))]
    public class CoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}