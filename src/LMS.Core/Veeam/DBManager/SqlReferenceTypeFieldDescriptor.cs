using LMS.Core.Veeam.Backup.DBManager;

namespace LMS.Core.Veeam.DBManager
{
    using System.Data;
    using System.Data.SqlClient;
    using Extensions;

    public class SqlReferenceTypeFieldDescriptor<T> : ISqlFieldDescriptor<T>, ISqlFieldDescriptor where T : class
    {
        public SqlReferenceTypeFieldDescriptor(string fieldName, SqlDbType fieldType)
        {
            FieldName = fieldName;
            FieldType = fieldType;
        }

        public string FieldName { get; private set; }

        public SqlDbType FieldType { get; private set; }

        public virtual string MakeSqlType()
        {
            return SqlFieldDescriptor.MakeSqlType(FieldType);
        }

        public SqlParameter MakeParam(T value)
        {
            return DbAccessor.MakeParam(64 + FieldName, value, FieldType);
        }

        public T Read(IDataReader reader)
        {
            return reader.GetClass<T>(FieldName);
        }

        public T Read(IDataReader reader, T defaultValue)
        {
            var obj = reader.GetClass<T>(FieldName);
            if (obj != null)
                return obj;
            return defaultValue;
        }
    }
}