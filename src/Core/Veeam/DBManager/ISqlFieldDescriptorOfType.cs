using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam.DBManager
{
    using System.Data;
    using System.Data.SqlClient;

    public interface ISqlFieldDescriptor<T> : ISqlFieldDescriptor
    {
        SqlParameter MakeParam(T value);

        T Read(IDataReader reader);

        T Read(IDataReader reader, T defaultValue);
    }
}
