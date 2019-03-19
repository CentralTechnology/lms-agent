namespace LMS.Core.Serialization
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

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
            return Expression.Lambda<Func<T, object>>((Expression) Expression.Convert((Expression) Expression.New(constructorInfo, (Expression) null), ReflectionTypes.Object), (ParameterExpression) null).Compile();
        }
    }
}
