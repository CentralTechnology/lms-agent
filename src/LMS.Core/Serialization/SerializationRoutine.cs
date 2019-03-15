namespace LMS.Core.Serialization
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.Serialization;

    internal static class SerializationRoutine
    {
        private static readonly ConcurrentDictionary<Type, ISerializationRoutine> TypeToConverter = new ConcurrentDictionary<Type, ISerializationRoutine>();

        public static ISerializationRoutine ProvideConverter(
            Type serializationRoutine)
        {
            return SerializationRoutine.TypeToConverter.GetOrAdd(serializationRoutine, new Func<Type, ISerializationRoutine>(SerializationRoutine.CreateConverter));
        }

        private static ISerializationRoutine CreateConverter(
            Type serializationRoutine)
        {
            return (ISerializationRoutine) FormatterServices.GetUninitializedObject(serializationRoutine);
        }
    }
}
