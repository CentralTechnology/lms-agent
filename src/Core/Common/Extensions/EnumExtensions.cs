namespace Core.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using Enum;

    public static class EnumExtensions
    {
        public static Monitor ClearFlag(Monitor value, Monitor flag)
        {
            return value & ~flag;
        }

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
    }
}