namespace LMS.Core.Serialization
{
    using System;
    using System.Collections.Generic;

    internal sealed class DescriptorInitializationStack
    {
        private readonly Dictionary<Type, SerializableTypeDescriptor> _typeDescriptors = new Dictionary<Type, SerializableTypeDescriptor>();
        public DescriptorGenerationRules GenerationRules;

        public void Add(Type type, SerializableTypeDescriptor self)
        {
            this._typeDescriptors.Add(type, self);
        }

        public void Remove(Type type)
        {
            this._typeDescriptors.Remove(type);
        }

        public bool TryGetPendingDescriptor(Type type, out SerializableTypeDescriptor descriptor)
        {
            return this._typeDescriptors.TryGetValue(type, out descriptor);
        }
    }
}
