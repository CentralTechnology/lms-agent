using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.DBManager
{
    public class CSqlExecErrorEventArgs : EventArgs
    {
        public readonly Exception Exception;
        public bool RetryDbOperation;

        public CSqlExecErrorEventArgs(Exception exc)
        {
            this.Exception = exc;
        }
    }
}
