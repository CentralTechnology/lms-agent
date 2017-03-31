namespace Abp.Topshelf
{
    using System.Collections.Generic;
    using Abp;
    using global::Topshelf.Builders;
    using global::Topshelf.Configurators;
    using global::Topshelf.HostConfigurators;

    public class AbpBuilderConfigurator : HostBuilderConfigurator
    {
        private readonly AbpBootstrapper _abpBootstrapper;

        public AbpBuilderConfigurator(AbpBootstrapper abpBootstrapper)
        {
            _abpBootstrapper = abpBootstrapper;
        }

        public IEnumerable<ValidateResult> Validate()
        {
            yield break;
        }

        public HostBuilder Configure(HostBuilder builder)
        {
            return builder;
        }
    }
}