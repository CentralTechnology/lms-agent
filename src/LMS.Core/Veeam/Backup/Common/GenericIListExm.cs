using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class GenericIListExm
    {
        private static readonly ThreadSafeRandom s_random = new ThreadSafeRandom();

        public static EnumeratorEx<IList<TEntry>, TEntry> GetListEnumeratorEx<TEntry>(
            this IList<TEntry> collection)
        {
            return new EnumeratorEx<IList<TEntry>, TEntry>(collection);
        }

        public static bool IsEmpty<TEntry>(this IList<TEntry> collection)
        {
            return collection.Count == 0;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            if (list.Count < 2)
                return;
            int count = list.Count;
            while (count > 1)
            {
                --count;
                int index = GenericIListExm.s_random.Next(count + 1);
                T obj = list[index];
                list[index] = list[count];
                list[count] = obj;
            }
        }

        public static void EnsureCapacity<T>(this List<T> list, int desiredCapacity)
        {
            if (list.Capacity - list.Count >= desiredCapacity)
                return;
            list.Capacity += desiredCapacity;
        }
    }
}
