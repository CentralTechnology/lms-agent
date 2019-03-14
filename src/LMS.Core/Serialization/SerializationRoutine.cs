using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
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
