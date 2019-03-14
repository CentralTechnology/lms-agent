using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public sealed class ConcurrentSet<T> : ConcurrentDictionary<T, T>
    {
        public ConcurrentSet()
        {
        }

        public ConcurrentSet(IEqualityComparer<T> comparer)
            : base(comparer)
        {
        }

        public bool Add(T value)
        {
            return this.TryAdd(value, value);
        }

        public bool TryAdd(T value)
        {
            return this.TryAdd(value, value);
        }

        public T AddIfNotPresent(T value, Action add)
        {
            return this.AddOrUpdate(value, (Func<T, T>) (key =>
            {
                add();
                return key;
            }), (Func<T, T, T>) ((key, val) => val));
        }

        public T GetOrAdd(T value)
        {
            return this.GetOrAdd(value, value);
        }

        public bool Contains(T value)
        {
            return this.ContainsKey(value);
        }

        public bool Remove(T value)
        {
            T obj;
            return this.TryRemove(value, out obj);
        }
    }
}
