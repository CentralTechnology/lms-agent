using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class SArrayExtensions
    {
        public static void Fill<T>(this T[] array, T value)
        {
            for (int index = 0; index < array.Length; ++index)
                array[index] = value;
        }

        public static T[] Create<T>(T value, int length)
        {
            T[] objArray = new T[length];
            for (int index = 0; index < length; ++index)
                objArray[index] = value;
            return objArray;
        }

        public static T[] Copy<T>(this T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof (array));
            return (T[]) array.Clone();
        }

        public static T[] Empty<T>()
        {
            return SArrayExtensions.SEmptyArray<T>.Value;
        }

        private static class SEmptyArray<T>
        {
            public static readonly T[] Value = new T[0];
        }
    }
}
