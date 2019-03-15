using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public sealed class ConcurrentLockProvider<TKey>
    {
        private readonly Dictionary<TKey, ConcurrentLockProvider<TKey>.Lock> _locks;

        public ConcurrentLockProvider()
            : this((IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
        {
        }

        public ConcurrentLockProvider(IEqualityComparer<TKey> comparer)
        {
            this._locks = new Dictionary<TKey, ConcurrentLockProvider<TKey>.Lock>(comparer);
        }

        public IDisposable Acquire(TKey key)
        {
            ConcurrentLockProvider<TKey>.Lock @lock;
            lock (this._locks)
            {
                if (!this._locks.TryGetValue(key, out @lock))
                {
                    @lock = new ConcurrentLockProvider<TKey>.Lock(key, this._locks);
                    this._locks[key] = @lock;
                }
                Interlocked.Increment(ref @lock.Counter);
            }
            Monitor.Enter((object) @lock);
            return (IDisposable) @lock;
        }

        private sealed class Lock : IDisposable
        {
            private readonly TKey _key;
            private readonly Dictionary<TKey, ConcurrentLockProvider<TKey>.Lock> _dic;
            public long Counter;

            public Lock(
                TKey key,
                Dictionary<TKey, ConcurrentLockProvider<TKey>.Lock> locks)
            {
                this._key = key;
                this._dic = locks;
            }

            public void Dispose()
            {
                Monitor.Exit((object) this);
                if (Interlocked.Decrement(ref this.Counter) >= 1L)
                    return;
                lock (this._dic)
                {
                    if (Interlocked.Read(ref this.Counter) >= 1L)
                        return;
                    this._dic.Remove(this._key);
                }
            }
        }
    }
}
