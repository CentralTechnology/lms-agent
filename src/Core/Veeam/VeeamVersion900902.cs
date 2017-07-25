namespace Core.Veeam
{
    using System;
    using System.Data;
    using Backup.Common;
    using Common.Constants;
    using DBManager;

    public class VeeamVersion900902 : VeeamVersion
    {
        public VeeamVersion900902()
        {
            
        }

        public VeeamVersion900902(string connectionString) 
            : base(connectionString)
        {
            
        }

        /// <inheritdoc />
        public override int GetProtectedVms()
        {
            using (DataTableReader dataReader = LocalDbAccessor.GetDataTable("GetProtectedVmCount", DbAccessor.MakeParam("@days", Constants.VeeamProtectedVmCountDays)).CreateDataReader())
            {
                if (dataReader.Read())
                {
                    return (int) dataReader["vm_count"];
                }
            }

            return 0;
        }

        /// <inheritdoc />
        public override int GetProtectedVms(EPlatform platform)
        {
            throw new NotImplementedException();
        }
    }
}