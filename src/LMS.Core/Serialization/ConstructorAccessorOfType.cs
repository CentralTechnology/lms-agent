using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    internal sealed class ConstructorAccessor<T>
    {
        private readonly Func<T, object> _creator;

        public ConstructorAccessor(ConstructorInfo constructorInfo)
        {
            this._creator = ConstructorAccessor<T>.CreateCreator(constructorInfo);
        }

        public object Create(T arg)
        {
            return this._creator(arg);
        }

        private static Func<T, object> CreateCreator(ConstructorInfo constructorInfo)
        {
            ParameterExpression parameterExpression;
            return Expression.Lambda<Func<T, object>>((Expression) Expression.Convert((Expression) Expression.New(constructorInfo, (Expression) parameterExpression), ReflectionTypes.Object), parameterExpression).Compile();
        }
    }
}
