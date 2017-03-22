using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Settings
{
    using Abp.Configuration;

    public class MySettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition("Url", "http://itadams.co.uk",scopes: SettingScopes.Application)
            };
        }
    }
}
