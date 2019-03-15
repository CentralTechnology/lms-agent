using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public class ConnectionStringBuilder
    {
        private readonly IDatabaseConfigurationInfo _info;
        private readonly IDatabaseConfigurationInfo _default;

        public ConnectionStringBuilder(
            IDatabaseConfigurationInfo info,
            IDatabaseConfigurationInfo defaultInfo)
        {
            if (info == null)
                throw new ArgumentNullException(nameof (info));
            if (defaultInfo == null)
                throw new ArgumentNullException(nameof (defaultInfo));
            this._info = info;
            this._default = defaultInfo;
        }

        public override string ToString()
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(this._info.CustomConnectionString)
            {
                MultipleActiveResultSets = true,
                DataSource = this._info.ServerInstancePort == this._default.ServerInstancePort ? this._info.ServerInstanceFullName : string.Format("{0},{1}", (object) this._info.ServerInstanceFullName, (object) this._info.ServerInstancePort),
                InitialCatalog = this._info.InitialCatalog,
                IntegratedSecurity = !this._info.SqlAuthentication,
                ConnectTimeout = this._info.ConnectionTimeout
            };
            if (this._info.MaxPoolSize > 0)
                connectionStringBuilder.MaxPoolSize = this._info.MaxPoolSize;
            if (this._info.SqlAuthentication)
            {
                connectionStringBuilder.UserID = this._info.Login;
                connectionStringBuilder.Password = this._info.Password;
            }
            return connectionStringBuilder.ToString();
        }

        public static implicit operator string(ConnectionStringBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof (builder));
            return builder.ToString();
        }
    }
}
