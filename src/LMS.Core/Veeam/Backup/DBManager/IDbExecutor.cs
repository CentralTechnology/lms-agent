using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.DBManager
{
    public interface IDbExecutor : IDisposable
    {
        void ExecuteNonQuery(string query, params SqlParameter[] args);

        void ExecuteNonQuery(TimeSpan timeout, string query, params SqlParameter[] args);

        SqlDataReader ExecuteQueryAsReader(string query, params SqlParameter[] args);

        SqlDataReader ExecuteQueryAsReader(
            TimeSpan timeout,
            string query,
            params SqlParameter[] args);

        SqlDataReader ExecuteStoredProcAsReader(
            string procedureName,
            params SqlParameter[] args);

        SqlDataReader ExecuteStoredProcAsReader(
            TimeSpan timeout,
            string procedureName,
            params SqlParameter[] args);

        void ExecuteBulkCopy(IDataReader dataReader, string destinationTable);

        void TransactionalBulkCopy(
            IDataReader dataReader,
            string destinationTable,
            SqlBulkCopyOptions sqlBulkOptions);
    }
}
