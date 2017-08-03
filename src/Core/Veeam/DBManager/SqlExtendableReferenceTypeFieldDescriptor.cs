namespace Core.Veeam.DBManager
{
    using System;
    using System.Data;

    public sealed class SqlExtendableReferenceTypeFieldDescriptor<T> : SqlReferenceTypeFieldDescriptor<T> where T : class
    {
        private readonly int _maxSize;

        public SqlExtendableReferenceTypeFieldDescriptor(string fieldName, SqlDbType fieldType, int maxSize)
            : base(fieldName, fieldType)
        {
            _maxSize = maxSize;
        }

        public override string MakeSqlType()
        {
            switch (FieldType)
            {
                case SqlDbType.NVarChar:
                    return "nvarchar(" + _maxSize + ")";
                case SqlDbType.VarBinary:
                    return "varbinary(" + _maxSize + ")";
                case SqlDbType.VarChar:
                    return "varchar(" + _maxSize + ")";
                default:
                    throw new NotSupportedException(FieldType.ToString());
            }
        }
    }
}