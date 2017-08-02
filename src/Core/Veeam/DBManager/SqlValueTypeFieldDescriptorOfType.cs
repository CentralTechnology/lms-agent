using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam.DBManager
{
    using System.Data;
    using System.Data.SqlClient;

    internal class SqlValueTypeFieldDescriptor<T> : ISqlFieldDescriptor<T>, ISqlFieldDescriptor where T : struct
    {
        public string FieldName { get; private set; }

        public SqlDbType FieldType { get; private set; }

        public SqlValueTypeFieldDescriptor(string fieldName, SqlDbType fieldType)
        {
            this.FieldName = fieldName;
            this.FieldType = fieldType;
        }

        public string MakeSqlType()
        {
            return SqlFieldDescriptor.MakeSqlType(this.FieldType);
        }

        public SqlParameter MakeParam(T value)
        {
            return DbAccessor.MakeParam(64.ToString() + this.FieldName, (object)value, this.FieldType);
        }

        public T Read(IDataReader reader)
        {
            return reader.GetValue<T>(this.FieldName);
        }

        public T Read(IDataReader reader, T defaultValue)
        {
            return reader.GetValue<T>(this.FieldName, defaultValue);
        }
    }
}
