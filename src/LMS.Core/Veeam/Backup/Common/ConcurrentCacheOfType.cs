using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public sealed class ConcurrentCache<TKey, TValue> : ConcurrentCache, IEnumerable<TValue>, IEnumerable, IDisposable
  {
    private readonly ConcurrentCacheFactory<TKey, TValue> _factory;
    private readonly ConcurrentDictionary<TKey, ConcurrentCache<TKey, TValue>.Item> _dic;
    private volatile ConcurrentLockProvider<TKey> _locks;
    private readonly ConcurrentSet<Task> _disposing;

    public bool ReleaseImmediately { get; set; }

    public ConcurrentCache(ConcurrentCacheFactory<TKey, TValue> factory)
      : this(factory, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
    {
    }

    public ConcurrentCache(
      ConcurrentCacheFactory<TKey, TValue> factory,
      IEqualityComparer<TKey> comparer)
    {
      this._factory = Exceptions.CheckArgumentNullException<ConcurrentCacheFactory<TKey, TValue>>(factory, nameof (factory));
      this._dic = new ConcurrentDictionary<TKey, ConcurrentCache<TKey, TValue>.Item>(comparer);
      this._locks = new ConcurrentLockProvider<TKey>(comparer);
      this._disposing = new ConcurrentSet<Task>();
    }

    public void Dispose()
    {
      foreach (TKey key in (IEnumerable<TKey>) this._dic.Keys)
        this.RemoveItem(key, true);
      Task.WaitAll(this._disposing.Values.ToArray<Task>(), 3600000);
    }

    public TValue Acquire(TKey key)
    {
      using (this._locks.Acquire(key))
      {
        ConcurrentCache<TKey, TValue>.Item obj;
        TValue result;
        do
        {
          obj = this.GetOrCreateItem(key);
        }
        while (!this.TryAcquire(key, obj, out result));
        return result;
      }
    }

    public IDisposable AcquireDisposable(TKey key, out TValue result)
    {
      result = this.Acquire(key);
      return (IDisposable) new DisposableAction((Action) (() => this.Release(key)));
    }

    public TValue AcquireIfExists(TKey key)
    {
      using (this._locks.Acquire(key))
      {
        ConcurrentCache<TKey, TValue>.Item obj;
        TValue result;
        do
        {
          obj = this.FindItem(key);
          if (obj == null)
            return default (TValue);
        }
        while (!this.TryAcquire(key, obj, out result));
        return result;
      }
    }

    public TValue PeekIfExists(TKey key)
    {
      ConcurrentCache<TKey, TValue>.Item obj = this.FindItem(key);
      if (obj != null)
        return obj.Value;
      return default (TValue);
    }

    public TValue Release(TKey key)
    {
      using (this._locks.Acquire(key))
      {
        ConcurrentCache<TKey, TValue>.Item obj = this.FindItem(key);
        if (obj == null)
        {
          this._factory.OnMissing(key);
          return default (TValue);
        }
        this.TryRelease(key, obj);
        return obj.Value;
      }
    }

    public void RemoveItem(TKey key, bool cacheDisposing)
    {
      ConcurrentCache<TKey, TValue>.Item obj;
      if ((object) key == null || !this._dic.TryRemove(key, out obj))
        return;
      this._factory.OnRemove(key, obj.Value, Interlocked.Read(ref obj.Counter));
      this._factory.Destroy(key, obj.Value, cacheDisposing);
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> AsPairs()
    {
      return this._dic.Select<KeyValuePair<TKey, ConcurrentCache<TKey, TValue>.Item>, KeyValuePair<TKey, TValue>>((Func<KeyValuePair<TKey, ConcurrentCache<TKey, TValue>.Item>, KeyValuePair<TKey, TValue>>) (p => new KeyValuePair<TKey, TValue>(p.Key, p.Value.Value)));
    }

    private bool TryAcquire(TKey key, ConcurrentCache<TKey, TValue>.Item item, out TValue result)
    {
      long counter;
      lock (item)
      {
        counter = Interlocked.Increment(ref item.Counter);
        if (counter < 1L)
        {
          Interlocked.Decrement(ref item.Counter);
          result = default (TValue);
          return false;
        }
        ConcurrentCache<TKey, TValue>.DeleteItemTask task = Interlocked.Exchange<ConcurrentCache<TKey, TValue>.DeleteItemTask>(ref item.DeleteTask, (ConcurrentCache<TKey, TValue>.DeleteItemTask) null);
        if (task != null)
          ConcurrentCache<TKey, TValue>.DeleteQueue.Cancel(task);
      }
      this._factory.OnAcquire(key, item.Value, counter);
      result = item.Value;
      return true;
    }

    private void TryRelease(TKey key, ConcurrentCache<TKey, TValue>.Item item)
    {
      long counter;
      lock (item)
        counter = Interlocked.Decrement(ref item.Counter);
      this._factory.OnRelease(key, item.Value, counter);
      if (counter > 0L)
        return;
      if (counter < 0L)
        Interlocked.Increment(ref item.Counter);
      else if (this.ReleaseImmediately)
      {
        this.RemoveItem(key, false);
      }
      else
      {
        ConcurrentCache<TKey, TValue>.DeleteItemTask task = new ConcurrentCache<TKey, TValue>.DeleteItemTask(this, key, item);
        Interlocked.Exchange<ConcurrentCache<TKey, TValue>.DeleteItemTask>(ref item.DeleteTask, task);
        ConcurrentCache<TKey, TValue>.DeleteQueue.Schedule(task);
      }
    }

    private ConcurrentCache<TKey, TValue>.Item FindItem(TKey key)
    {
      return this._dic.TryGet<TKey, ConcurrentCache<TKey, TValue>.Item>(key);
    }

    private ConcurrentCache<TKey, TValue>.Item GetOrCreateItem(TKey key)
    {
      return this._dic.GetOrAdd(key, new Func<TKey, ConcurrentCache<TKey, TValue>.Item>(this.CreateItem));
    }

    private ConcurrentCache<TKey, TValue>.Item CreateItem(TKey key)
    {
      return new ConcurrentCache<TKey, TValue>.Item(this._factory.Create(key));
    }

    public IEnumerator<TValue> GetEnumerator()
    {
      return this._dic.Values.Select<ConcurrentCache<TKey, TValue>.Item, TValue>((Func<ConcurrentCache<TKey, TValue>.Item, TValue>) (item => item.Value)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private sealed class Item
    {
      public readonly TValue Value;
      public ConcurrentCache<TKey, TValue>.DeleteItemTask DeleteTask;
      public long Counter;

      public Item(TValue value)
      {
        this.Value = value;
      }
    }

    private sealed class DeleteItemTask
    {
      private static long s_counter;
      public readonly long Id;
      public readonly DateTime Time;
      public readonly ConcurrentCache<TKey, TValue> Cache;
      public readonly TKey Key;
      public readonly ConcurrentCache<TKey, TValue>.Item Item;
      private Task _destroingTask;

      public DeleteItemTask(
        ConcurrentCache<TKey, TValue> cache,
        TKey key,
        ConcurrentCache<TKey, TValue>.Item item)
      {
        this.Id = Interlocked.Increment(ref ConcurrentCache<TKey, TValue>.DeleteItemTask.s_counter);
        this.Time = SManagedDateTime.UtcNow;
        this.Cache = cache;
        this.Key = key;
        this.Item = item;
      }

      public void DestroyAsyncSafe()
      {
        ConcurrentCache<TKey, TValue>.DeleteItemTask.State state = new ConcurrentCache<TKey, TValue>.DeleteItemTask.State();
        this._destroingTask = Task.Factory.StartNew((Action) (() => this.DestroySafe(state)), TaskCreationOptions.LongRunning);
        ConcurrentSet<Task> disposing = this.Cache._disposing;
        if (disposing.Count > 128)
        {
          disposing.Clear();
        }
        lock (state)
        {
          if (state.IsCompleted)
            return;
          disposing.TryAdd(this._destroingTask);
          state.IsAdded = true;
        }
      }

      private void DestroySafe(
        ConcurrentCache<TKey, TValue>.DeleteItemTask.State state)
      {
        try
        {
          this.Cache._factory.Destroy(this.Key, this.Item.Value, false);
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Failed to release resource {0}.", (object) this.Key);
        }
        finally
        {
          lock (state)
          {
            if (state.IsAdded)
              this.Cache._disposing.Remove(this._destroingTask);
            state.IsCompleted = true;
          }
        }
      }

      private sealed class State
      {
        public volatile bool IsAdded;
        public volatile bool IsCompleted;
      }
    }

    private static class DeleteQueue
    {
      private static readonly ConcurrentDictionary<long, ConcurrentCache<TKey, TValue>.DeleteItemTask> Tasks = new ConcurrentDictionary<long, ConcurrentCache<TKey, TValue>.DeleteItemTask>();
      private const int LifeTime = 30000;

      static DeleteQueue()
      {
        ConcurrentCache.TimerCallbacks.Add(new Action(ConcurrentCache<TKey, TValue>.DeleteQueue.DispatchSafe));
      }

      public static void Schedule(ConcurrentCache<TKey, TValue>.DeleteItemTask task)
      {
        ConcurrentCache<TKey, TValue>.DeleteQueue.Tasks.Add<long, ConcurrentCache<TKey, TValue>.DeleteItemTask>(task.Id, task);
      }

      public static void Cancel(ConcurrentCache<TKey, TValue>.DeleteItemTask task)
      {
        ConcurrentCache<TKey, TValue>.DeleteQueue.Tasks.Remove<long, ConcurrentCache<TKey, TValue>.DeleteItemTask>(task.Id);
      }

      private static void DispatchSafe()
      {
        try
        {
          foreach (ConcurrentCache<TKey, TValue>.DeleteItemTask task in (IEnumerable<ConcurrentCache<TKey, TValue>.DeleteItemTask>) ConcurrentCache<TKey, TValue>.DeleteQueue.Tasks.Values)
            ConcurrentCache<TKey, TValue>.DeleteQueue.DispatchTaskLock(task);
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Failed to dispatch delete queue.");
        }
      }

      private static void DispatchTaskLock(ConcurrentCache<TKey, TValue>.DeleteItemTask task)
      {
        if (!Monitor.TryEnter((object) task.Item, 0))
          return;
        try
        {
          ConcurrentCache<TKey, TValue>.DeleteQueue.DispatchTask(task);
        }
        catch (Exception ex)
        {
        }
        finally
        {
          Monitor.Exit((object) task.Item);
        }
      }

      private static void DispatchTask(ConcurrentCache<TKey, TValue>.DeleteItemTask task)
      {
        if (Interlocked.Read(ref task.Item.Counter) > 0L)
        {
          ConcurrentCache<TKey, TValue>.DeleteQueue.Tasks.Remove<long, ConcurrentCache<TKey, TValue>.DeleteItemTask>(task.Id);
        }
        else
        {
          if (Math.Abs((SManagedDateTime.UtcNow - task.Time).TotalMilliseconds) <= 30000.0)
            return;
          ConcurrentCache<TKey, TValue>.DeleteQueue.Tasks.Remove<long, ConcurrentCache<TKey, TValue>.DeleteItemTask>(task.Id);
          if (Interlocked.Read(ref task.Item.Counter) > 0L)
            return;
          task.Item.Counter = -1073741824L;
          task.Cache._dic.Remove<TKey, ConcurrentCache<TKey, TValue>.Item>(task.Key);
          task.DestroyAsyncSafe();
        }
      }
    }
  }
}
