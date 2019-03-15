namespace LMS.Core.Serialization
{
    using System;

    internal static class ReflectionTypes
    {
        public static readonly Type Object = typeof (object);
        public static readonly Type ObjectRef = ReflectionTypes.Object.MakeByRefType();
        public static readonly Type ValueType = typeof (System.ValueType);
        public static readonly Type Nullable = typeof (System.Nullable<>);
        public static readonly Type List = typeof (System.Collections.Generic.List<>);
        public static readonly Type IList = typeof (System.Collections.IList);
        public static readonly Type Dictionary = typeof (System.Collections.Generic.Dictionary<,>);
        public static readonly Type IDictionary = typeof (System.Collections.IDictionary);
        public static readonly Type Int32 = typeof (int);
        public static readonly Type SerializableDataAttribute = typeof (SerializableDataAttribute);
        public static readonly Type SerializableTypeAttribute = typeof (SerializableTypeAttribute);
        public static readonly Type ISerializationRoutine = typeof (ISerializationRoutine<,>);
    }
}
