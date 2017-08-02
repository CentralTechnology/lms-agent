namespace Core.Veeam.DBManager
{
    using System;
    using System.Data;

    public sealed class SqlExtendableReferenceTypeFieldDescriptor<T> : SqlReferenceTypeFieldDescriptor<T> where T : class
    {
        private readonly int MaxSize;

        public SqlExtendableReferenceTypeFieldDescriptor(string fieldName, SqlDbType fieldType, int maxSize)
            : base(fieldName, fieldType)
        {
            MaxSize = maxSize;
        }

        public override string MakeSqlType()
        {
            switch (FieldType)
            {
                case SqlDbType.NVarChar:
                    return "nvarchar(" + MaxSize + ")";
                case SqlDbType.VarBinary:
                    return "varbinary(" + MaxSize + ")";
                case SqlDbType.VarChar:
                    return "varchar(" + MaxSize + ")";
                default:
                    throw new NotSupportedException(FieldType.ToString());
            }
        }
    }
}