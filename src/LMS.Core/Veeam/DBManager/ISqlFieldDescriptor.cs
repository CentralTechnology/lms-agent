namespace LMS.Core.Veeam.DBManager
{
    using System.Data;

    public interface ISqlFieldDescriptor
    {
        string FieldName { get; }

        SqlDbType FieldType { get; }

        string MakeSqlType();
    }
}