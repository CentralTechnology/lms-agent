using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.DBManager
{
    public interface IDbConnection : IDisposable
    {
        SqlTransaction Transaction { get; }

        SqlConnection ObtainConnection();

        bool CheckOpenedConnection();

        bool TryToObtainConnection();
    }
}
