namespace Core.Veeam.DBManager
{
    using System;
    using System.Data;

    public static class SqlFieldDescriptor
    {
        public static ISqlFieldDescriptor<bool> Bit(string fieldName)
        {
            return new SqlValueTypeFieldDescriptor<bool>(fieldName, SqlDbType.Bit);
        }

        public static ISqlFieldDescriptor<bool?> BitNullable(string fieldName)
        {
            return new SqlNullableTypeFieldDescriptor<bool?, bool>(fieldName, SqlDbType.Bit);
        }

        private static void CheckEnumTypeCompatibility<T>(SqlDbType dbType)
        {
            CheckPrimitiveTypeCompatibility(Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))), dbType);
        }

        private static void CheckPrimitiveTypeCompatibility(TypeCode typeCode, SqlDbType dbType)
        {
            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                    if (dbType == SqlDbType.TinyInt)
                        return;
                    break;
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    if (dbType == SqlDbType.SmallInt)
                        return;
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    if (dbType == SqlDbType.Int)
                        return;
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    if (dbType == SqlDbType.BigInt)
                        return;
                    break;
            }
            throw new NotSupportedException($"Enum underlying type: {(object) typeCode}, Database field type: {(object) dbType}");
        }       

        public static ISqlFieldDescriptor<int> Int(string fieldName)
        {
            return new SqlValueTypeFieldDescriptor<int>(fieldName, SqlDbType.Int);
        }

        public static ISqlFieldDescriptor<T> IntEnum<T>(string fieldName) where T : struct
        {
            CheckEnumTypeCompatibility<T>(SqlDbType.Int);
            return new SqlCastableTypeFieldDescriptor<T, int>(Int(fieldName));
        }

        public static ISqlFieldDescriptor<int?> IntNullable(string fieldName)
        {
            return new SqlNullableTypeFieldDescriptor<int?, int>(fieldName, SqlDbType.Int);
        }

        public static ISqlFieldDescriptor<T?> IntNullableEnum<T>(string fieldName) where T : struct
        {
            CheckEnumTypeCompatibility<T>(SqlDbType.Int);
            return new SqlCastableTypeFieldDescriptor<T?, int?>(IntNullable(fieldName));
        }

        public static string MakeSqlType(SqlDbType fieldType)
        {
            return fieldType.ToString().ToLower();
        }

        public static ISqlFieldDescriptor<DateTime?> DateTimeNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<DateTime?>)new SqlNullableTypeFieldDescriptor<DateTime?, DateTime>(fieldName, SqlDbType.DateTime);
        }

        public static ISqlFieldDescriptor<Guid> UniqueIdentifier(string fieldName)
        {
            return (ISqlFieldDescriptor<Guid>)new SqlValueTypeFieldDescriptor<Guid>(fieldName, SqlDbType.UniqueIdentifier);
        }
    }
}