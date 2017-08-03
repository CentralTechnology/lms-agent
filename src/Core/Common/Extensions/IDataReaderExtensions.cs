namespace Core.Common.Extensions
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using Abp;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class IDataReaderExtensions
    {
        public static T GetClass<T>(this SqlDataReader self, Func<SqlDataReader, T> reader)
        {
            return reader(self);
        }

        public static T GetClass<T>(this IDataReader self, Func<IDataReader, T> reader)
        {
            return reader(self);
        }

        public static T GetClass<T>(this IDataReader self, string name) where T : class
        {
            object obj = self[name];
            if (obj == Convert.DBNull)
                return default(T);
            return (T)obj;
        }

        public static T GetNullable<T>(this IDataReader self, string name, T defaultValue) where T : struct
        {
            object obj = self[name];
            if (obj == Convert.DBNull)
                return defaultValue;
            return (T)obj;
        }

        public static T GetNullable<T>(this IDataReader self, string name)
        {
            object value = self[name];

            Type t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;

            return (value == null || DBNull.Value.Equals(value)) ? default(T) : (T) Convert.ChangeType(value, t);
        }

        public static T GetValue<T>(this IDataReader self, string name) where T : struct
        {
            object obj = self[name];
            if (obj == Convert.DBNull)
                throw new AbpException($"Unexpected DBNull value. [Name: {name}]");
            return (T)obj;
        }

        public static T GetValue<T>(this IDataReader self, string name, T defaultValue) where T : struct
        {
            object obj = self[name];
            if (obj == Convert.DBNull)
                return defaultValue;
            return (T)obj;
        }
    }
}