using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public class ProductDatabaseVersion : IDatabaseVersion
    {
        private readonly IDictionary<DatabaseVersionType, int> _versions;
        private readonly DatabaseVersionType[] _types;

        protected ProductDatabaseVersion(int schema, int content, params DatabaseVersionType[] types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof (types));
            this._versions = (IDictionary<DatabaseVersionType, int>) new Dictionary<DatabaseVersionType, int>()
            {
                {
                    DatabaseVersionType.Schema,
                    schema
                },
                {
                    DatabaseVersionType.Content,
                    content
                }
            };
            this._types = types;
        }

        DatabaseVersionType[] IDatabaseVersion.Types
        {
            get
            {
                return this._types;
            }
        }

        int IDatabaseVersion.this[DatabaseVersionType type]
        {
            get
            {
                return this._versions[type];
            }
        }

        public static IDatabaseVersion Create(
            int schema,
            int content,
            params DatabaseVersionType[] types)
        {
            return (IDatabaseVersion) new ProductDatabaseVersion(schema, content, types);
        }
    }
}
