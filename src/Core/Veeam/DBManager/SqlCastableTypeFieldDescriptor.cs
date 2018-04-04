namespace LMS.Veeam.DBManager
{
    using System.Data;
    using System.Data.SqlClient;
    using global::Core.Common;

    public sealed class SqlCastableTypeFieldDescriptor<TExternal, TInternal> : ISqlFieldDescriptor<TExternal>, ISqlFieldDescriptor
    {
        private readonly ISqlFieldDescriptor<TInternal> _internal;

        public SqlCastableTypeFieldDescriptor(ISqlFieldDescriptor<TInternal> internalDescriptor)
        {
            _internal = internalDescriptor;
        }

        public string FieldName => _internal.FieldName;

        public SqlDbType FieldType => _internal.FieldType;

        public string MakeSqlType()
        {
            return _internal.MakeSqlType();
        }

        public SqlParameter MakeParam(TExternal value)
        {
            return _internal.MakeParam(Caster<TExternal, TInternal>.Cast(value));
        }

        public TExternal Read(IDataReader reader)
        {
            TInternal @internal = _internal.Read(reader);
            return Caster<TInternal, TExternal>.Cast(@internal);
        }

        public TExternal Read(IDataReader reader, TExternal defaultValue)
        {
            TInternal defaultValue1 = Caster<TExternal, TInternal>.Cast(defaultValue);
            TInternal @internal = _internal.Read(reader, defaultValue1);
            return Caster<TInternal, TExternal>.Cast(@internal);
        }
    }
}