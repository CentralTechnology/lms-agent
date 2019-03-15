namespace LMS.Core.Serialization
{
    using System.Reflection;

    internal sealed class PropertyAccessor : IExpressionAccessor
    {
        private readonly GetValue _getter;
        private readonly SetValue _setter;

        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            this._getter = Expressions.CreateGetter(propertyInfo.DeclaringType, propertyInfo.Name);
            this._setter = Expressions.CreatePropertySetter(propertyInfo.DeclaringType, propertyInfo.Name, propertyInfo.PropertyType);
        }

        public void Set(ref object obj, object value)
        {
            this._setter(ref obj, value);
        }

        public object Get(object obj)
        {
            return this._getter(obj);
        }
    }
}
