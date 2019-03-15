namespace LMS.Core.Serialization
{
    using System.Reflection;

    internal sealed class PropertyReader : IExpressionAccessor
    {
        private readonly GetValue _getter;

        public PropertyReader(PropertyInfo propertyInfo)
        {
            this._getter = Expressions.CreateGetter(propertyInfo.DeclaringType, propertyInfo.Name);
        }

        public void Set(ref object obj, object value)
        {
        }

        public object Get(object obj)
        {
            return this._getter(obj);
        }
    }
}
