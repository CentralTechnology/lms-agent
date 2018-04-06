namespace LMS.Veeam.DBManager
{
    using System.Data;
    using System.Data.SqlClient;
    using Core.Extensions;

    internal class SqlValueTypeFieldDescriptor<T> : ISqlFieldDescriptor<T> where T : struct
    {
        public SqlValueTypeFieldDescriptor(string fieldName, SqlDbType fieldType)
        {
            FieldName = fieldName;
            FieldType = fieldType;
        }

        public string FieldName { get; private set; }

        public SqlDbType FieldType { get; private set; }

        public string MakeSqlType()
        {
            return SqlFieldDescriptor.MakeSqlType(FieldType);
        }

        public SqlParameter MakeParam(T value)
        {
            return DbAccessor.MakeParam(64 + FieldName, value, FieldType);
        }

        public T Read(IDataReader reader)
        {
            return reader.GetValue<T>(FieldName);
        }

        public T Read(IDataReader reader, T defaultValue)
        {
            return reader.GetValue(FieldName, defaultValue);
        }
    }
}