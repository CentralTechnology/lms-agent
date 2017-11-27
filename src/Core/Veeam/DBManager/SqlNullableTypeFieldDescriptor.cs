namespace LMS.Veeam.DBManager
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using Core.Common.Extensions;

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

        /// <inheritdoc />
        public TNullable Read(IDataReader reader)
        {
            object value = reader.GetNullable<TStruct?>(FieldName);

            Type t = typeof(TNullable);
            t = Nullable.GetUnderlyingType(t) ?? t;

            return value == null ? default(TNullable) : (TNullable) Convert.ChangeType(value, t);
        }

        /// <inheritdoc />
        public TNullable Read(IDataReader reader, TNullable defaultValue)
        {
            throw new NotImplementedException();
            //var val = reader.GetNullable(FieldName, (TStruct?)(object)defaultValue);
            //return (TNullable) Convert.ChangeType(val, typeof(TNullable));
        }
    }
}