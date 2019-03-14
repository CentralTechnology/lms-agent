using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SerializableDataAttribute : Attribute
    {
        public ushort Index { get; private set; }

        public string Name { get; set; }

        public SerializationOptions Options { get; set; }

        public SerializableDataAttribute(ushort index)
        {
            this.Index = index;
        }

        public SerializableDataAttribute(ushort index, string name)
        {
            this.Index = index;
            this.Name = name;
        }
    }
}
