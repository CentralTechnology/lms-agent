using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.Backup.DBManager;

namespace LMS.Core.Veeam.DBManager
{
    public class SqlLicensePlatformTableType : ISqlTableType, IDisposable
    {
        private const string Column1Name = "platform";
        private const string Column2Name = "ep_license_mode";

        public DataTable Table { get; private set; }

        public SqlLicensePlatformTableType()
        {
            this.Table = new DataTable();
            this.Table.Columns.Add("platform", TypeCache<SqlInt32>.Type);
            this.Table.Columns.Add("ep_license_mode", TypeCache<SqlInt32>.Type);
        }

        public void AddRow(ELicensePlatform value, EEpLicenseMode? licenseMode)
        {
            DataRowCollection rows = this.Table.Rows;
            object[] objArray1 = new object[2]
            {
                (object) (int) value,
                null
            };
            object[] objArray2 = objArray1;
            EEpLicenseMode? nullable = licenseMode;
            // ISSUE: variable of a boxed type
            var local = (System.ValueType) (nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?());
            objArray2[1] = (object) local;
            object[] objArray3 = objArray1;
            rows.Add(objArray3);
        }

        public SqlParameter ToSqlParameter(string parameterName)
        {
            return CDbAccessor.MakeParam("@" + parameterName, (object) this.Table, SqlDbType.Structured);
        }

        public void Dispose()
        {
            this.Table.Dispose();
        }
    }
}
