using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    internal sealed class SerializationBinding
    {
        public readonly TypeDescriptor TypeDescriptor;
        public readonly ushort Index;
        public readonly string Name;
        public readonly SerializationOptions Options;
        public readonly IExpressionAccessor Expression;

        public SerializationBinding(
            TypeDescriptor typeDescriptor,
            ushort index,
            string name,
            SerializationOptions options,
            IExpressionAccessor expression)
        {
            this.TypeDescriptor = typeDescriptor;
            this.Index = index;
            this.Name = name;
            this.Options = options;
            this.Expression = expression;
        }

        public bool IsNullable
        {
            get
            {
                return this.TypeDescriptor.IsNullable;
            }
        }

        public bool ValueIsNull(object value)
        {
            if (this.IsNullable)
                return object.ReferenceEquals(value, (object) null);
            return false;
        }
    }
}
