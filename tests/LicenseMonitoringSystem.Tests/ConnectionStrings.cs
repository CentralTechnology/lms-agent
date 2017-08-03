using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Tests
{
    using System.Data.SqlClient;

    public static class ConnectionStrings
    {
        private const string Veeam900902Database = "9.0.0.902";
        public static string Veeam900902()
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = $"|DataDirectory|{Veeam900902Database}.sdf"
                
            };

            return connectionString.ConnectionString;
        }
    }
}
