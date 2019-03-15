namespace LMS.Core.Serialization
{
    using System.Collections.Generic;

    internal sealed class SerializationContext
    {
        private readonly Dictionary<object, int> _serializedObjects;

        public SerializationContext()
        {
            this._serializedObjects = new Dictionary<object, int>(7);
        }

        public bool TryGetReference(object value, out int reference)
        {
            return this._serializedObjects.TryGetValue(value, out reference);
        }

        public void AddSerializedObject(object value)
        {
            int count = this._serializedObjects.Count;
            this._serializedObjects.Add(value, count);
        }
    }
}
