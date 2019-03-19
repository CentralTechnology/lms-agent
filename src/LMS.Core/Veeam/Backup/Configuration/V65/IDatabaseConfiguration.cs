using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IDatabaseConfiguration : IConfiguration
    {
        IDatabaseConfigurationInfo Info { get; }

        IDatabaseConfigurationInfo Default { get; }

        IDatabaseConfiguration Copy();

        void Load(RegistryView registryView = RegistryView.Default);

        void Save(bool secured = true, RegistryView registryView = RegistryView.Default);
    }
}
