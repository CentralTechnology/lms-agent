namespace Abp.Topshelf
{
    using Dependency;
    using global::Topshelf.Logging;
    using global::Topshelf.ServiceConfigurators;

    public static class AbpServiceConfiguratorExtensions
    {
        public static ServiceConfigurator<T> ConstructUsingAbp<T>(this ServiceConfigurator<T> configurator) where T : class
        {
            var log = HostLogger.Get(typeof(HostConfiguratorExtensions));

            log.Info("[Abp.Topshelf] Service configured to construct using Abp.");

            configurator.ConstructUsing(serviceFactory => IocManager.Instance.Resolve<T>());

            return configurator;
        }
    }
}