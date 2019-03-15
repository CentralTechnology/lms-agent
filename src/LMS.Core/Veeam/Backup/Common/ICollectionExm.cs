using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class ICollectionExm
    {
        public static void AddRange<TValue>(
            this ICollection<TValue> collection,
            IEnumerable<TValue> enumerable)
        {
            foreach (TValue obj in enumerable)
                collection.Add(obj);
        }

        public static ICollection<T> Random<T>(this ICollection<T> collection)
        {
            if (collection.Count < 2)
                return collection;
            List<T> list = new List<T>((IEnumerable<T>) collection);
            list.Shuffle<T>();
            return (ICollection<T>) list;
        }

        public static IReadOnlyList<T> RandomUnion<T>(params ICollection<T>[] collections)
        {
            if (((IReadOnlyCollection<ICollection<T>>) collections).IsNullOrEmpty<ICollection<T>>())
                return (IReadOnlyList<T>) new T[0];
            int capacity = 0;
            foreach (ICollection<T> collection in collections)
            {
                if (collection != null)
                    capacity += collection.Count;
            }
            List<T> list = new List<T>(capacity);
            foreach (ICollection<T> collection in collections)
            {
                if (collection != null)
                    list.AddRange((IEnumerable<T>) collection);
            }
            list.Shuffle<T>();
            return (IReadOnlyList<T>) list;
        }
    }
}
