namespace LMS.Core.Serialization
{
    using System;
    using System.Collections.Generic;

    internal sealed class SerializationBindingsBuilder
    {
        private readonly HashSet<ushort> _indexToBinding = new HashSet<ushort>();
        private readonly Dictionary<string, SerializationBinding> _nameToBinding = new Dictionary<string, SerializationBinding>(16);

        public void Add(SerializationBinding binding)
        {
            if (!this._indexToBinding.Add(binding.Index))
                throw new ArgumentException(string.Format("A binding with index {0} has already been added.", (object) binding.Index));
            this._nameToBinding.Add(binding.Name, binding);
        }

        public SerializationBindings Build()
        {
            return new SerializationBindings(this._nameToBinding);
        }
    }
}
