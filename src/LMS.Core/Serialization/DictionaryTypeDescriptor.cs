namespace LMS.Core.Serialization
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal sealed class DictionaryTypeDescriptor : TypeDescriptor
    {
        private readonly ConstructorAccessor<int> _constructorAccessor;

        public DictionaryTypeDescriptor(
            Type type,
            TypeDescriptor keyDescriptor,
            TypeDescriptor valueDescriptor)
            : base(type, SerializationRoute.Dictionary, keyDescriptor, valueDescriptor)
        {
            this._constructorAccessor = DictionaryTypeDescriptor.CreateConstructor(type);
        }

        public IDictionary CreateDictionary(int capacity)
        {
            return (IDictionary) this._constructorAccessor.Create(capacity);
        }

        private static ConstructorAccessor<int> CreateConstructor(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(new Type[1]
            {
                ReflectionTypes.Int32
            });
            if (constructor == (ConstructorInfo) null)
                throw new NotSupportedException("Cannot find an expected constructor: public Dictionary(Int32 capacity).");
            ParameterInfo parameter = constructor.GetParameters()[0];
            if (parameter.Name != "capacity")
                throw new NotSupportedException("An unexpected constructor occured: public Dictionary(Int32 " + parameter.Name + ").");
            return new ConstructorAccessor<int>(constructor);
        }
    }
}
