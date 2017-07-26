using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam
{
    using System.Data;
    using System.Xml.Linq;
    using Backup.Common;
    using Common.Constants;
    using DBManager;

    public class VeeamVersion9501038 : VeeamVersion
    {
        /// <inheritdoc />
        public override int GetProtectedVms()
        {
            using (DataTableReader dataReader = LocalDbAccessor.GetDataTable("GetProtectedVmCount", DbAccessor.MakeParam("@days", Constants.VeeamProtectedVmCountDays)).CreateDataReader())
            {
                if (dataReader.Read())
                {
                    return (int)dataReader["vm_count"];
                }
            }

            return 0;
        }

        /// <inheritdoc />
        public override int GetProtectedVms(EPlatform platform)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Veeam Build()
        {
            var veeam = new Veeam
            {
                vSphere = GetProtectedVms()
            };

            return veeam;
        }
    }
}
