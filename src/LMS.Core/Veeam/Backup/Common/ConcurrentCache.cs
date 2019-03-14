using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
    public abstract class ConcurrentCache
    {
        private static readonly Timer DeleteQueueTimer = new Timer(new TimerCallback(ConcurrentCache.TimerCallback), (object) null, 0, 10000);
        protected static readonly ConcurrentBag<Action> TimerCallbacks = new ConcurrentBag<Action>();
        private const int TimerPeriod = 10000;

        public static ConcurrentCache<TKey, TValue> Create<TKey, TValue>(
            ConcurrentCacheFactory<TKey, TValue> factory)
        {
            return new ConcurrentCache<TKey, TValue>(factory);
        }

        public static ConcurrentCache<TKey, TValue> Create<TKey, TValue>(
            ConcurrentCacheFactory<TKey, TValue> factory,
            IEqualityComparer<TKey> comparer)
        {
            return new ConcurrentCache<TKey, TValue>(factory, comparer);
        }

        private static void TimerCallback(object state)
        {
            if (!Monitor.TryEnter((object) ConcurrentCache.DeleteQueueTimer))
                return;
            try
            {
                foreach (Action timerCallback in ConcurrentCache.TimerCallbacks)
                    timerCallback();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to perform timer callback.");
            }
            finally
            {
                Monitor.Exit((object) ConcurrentCache.DeleteQueueTimer);
            }
        }
    }
}
