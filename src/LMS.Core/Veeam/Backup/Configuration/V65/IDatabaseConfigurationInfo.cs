using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IDatabaseConfigurationInfo
    {
        string ServerName { get; set; }

        string ServerInstanceName { get; set; }

        string ServerInstancePipeName { get; set; }

        int ServerInstancePort { get; set; }

        string ServerInstanceFullName { get; }

        string InitialCatalog { get; set; }

        string CustomConnectionString { get; set; }

        int ConnectionRetryCount { get; set; }

        int ConnectionRetryTimeout { get; set; }

        int ConnectionTimeout { get; set; }

        int SetupConnectionTimeout { get; set; }

        int StatementTimeout { get; set; }

        int SetupStatementTimeout { get; set; }

        bool SqlAuthentication { get; }

        string Login { get; set; }

        string Password { get; set; }

        bool ImpersonateUser { get; }

        string ExecuteAsUser { get; set; }

        bool EnableLog { get; set; }

        bool EnableExtendedLog { get; set; }

        string LockInfo { get; set; }

        int MaxPoolSize { get; set; }

        IDatabaseConfigurationInfo Copy();
    }
}
