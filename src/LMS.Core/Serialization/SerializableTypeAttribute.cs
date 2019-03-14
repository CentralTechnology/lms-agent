﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class SerializableTypeAttribute : Attribute
    {
        public readonly Guid Id;

        public Type As { get; set; }

        public Type Routine { get; set; }

        public SerializableTypeAttribute()
        {
        }

        public SerializableTypeAttribute(string id)
        {
            this.Id = Guid.Parse(id);
        }
    }
}
