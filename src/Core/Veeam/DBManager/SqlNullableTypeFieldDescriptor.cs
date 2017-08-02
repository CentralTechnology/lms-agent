namespace Core.Veeam.DBManager
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using Common.Extensions;

    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed class SqlNullableTypeFieldDescriptor<TNullable, TStruct> : ISqlFieldDescriptor<TNullable>, ISqlFieldDescriptor where TStruct : struct
    {
        public SqlNullableTypeFieldDescriptor(string fieldName, SqlDbType fieldType)
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

        public SqlParameter MakeParam(TNullable value)
        {
            return DbAccessor.MakeParam(64 + FieldName, value, FieldType);
        }

        public TNullable Read(IDataReader reader)
        {
            throw new NotImplementedException();
            //return (TNullable)reader.GetNullable(FieldName, new TStruct?());
        }

        public TNullable Read(IDataReader reader, TNullable defaultValue)
        {
            throw new NotImplementedException();
            //return (TNullable)(ValueType)reader.GetNullable(FieldName, (TStruct?)(object)defaultValue);
        }
    }
}