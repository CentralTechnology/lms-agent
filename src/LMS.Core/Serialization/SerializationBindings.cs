using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    internal sealed class SerializationBindings
    {
        private readonly Dictionary<string, SerializationBinding> _dic;
        private readonly SerializationBinding[] _array;

        public SerializationBindings(Dictionary<string, SerializationBinding> dic)
        {
            this._dic = new Dictionary<string, SerializationBinding>((IDictionary<string, SerializationBinding>) dic);
            this._array = dic.Values.OrderBy<SerializationBinding, ushort>((Func<SerializationBinding, ushort>) (a => a.Index)).ToArray<SerializationBinding>();
        }

        public SerializationBinding FindBinding(
            ushort readedIndex,
            ref ushort currentIndex)
        {
            while ((int) currentIndex <= (int) readedIndex && (int) currentIndex < this._array.Length)
            {
                SerializationBinding serializationBinding = this._array[(int) currentIndex];
                if ((int) serializationBinding.Index > (int) readedIndex)
                    return (SerializationBinding) null;
                ++currentIndex;
                if ((int) serializationBinding.Index == (int) readedIndex)
                    return serializationBinding;
            }
            return (SerializationBinding) null;
        }

        public SerializationBinding FindBinding(string name)
        {
            SerializationBinding serializationBinding;
            if (this._dic.TryGetValue(name, out serializationBinding))
                return serializationBinding;
            return (SerializationBinding) null;
        }

        public IEnumerable<SerializationBinding> EnumerateSerializableBindings()
        {
            return ((IEnumerable<SerializationBinding>) this._array).Where<SerializationBinding>((Func<SerializationBinding, bool>) (b => (b.Options & SerializationOptions.Exclude) != SerializationOptions.Exclude));
        }
    }
}
