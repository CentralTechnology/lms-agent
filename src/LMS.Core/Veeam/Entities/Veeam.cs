// ReSharper disable CheckNamespace
namespace Portal.LicenseMonitoringSystem.Veeam.Entities
{
    public partial class Veeam
    {
        public override string ToString()
        {
            return $"Edition: {Edition}  License: {LicenseType}  Version: {ProgramVersion}  Hyper-V: {HyperV}  VMWare: {vSphere}";
        }

        public void UpdateValues(Veeam veeam)
        {
            ClientVersion = veeam.ClientVersion;
            DeleterUserId = null;
            DeletionTime = null;
            Edition = veeam.Edition;
            ExpirationDate = veeam.ExpirationDate;
            Hostname = veeam.Hostname;
            HyperV = veeam.HyperV;
            IsDeleted = false;
            LicenseType = veeam.LicenseType;
            ProgramVersion = veeam.ProgramVersion;
            SupportId = veeam.SupportId;
            TenantId = veeam.TenantId;
            vSphere = veeam.vSphere;
        }
    }
}
