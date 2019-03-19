using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal class ProductServerFamilyName
    {
        private static readonly IDictionary<ServerFamily, string> Families = (IDictionary<ServerFamily, string>) new Dictionary<ServerFamily, string>()
        {
            {
                ServerFamily.Undefined,
                string.Empty
            },
            {
                ServerFamily.Sql2000,
                "SQL Server 2000"
            },
            {
                ServerFamily.Sql2005,
                "SQL Server 2005"
            },
            {
                ServerFamily.Sql2008,
                "SQL Server 2008"
            },
            {
                ServerFamily.Sql2008R2,
                "SQL Server 2008 R2"
            },
            {
                ServerFamily.Sql2012,
                "SQL Server 2012"
            },
            {
                ServerFamily.Sql2014,
                "SQL Server 2014"
            },
            {
                ServerFamily.Sql2016,
                "SQL Server 2016"
            },
            {
                ServerFamily.Sql2017,
                "SQL Server 2017"
            }
        };
        private readonly ServerFamily _family;

        public ProductServerFamilyName(ServerFamily family)
        {
            this._family = family;
        }

        public override string ToString()
        {
            string str;
            if (!ProductServerFamilyName.Families.TryGetValue(this._family, out str))
                throw new InvalidEnumArgumentException();
            return str;
        }

        public static implicit operator string(ProductServerFamilyName name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof (name));
            return name.ToString();
        }
    }
}
