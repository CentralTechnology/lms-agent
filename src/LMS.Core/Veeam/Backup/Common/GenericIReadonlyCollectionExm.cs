using System;
using System.Collections.Generic;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class GenericIReadonlyCollectionExm
    {
        public static bool IsNullOrEmpty<TEntry>(this IReadOnlyCollection<TEntry> collection)
        {
            if (collection != null)
                return collection.Count == 0;
            return true;
        }

        public static T2[] ToArray<T1, T2>(
            this IReadOnlyCollection<T1> collection,
            Func<T1, T2> selector)
        {
            if (collection == null)
                return (T2[]) null;
            int num = 0;
            T2[] objArray = new T2[collection.Count];
            foreach (T1 obj in (IEnumerable<T1>) collection)
                objArray[num++] = selector(obj);
            return objArray;
        }
    }
}
