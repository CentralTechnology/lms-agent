namespace LMS.Core.Serialization
{
    using System.Reflection;

    internal sealed class PropertyWriter : IExpressionAccessor
    {
        private readonly SetValue _setter;

        public PropertyWriter(PropertyInfo propertyInfo)
        {
            this._setter = Expressions.CreatePropertySetter(propertyInfo.DeclaringType, propertyInfo.Name, propertyInfo.PropertyType);
        }

        public void Set(ref object obj, object value)
        {
            this._setter(ref obj, value);
        }

        public object Get(object obj)
        {
            return (object) null;
        }
    }
}
