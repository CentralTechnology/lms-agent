using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Common;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class EnumCache<T> where T : struct
    {
        public static readonly T[] Values = Enum.GetValues(TypeCache<T>.Type).Cast<T>().ToArray<T>();
        public static readonly string[] Names = Enum.GetNames(TypeCache<T>.Type);
        public static readonly TypeCode UnderlyingTypeCode = System.Type.GetTypeCode(TypeCache<T>.Type.GetEnumUnderlyingType());

        public static int Count
        {
            get
            {
                return EnumCache<T>.Values.Length;
            }
        }

        public static bool IsDefined(T value)
        {
            return ((IEnumerable<T>) EnumCache<T>.Values).Contains<T>(value);
        }

        public static T? TryParse(string name, StringComparison nameComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            for (int index = 0; index < EnumCache<T>.Count; ++index)
            {
                if (string.Equals(EnumCache<T>.Names[index], name, nameComparison))
                    return new T?(EnumCache<T>.Values[index]);
            }
            return new T?();
        }

        public static T Parse(string name, T defaulValue, StringComparison nameComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return EnumCache<T>.TryParse(name, nameComparison) ?? defaulValue;
        }

        public static T Parse(string name, StringComparison nameComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            T? nullable = EnumCache<T>.TryParse(name, nameComparison);
            if (!nullable.HasValue)
                throw ExceptionFactory.Create("Unknown value '{0}.{1}'", (object) TypeCache<T>.Type, (object) name);
            return nullable.Value;
        }
    }
}
