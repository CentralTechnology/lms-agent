using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class ConcurrentDictionaryExm
  {
    public static TValue TryGet<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dic, TKey key) where TValue : class
    {
      TValue obj;
      if (!dic.TryGetValue(key, out obj))
        return default (TValue);
      return obj;
    }

    public static void Add<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> self,
      TKey key,
      TValue value)
    {
      ((IDictionary<TKey, TValue>) self).Add(key, value);
    }

    public static void Add<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> self,
      KeyValuePair<TKey, TValue> pair)
    {
      ((ICollection<KeyValuePair<TKey, TValue>>) self).Add(pair);
    }

    public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key)
    {
      return ((IDictionary<TKey, TValue>) self).Remove(key);
    }

    public static void Remove<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> self,
      KeyValuePair<TKey, TValue> pair)
    {
      ((ICollection<KeyValuePair<TKey, TValue>>) self).Remove(pair);
    }

    public static void SafeDisposeAndClear<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> self)
      where TValue : IDisposable
    {
      if (object.ReferenceEquals((object) self, (object) null))
        return;
      foreach (KeyValuePair<TKey, TValue> pair in self)
      {
        self.Remove<TKey, TValue>(pair);
        pair.Value.SafeDispose();
      }
    }

    public static bool IsAddedNotGotten<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> self,
      TKey key,
      Func<TKey, TValue> addValueFactory,
      out TValue value)
      where TValue : class
    {
      TValue createdValue = default (TValue);
      value = self.GetOrAdd(key, (Func<TKey, TValue>) (k =>
      {
        createdValue = addValueFactory(key);
        if (object.ReferenceEquals((object) createdValue, (object) null))
          throw new NotSupportedException(string.Format("A new value cannot be null. Key: {0}", (object) (TKey) key));
        return createdValue;
      }));
      return object.ReferenceEquals((object) value, (object) (TValue) createdValue);
    }

    public static IEnumerable<TKey> EnumerateKeys<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> self)
    {
      return self.Select<KeyValuePair<TKey, TValue>, TKey>((Func<KeyValuePair<TKey, TValue>, TKey>) (kvp => kvp.Key));
    }

    public static IEnumerable<TValue> EnumerateValues<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> self)
    {
      return self.Select<KeyValuePair<TKey, TValue>, TValue>((Func<KeyValuePair<TKey, TValue>, TValue>) (kvp => kvp.Value));
    }
  }
}
