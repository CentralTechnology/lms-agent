namespace LMS.Core.Serialization
{
    using System.Reflection;

    internal sealed class FieldAccessor : IExpressionAccessor
    {
        private readonly GetValue _getter;
        private readonly SetValue _setter;

        public FieldAccessor(FieldInfo fieldInfo)
        {
            this._getter = Expressions.CreateGetter(fieldInfo.DeclaringType, fieldInfo.Name);
            this._setter = Expressions.CreateFieldSetter(fieldInfo);
        }

        public void Set(ref object instance, object value)
        {
            this._setter(ref instance, value);
        }

        public object Get(object instance)
        {
            return this._getter(instance);
        }
    }
}
