using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public abstract class ConcurrentCacheFactory<TKey, TValue>
  {
    public abstract TValue Create(TKey key);

    public abstract void Destroy(TKey key, TValue value, bool cacheDisposing);

    internal void OnAcquire(TKey key, TValue value, long counter)
    {
      
    }

    internal void OnRelease(TKey key, TValue value, long counter)
    {

      if (counter >= 0L)
      {

      }
      else
      {

      }
    }

    internal void OnRemove(TKey key, TValue value, long counter)
    {

    }

    internal void OnMissing(TKey key)
    {

    }

    protected virtual string FormatKey(TKey key)
    {
      return key.ToString();
    }

    protected virtual string FormatValue(TValue value)
    {
      return typeof (TValue).Name;
    }
  }
}
