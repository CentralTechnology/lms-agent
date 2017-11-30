using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Configuration
{
    using Abp.Configuration;

    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.AutotaskAccountId, default(int).ToString()),
                new SettingDefinition(AppSettingNames.CentrastageDeviceId,default(Guid).ToString()),
                new SettingDefinition(AppSettingNames.MonitorUsers,false.ToString()),
                new SettingDefinition(AppSettingNames.MonitorVeeam,false.ToString()),
                new SettingDefinition(AppSettingNames.PrimaryDomainControllerOverride,false.ToString()),
                new SettingDefinition(AppSettingNames.VeeamOverride, false.ToString()),
                new SettingDefinition(AppSettingNames.VeeamVersion, string.Empty),
                new SettingDefinition(AppSettingNames.UsersOverride, false.ToString()),
            };
        }
    }
}
