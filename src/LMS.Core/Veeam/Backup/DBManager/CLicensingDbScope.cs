using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Extensions;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.Backup.Model;
using LMS.Core.Veeam.DBManager;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public class CLicensingDbScope
  {
    private readonly IDatabaseAccessor _dbAccessor;

    internal CLicensingDbScope(IDatabaseAccessor dbAccessor)
    {
      this._dbAccessor = dbAccessor;
      this.Instances = new CInstanceLicensingDbScope(dbAccessor);
    }

    public CInstanceLicensingDbScope Instances { get; private set; }

    public int GetProtectedInstancesCounter(SqlLicensePlatformTableType platforms)
    {
      using (ReadableTable readableTable = this._dbAccessor.GetReadableTable("[dbo].[Licensing.GetProtectedVmsCount]", platforms.ToSqlParameter("platformTable")))
        return readableTable.ReadSingleOrDefault<int>((Func<DataTableReader, int>) (reader => reader.GetValue<int>("protected_vms_count")));
    }

    
  }
}
