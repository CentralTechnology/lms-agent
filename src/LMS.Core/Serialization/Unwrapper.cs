namespace LMS.Core.Serialization
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;

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
