using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    internal static class Unwrapper
    {
        private static readonly ConcurrentDictionary<Type, Func<object, object>> TypeToUnwrapper = new ConcurrentDictionary<Type, Func<object, object>>();

        public static Func<object, object> ProvideUnwrapper(Type type)
        {
            return Unwrapper.TypeToUnwrapper.GetOrAdd(type, new Func<Type, Func<object, object>>(Unwrapper.UnwrapAndWrap));
        }

        public static Func<object, object> UnwrapAndWrap(Type unwrappingType)
        {
            ParameterExpression parameterExpression = Expression.Parameter(ReflectionTypes.Object);
            return Expression.Lambda<Func<object, object>>((Expression) Expression.Convert((Expression) Expression.Convert((Expression) parameterExpression, unwrappingType), ReflectionTypes.Object), parameterExpression).Compile();
        }
    }
}
