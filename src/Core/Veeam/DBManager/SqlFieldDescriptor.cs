using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Veeam.DBManager
{
    using System.Data;

    public static class SqlFieldDescriptor
    {
        public static ISqlFieldDescriptor<Guid> UniqueIdentifier(string fieldName)
        {
            return (ISqlFieldDescriptor<Guid>)new SqlValueTypeFieldDescriptor<Guid>(fieldName, SqlDbType.UniqueIdentifier);
        }

        public static ISqlFieldDescriptor<byte> TinyInt(string fieldName)
        {
            return (ISqlFieldDescriptor<byte>)new SqlValueTypeFieldDescriptor<byte>(fieldName, SqlDbType.TinyInt);
        }

        public static ISqlFieldDescriptor<short> SmallInt(string fieldName)
        {
            return (ISqlFieldDescriptor<short>)new SqlValueTypeFieldDescriptor<short>(fieldName, SqlDbType.SmallInt);
        }

        public static ISqlFieldDescriptor<int> Int(string fieldName)
        {
            return (ISqlFieldDescriptor<int>)new SqlValueTypeFieldDescriptor<int>(fieldName, SqlDbType.Int);
        }

        public static ISqlFieldDescriptor<long> BigInt(string fieldName)
        {
            return (ISqlFieldDescriptor<long>)new SqlValueTypeFieldDescriptor<long>(fieldName, SqlDbType.BigInt);
        }

        public static ISqlFieldDescriptor<float> Real(string fieldName)
        {
            return (ISqlFieldDescriptor<float>)new SqlValueTypeFieldDescriptor<float>(fieldName, SqlDbType.Real);
        }

        public static ISqlFieldDescriptor<double> Float(string fieldName)
        {
            return (ISqlFieldDescriptor<double>)new SqlValueTypeFieldDescriptor<double>(fieldName, SqlDbType.Float);
        }

        public static ISqlFieldDescriptor<Decimal> Decimal(string fieldName)
        {
            return (ISqlFieldDescriptor<Decimal>)new SqlValueTypeFieldDescriptor<Decimal>(fieldName, SqlDbType.Decimal);
        }

        public static ISqlFieldDescriptor<bool> Bit(string fieldName)
        {
            return (ISqlFieldDescriptor<bool>)new SqlValueTypeFieldDescriptor<bool>(fieldName, SqlDbType.Bit);
        }

        public static ISqlFieldDescriptor<DateTime> DateTime(string fieldName)
        {
            return (ISqlFieldDescriptor<DateTime>)new SqlValueTypeFieldDescriptor<DateTime>(fieldName, SqlDbType.DateTime);
        }

        public static ISqlFieldDescriptor<Guid?> UniqueIdentifierNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<Guid?>)new SqlNullableTypeFieldDescriptor<Guid?, Guid>(fieldName, SqlDbType.UniqueIdentifier);
        }

        public static ISqlFieldDescriptor<byte?> TinyIntNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<byte?>)new SqlNullableTypeFieldDescriptor<byte?, byte>(fieldName, SqlDbType.TinyInt);
        }

        public static ISqlFieldDescriptor<short?> SmallIntNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<short?>)new SqlNullableTypeFieldDescriptor<short?, short>(fieldName, SqlDbType.SmallInt);
        }

        public static ISqlFieldDescriptor<int?> IntNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<int?>)new SqlNullableTypeFieldDescriptor<int?, int>(fieldName, SqlDbType.Int);
        }

        public static ISqlFieldDescriptor<long?> BigIntNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<long?>)new SqlNullableTypeFieldDescriptor<long?, long>(fieldName, SqlDbType.BigInt);
        }

        public static ISqlFieldDescriptor<float?> RealNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<float?>)new SqlNullableTypeFieldDescriptor<float?, float>(fieldName, SqlDbType.Real);
        }

        public static ISqlFieldDescriptor<double?> FloatNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<double?>)new SqlNullableTypeFieldDescriptor<double?, double>(fieldName, SqlDbType.Float);
        }

        public static ISqlFieldDescriptor<Decimal?> DecimalNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<Decimal?>)new SqlNullableTypeFieldDescriptor<Decimal?, Decimal>(fieldName, SqlDbType.Decimal);
        }

        public static ISqlFieldDescriptor<bool?> BitNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<bool?>)new SqlNullableTypeFieldDescriptor<bool?, bool>(fieldName, SqlDbType.Bit);
        }

        public static ISqlFieldDescriptor<DateTime?> DateTimeNullable(string fieldName)
        {
            return (ISqlFieldDescriptor<DateTime?>)new SqlNullableTypeFieldDescriptor<DateTime?, DateTime>(fieldName, SqlDbType.DateTime);
        }

        public static ISqlFieldDescriptor<string> Xml(string fieldName)
        {
            return (ISqlFieldDescriptor<string>)new SqlReferenceTypeFieldDescriptor<string>(fieldName, SqlDbType.Xml);
        }

        public static ISqlFieldDescriptor<string> NChar(string fieldName)
        {
            return (ISqlFieldDescriptor<string>)new SqlReferenceTypeFieldDescriptor<string>(fieldName, SqlDbType.NChar);
        }

        public static ISqlFieldDescriptor<string> NVarChar(string fieldName)
        {
            return (ISqlFieldDescriptor<string>)new SqlReferenceTypeFieldDescriptor<string>(fieldName, SqlDbType.NVarChar);
        }

        public static ISqlFieldDescriptor<string> NVarChar(string fieldName, int maxSize)
        {
            if (maxSize < 1 || maxSize > 4000)
                throw new ArgumentOutOfRangeException("maxSize", (object)maxSize, "Unicode string length must be a value from 1 through 4000.");
            return (ISqlFieldDescriptor<string>)new SqlExtendableReferenceTypeFieldDescriptor<string>(fieldName, SqlDbType.NVarChar, maxSize);
        }

        public static ISqlFieldDescriptor<T> TinyIntEnum<T>(string fieldName) where T : struct
        {
            SqlFieldDescriptor.CheckEnumTypeCompatibility<T>(SqlDbType.TinyInt);
            return (ISqlFieldDescriptor<T>)new SqlCastableTypeFieldDescriptor<T, byte>(SqlFieldDescriptor.TinyInt(fieldName));
        }

        public static ISqlFieldDescriptor<T> SmallIntEnum<T>(string fieldName) where T : struct
        {
            SqlFieldDescriptor.CheckEnumTypeCompatibility<T>(SqlDbType.SmallInt);
            return (ISqlFieldDescriptor<T>)new SqlCastableTypeFieldDescriptor<T, short>(SqlFieldDescriptor.SmallInt(fieldName));
        }

        public static ISqlFieldDescriptor<T> IntEnum<T>(string fieldName) where T : struct
        {
            SqlFieldDescriptor.CheckEnumTypeCompatibility<T>(SqlDbType.Int);
            return (ISqlFieldDescriptor<T>)new SqlCastableTypeFieldDescriptor<T, int>(SqlFieldDescriptor.Int(fieldName));
        }

        public static ISqlFieldDescriptor<T> BigIntEnum<T>(string fieldName) where T : struct
        {
            SqlFieldDescriptor.CheckEnumTypeCompatibility<T>(SqlDbType.BigInt);
            return (ISqlFieldDescriptor<T>)new SqlCastableTypeFieldDescriptor<T, long>(SqlFieldDescriptor.BigInt(fieldName));
        }

        public static ISqlFieldDescriptor<T?> TinyIntNullableEnum<T>(string fieldName) where T : struct
        {
            SqlFieldDescriptor.CheckEnumTypeCompatibility<T>(SqlDbType.TinyInt);
            return (ISqlFieldDescriptor<T?>)new SqlCastableTypeFieldDescriptor<T?, byte?>(SqlFieldDescriptor.TinyIntNullable(fieldName));
        }

        public static ISqlFieldDescriptor<T?> SmallIntNullableEnum<T>(string fieldName) where T : struct
        {
            SqlFieldDescriptor.CheckEnumTypeCompatibility<T>(SqlDbType.SmallInt);
            return (ISqlFieldDescriptor<T?>)new SqlCastableTypeFieldDescriptor<T?, short?>(SqlFieldDescriptor.SmallIntNullable(fieldName));
        }

        public static ISqlFieldDescriptor<T?> IntNullableEnum<T>(string fieldName) where T : struct
        {
            SqlFieldDescriptor.CheckEnumTypeCompatibility<T>(SqlDbType.Int);
            return (ISqlFieldDescriptor<T?>)new SqlCastableTypeFieldDescriptor<T?, int?>(SqlFieldDescriptor.IntNullable(fieldName));
        }

        public static ISqlFieldDescriptor<T?> BigIntNullableEnum<T>(string fieldName) where T : struct
        {
            SqlFieldDescriptor.CheckEnumTypeCompatibility<T>(SqlDbType.BigInt);
            return (ISqlFieldDescriptor<T?>)new SqlCastableTypeFieldDescriptor<T?, long?>(SqlFieldDescriptor.BigIntNullable(fieldName));
        }

        private static void CheckEnumTypeCompatibility<T>(SqlDbType dbType)
        {
            SqlFieldDescriptor.CheckPrimitiveTypeCompatibility(Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))), dbType);
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
            throw new NotSupportedException(string.Format("Enum underlying type: {0}, Database field type: {1}", (object)typeCode, (object)dbType));
        }

        public static string MakeSqlType(SqlDbType fieldType)
        {
            return fieldType.ToString().ToLower();
        }
    }
}
