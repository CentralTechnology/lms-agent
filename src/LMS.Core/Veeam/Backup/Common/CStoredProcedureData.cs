using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LMS.Core.Veeam.Backup.Common
{
    public class CStoredProcedureData
    {
        private readonly int? _timeout;

        public string ProcedureName { get; private set; }

        public IEnumerable<SqlParameter> Parameters { get; private set; }

        public bool IsLoggingEnabled { get; set; }

        public CStoredProcedureData(string procedureName, params SqlParameter[] parameters)
        {
            this.ProcedureName = procedureName;
            this.Parameters = (IEnumerable<SqlParameter>) parameters;
            this._timeout = new int?();
        }

        public CStoredProcedureData(
            string procedureName,
            int timeout,
            params SqlParameter[] parameters)
        {
            this.ProcedureName = procedureName;
            this.Parameters = (IEnumerable<SqlParameter>) parameters;
            this._timeout = new int?(timeout);
        }

        public bool IsTimeoutSpecified
        {
            get
            {
                return this._timeout.HasValue;
            }
        }

        public int Timeout
        {
            get
            {
                if (!this._timeout.HasValue)
                    throw new InvalidOperationException("Timeout is not specified.");
                return this._timeout.Value;
            }
        }
    }
}
