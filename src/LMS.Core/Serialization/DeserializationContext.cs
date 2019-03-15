using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serialization
{
    internal sealed class DeserializationContext
    {
        private readonly Dictionary<int, object> _deserializedObjects;
        public readonly ushort Version;

        public DeserializationContext(ushort version)
        {
            this._deserializedObjects = new Dictionary<int, object>(7);
            this.Version = version;
        }

        public void AddDeserializedObject(object value)
        {
            this._deserializedObjects.Add(this._deserializedObjects.Count, value);
        }

        public object GetValueByReference(int reference)
        {
            return this._deserializedObjects[reference];
        }
    }
}
