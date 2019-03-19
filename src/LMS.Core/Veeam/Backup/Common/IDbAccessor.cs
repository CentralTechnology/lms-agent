using System.Data;
using System.Data.SqlClient;

namespace LMS.Core.Veeam.Backup.Common
{
    public interface IDbAccessor
    {
        IDataReader ExecQuery(string query, params SqlParameter[] parameters);

        void ExecCommand(CTransactionScopeIdentifier identifier, string commandText);
    }
}
