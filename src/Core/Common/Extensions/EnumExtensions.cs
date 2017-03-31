﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    using Enum;
    using Enum = System.Enum;

    public static class EnumExtensions
    {
        public static IEnumerable<Monitor> GetFlags(this Monitor input)
        {
            foreach (Monitor item in Enum.GetValues(input.GetType()))
            {
                if (input.HasFlag(item))
                {
                    yield return item;
                }
            }
        }
        public static Monitor ClearFlag(Monitor value, Monitor flag)
        {
            return value & ~flag;
        }
    }
}