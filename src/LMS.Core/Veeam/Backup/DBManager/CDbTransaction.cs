using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Common;
using LMS.Core.Veeam.Backup.Common.Sources;
using LMS.Core.Veeam.Backup.Configuration.V65;
using Serilog;

namespace LMS.Core.Veeam.Backup.DBManager
{
    public class CDbTransaction : IDisposeThrowable, IDisposable
    {
        private CDisposableList m_disposableObjects = new CDisposableList();
        private bool m_isCommited;
        private string m_transactionName;
        private IDbConnection m_dbConnection;

        public CDbTransaction(
            string transactionName,
            IDatabaseConfiguration dbConnectionParams,
            IsolationLevel level)
        {
            try
            {
                this.m_transactionName = transactionName;
                CNotRetryableDbConnection disposableObject = new CNotRetryableDbConnection(dbConnectionParams.ConnectionString);
                this.m_disposableObjects.Add<CNotRetryableDbConnection>(disposableObject);
                disposableObject.BeginTransaction(transactionName, level);
                this.m_dbConnection = (IDbConnection) disposableObject;
            }
            catch (Exception)
            {
                this.m_disposableObjects.Dispose();
                throw;
            }
        }

        public IDbConnection DbConnection
        {
            get
            {
                return this.m_dbConnection;
            }
        }

        public void Commit()
        {
            this.m_dbConnection.Transaction.Commit();
            this.m_isCommited = true;
        }

        void IDisposable.Dispose()
        {
            try
            {
                if (this.m_isCommited)
                    return;
                Log.Information("Rolling back transaction [{0}]", (object) this.m_transactionName);
                this.m_dbConnection.Transaction.Rollback();
            }
            catch (Exception ex)
            {
                CExceptionUtil.RegenTraceExc(ex, "Failed to rollback transaction [{0}]", (object) this.m_transactionName);
                throw;
            }
            finally
            {
                this.m_disposableObjects.Dispose();
            }
        }
    }
}
