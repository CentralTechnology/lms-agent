using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Common;
using LMS.Core.Veeam.DBManager;

namespace LMS.Core.Veeam.Backup.DBManager
{
    public class CInstanceLicensingDbScope
    {
        private static readonly ISqlFieldDescriptor<Guid> InstanceIdField = SqlFieldDescriptor.UniqueIdentifier("instance_id");
        private static readonly ISqlFieldDescriptor<ELicensePlatform> LicensePlatformField = SqlFieldDescriptor.IntEnum<ELicensePlatform>("license_platform");
        private static readonly ISqlFieldDescriptor<DateTime> RegistrationTimeField = SqlFieldDescriptor.DateTime("registration_time");
        private static readonly ISqlFieldDescriptor<DateTime> LastProcessingTimeField = SqlFieldDescriptor.DateTime("last_processing_time");
        private static readonly ISqlFieldDescriptor<string> UuidField = SqlFieldDescriptor.NVarChar("uuid", 512);
        private static readonly ISqlFieldDescriptor<string> DnsNameField = SqlFieldDescriptor.NVarChar("dns_name", (int) byte.MaxValue);
        private readonly IDatabaseAccessor _dbAccessor;

        internal CInstanceLicensingDbScope(IDatabaseAccessor dbAccessor)
        {
            this._dbAccessor = dbAccessor;
        }

        public void CreateOrUpdateInstance(ILicensedInstance instance)
        {
            this._dbAccessor.ExecNonQuery("[dbo].[Licensing.CreateOrUpdateInstance]", CInstanceLicensingDbScope.InstanceIdField.MakeParam(instance.InstanceId), CInstanceLicensingDbScope.LicensePlatformField.MakeParam(instance.Platform.Platform), CInstanceLicensingDbScope.RegistrationTimeField.MakeParam(instance.LastProcessingTime), CInstanceLicensingDbScope.LastProcessingTimeField.MakeParam(instance.LastProcessingTime), CInstanceLicensingDbScope.UuidField.MakeParam(instance.Uuid), CInstanceLicensingDbScope.DnsNameField.MakeParam(instance.DnsName));
        }

        public void UnregisterInstance(Guid instanceId)
        {
            this._dbAccessor.ExecNonQuery("[dbo].[Licensing.UnregisterInstance]", CInstanceLicensingDbScope.InstanceIdField.MakeParam(instanceId));
        }
    }
}
