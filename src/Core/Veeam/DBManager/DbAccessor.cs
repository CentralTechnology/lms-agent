namespace Core.Veeam.DBManager
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class DbAccessor
    {
        public static SqlParameter MakeParam(string name, object value)
        {
            var sqlParameter = new SqlParameter
            {
                ParameterName = name,
                Value = value ?? DBNull.Value
            };
            return sqlParameter;
        }

        public static SqlParameter MakeParam(string name, object value, SqlDbType type)
        {
            var sqlParameter = new SqlParameter(name, type) {Value = value ?? DBNull.Value};
            return sqlParameter;
        }
    }
}