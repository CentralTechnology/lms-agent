using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
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
