// ReSharper disable CheckNamespace
namespace Portal.LicenseMonitoringSystem.Users.Entities
{
    using System;

    public partial class ManagedSupport
    {
        public static ManagedSupport Create(string clientVersion, Guid deviceId, long accountId)
        {
            return new ManagedSupport
            {
                ClientVersion = clientVersion,
                DeviceId = deviceId,
                Hostname = Environment.MachineName,
                TenantId = Convert.ToInt32(accountId)
            };
        }
    }
}