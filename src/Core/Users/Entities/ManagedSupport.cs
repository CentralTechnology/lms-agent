using System;

namespace Portal.LicenseMonitoringSystem.Users.Entities
{
    public partial class ManagedSupport
    {
        public static ManagedSupport Create(string clientVersion, Guid deviceId)
        {
            return new ManagedSupport
            {
                ClientVersion = clientVersion,
                DeviceId = deviceId,
                Hostname = Environment.MachineName
            };
        }
    }
}
