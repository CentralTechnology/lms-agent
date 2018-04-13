// ReSharper disable CheckNamespace
namespace Portal.LicenseMonitoringSystem.Veeam.Entities
{
    public partial class Veeam
    {
        public override string ToString()
        {
            return $"Edition: {Edition}  License: {LicenseType}  Version: {ProgramVersion}  Hyper-V: {HyperV}  VMWare: {vSphere}";
        }
    }
}
