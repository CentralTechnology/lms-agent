using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.DBManager
{
    public interface ISqlTableType
    {
        DataTable Table { get; }

        SqlParameter ToSqlParameter(string parameterName);
    }
}
