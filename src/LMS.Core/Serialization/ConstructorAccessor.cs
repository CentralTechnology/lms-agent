namespace LMS.Core.Serialization
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;

    internal sealed class ConstructorAccessor
    {
        private readonly Func<object> _creator;

        public ConstructorAccessor(Type type, ConstructorInfo constructorInfo)
        {
            this._creator = ConstructorAccessor.CreateCreator(type, constructorInfo);
        }

        public object Create()
        {
            return this._creator();
        }

        private static Func<object> CreateCreator(Type type, ConstructorInfo constructorInfo)
        {
            if (constructorInfo == (ConstructorInfo) null)
                return (Func<object>) (() => FormatterServices.GetUninitializedObject(type));
            return Expression.Lambda<Func<object>>((Expression) Expression.Convert(Expression.New(constructorInfo), ReflectionTypes.Object), new ParameterExpression[0]).Compile();
        }
    }
}
