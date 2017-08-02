using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam.DBManager
{
    using System.Data;

    public interface ISqlFieldDescriptor
    {
        string FieldName { get; }

        SqlDbType FieldType { get; }

        string MakeSqlType();
    }
}
