namespace Abp.Topshelf
{
    using Abp;
    using global::Topshelf.HostConfigurators;
    using global::Topshelf.Logging;

    public static class HostConfiguratorExtensions
    {
        public static HostConfigurator UseAbp(this HostConfigurator configurator, AbpBootstrapper abpBootstrapper)
        {
            var log = HostLogger.Get(typeof(HostConfiguratorExtensions));

            log.Info("[Abp.Topshelf] Integration Started in host.");

            configurator.AddConfigurator(new AbpBuilderConfigurator(abpBootstrapper));
            return configurator;
        }
    }
}