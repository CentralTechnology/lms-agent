using System;
using System.Collections.Generic;
using System.Linq;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class SCollectionExtensions
  {
    public static TResult[] TransformToArray<TSource, TResult>(
      this ICollection<TSource> collection,
      Func<TSource, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().TransformToArray<TSource, TResult>(collection.Count, transformer);
    }

    public static TResult[] TransformToArray<TSource, TResult>(
      this ICollection<TSource> collection,
      Func<TSource, int, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().TransformToArray<TSource, TResult>(collection.Count, transformer);
    }

    public static List<TResult> TransformToList<TSource, TResult>(
      this ICollection<TSource> collection,
      Func<TSource, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().TransformToList<TSource, TResult>(collection.Count, transformer);
    }

    public static TResult[] CastToArray<TSource, TResult>(this ICollection<TSource> collection) where TSource : TResult
    {
      return collection.AsEnumerable<TSource>().CastToArray<TSource, TResult>(collection.Count);
    }

    public static T[] FilterToArray<T>(this ICollection<T> collection, Func<T, bool> filter)
    {
      return collection.AsEnumerable<T>().FilterToArray<T>(collection.Count, filter);
    }

    public static T[] FilterToArray<T>(this ICollection<T> collection, Func<T, int, bool> filter)
    {
      return collection.AsEnumerable<T>().FilterToArray<T>(collection.Count, filter);
    }

    public static TResult[] FilterToArray<TSource, TResult>(
      this ICollection<TSource> collection,
      Func<TSource, bool> filter,
      Func<TSource, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().FilterToArray<TSource, TResult>(collection.Count, filter, transformer);
    }

    public static TResult[] FilterToArray<TSource, TResult>(
      this ICollection<TSource> collection,
      Func<TSource, int, bool> filter,
      Func<TSource, int, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().FilterToArray<TSource, TResult>(collection.Count, filter, transformer);
    }

    public static T[] ConcatToArray<T>(this ICollection<T> items1, ICollection<T> items2)
    {
      return items1.AsEnumerable<T>().ConcatToArray<T>(items1.Count, (IEnumerable<T>) items2, items2.Count);
    }

    public static T[] ConcatToArray<T>(this ICollection<T> items1, params T[] items2)
    {
      return items1.AsEnumerable<T>().ConcatToArray<T>(items1.Count, (IEnumerable<T>) items2, items2.Length);
    }

    public static T[] DistinctToArray<T>(this ICollection<T> items)
    {
      return items.AsEnumerable<T>().DistinctToArray<T>(items.Count, (IEqualityComparer<T>) EqualityComparer<T>.Default);
    }

    public static T[] DistinctToArray<T>(this ICollection<T> items, IEqualityComparer<T> comparer)
    {
      return items.AsEnumerable<T>().DistinctToArray<T>(items.Count, comparer);
    }

    public static Dictionary<TKey, List<T>> TransformToDictionaryWithMany<T, TKey>(
      this ICollection<T> items,
      Func<T, TKey> keySelector)
    {
      return items.AsEnumerable<T>().TransformToDictionaryWithMany<T, TKey>(keySelector, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default);
    }

    public static Dictionary<TKey, List<T>> TransformToDictionaryWithMany<T, TKey>(
      this ICollection<T> items,
      Func<T, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      return items.AsEnumerable<T>().TransformToDictionaryWithMany<T, TKey>(keySelector, comparer);
    }

    public static TResult[] TransformReadonlyToArray<TSource, TResult>(
      this IReadOnlyCollection<TSource> collection,
      Func<TSource, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().TransformToArray<TSource, TResult>(collection.Count, transformer);
    }

    public static TResult[] TransformReadonlyToArray<TSource, TResult>(
      this IReadOnlyCollection<TSource> collection,
      Func<TSource, int, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().TransformToArray<TSource, TResult>(collection.Count, transformer);
    }

    public static List<TResult> TransformReadonlyToList<TSource, TResult>(
      this IReadOnlyCollection<TSource> collection,
      Func<TSource, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().TransformToList<TSource, TResult>(collection.Count, transformer);
    }

    public static TResult[] CastReadonlyToArray<TSource, TResult>(
      this IReadOnlyCollection<TSource> collection)
      where TSource : TResult
    {
      return collection.AsEnumerable<TSource>().CastToArray<TSource, TResult>(collection.Count);
    }

    public static T[] FilterReadonlyToArray<T>(
      this IReadOnlyCollection<T> collection,
      Func<T, bool> filter)
    {
      return collection.AsEnumerable<T>().FilterToArray<T>(collection.Count, filter);
    }

    public static T[] FilterReadonlyToArray<T>(
      this IReadOnlyCollection<T> collection,
      Func<T, int, bool> filter)
    {
      return collection.AsEnumerable<T>().FilterToArray<T>(collection.Count, filter);
    }

    public static TResult[] FilterReadonlyToArray<TSource, TResult>(
      this IReadOnlyCollection<TSource> collection,
      Func<TSource, bool> filter,
      Func<TSource, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().FilterToArray<TSource, TResult>(collection.Count, filter, transformer);
    }

    public static TResult[] FilterReadonlyToArray<TSource, TResult>(
      this IReadOnlyCollection<TSource> collection,
      Func<TSource, int, bool> filter,
      Func<TSource, int, TResult> transformer)
    {
      return collection.AsEnumerable<TSource>().FilterToArray<TSource, TResult>(collection.Count, filter, transformer);
    }

    public static T[] ConcatReadonlyToArray<T>(
      this IReadOnlyCollection<T> items1,
      IReadOnlyCollection<T> items2)
    {
      return items1.AsEnumerable<T>().ConcatToArray<T>(items1.Count, (IEnumerable<T>) items2, items2.Count);
    }

    public static T[] ConcatReadonlyToArray<T>(
      this IReadOnlyCollection<T> items1,
      params T[] items2)
    {
      return items1.AsEnumerable<T>().ConcatToArray<T>(items1.Count, (IEnumerable<T>) items2, items2.Length);
    }

    public static T[] DistinctReadonlyToArray<T>(this IReadOnlyCollection<T> items)
    {
      return items.AsEnumerable<T>().DistinctToArray<T>(items.Count, (IEqualityComparer<T>) EqualityComparer<T>.Default);
    }

    public static T[] DistinctReadonlyToArray<T>(
      this IReadOnlyCollection<T> items,
      IEqualityComparer<T> comparer)
    {
      return items.AsEnumerable<T>().DistinctToArray<T>(items.Count, comparer);
    }

    public static Dictionary<TKey, List<T>> TransformReadonlyToDictionaryWithMany<T, TKey>(
      this IReadOnlyCollection<T> items,
      Func<T, TKey> keySelector)
    {
      return items.AsEnumerable<T>().TransformToDictionaryWithMany<T, TKey>(keySelector, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default);
    }

    public static Dictionary<TKey, List<T>> TransformReadonlyToDictionaryWithMany<T, TKey>(
      this IReadOnlyCollection<T> items,
      Func<T, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      return items.AsEnumerable<T>().TransformToDictionaryWithMany<T, TKey>(keySelector, comparer);
    }

    public static T[] Slice<T>(this T[] items, int startIndex, int count)
    {
      if (startIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (startIndex));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (startIndex >= items.Length)
        return new T[0];
      if (count == 0)
        return new T[0];
      int sliceItemsCount = SCollectionExtensions.GetSliceItemsCount(items.Length, startIndex, count);
      T[] objArray = new T[sliceItemsCount];
      Array.Copy((Array) items, startIndex, (Array) objArray, 0, sliceItemsCount);
      return objArray;
    }

    public static T[] Slice<T>(this IList<T> items, int startIndex, int count)
    {
      if (startIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (startIndex));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (startIndex >= items.Count)
        return new T[0];
      if (count == 0)
        return new T[0];
      List<T> objList = new List<T>(SCollectionExtensions.GetSliceItemsCount(items.Count, startIndex, count));
      for (int index = 0; index < count && index + startIndex < items.Count; ++index)
      {
        T obj = items[index + startIndex];
        objList.Add(obj);
      }
      return objList.ToArray();
    }

    private static TResult[] TransformToArray<TSource, TResult>(
      this IEnumerable<TSource> collection,
      int count,
      Func<TSource, TResult> transformer)
    {
      Func<TSource, int, TResult> transformer1 = (Func<TSource, int, TResult>) ((item, index) => transformer(item));
      return collection.TransformToArray<TSource, TResult>(count, transformer1);
    }

    private static TResult[] TransformToArray<TSource, TResult>(
      this IEnumerable<TSource> collection,
      int count,
      Func<TSource, int, TResult> transformer)
    {
      TResult[] resultArray = new TResult[count];
      using (IEnumerator<TSource> enumerator = collection.GetEnumerator())
      {
        int index = 0;
        while (enumerator.MoveNext())
        {
          resultArray[index] = transformer(enumerator.Current, index);
          ++index;
        }
      }
      return resultArray;
    }

    private static List<TResult> TransformToList<TSource, TResult>(
      this IEnumerable<TSource> collection,
      int count,
      Func<TSource, TResult> transformer)
    {
      List<TResult> resultList = new List<TResult>(count);
      foreach (TSource source in collection)
        resultList.Add(transformer(source));
      return resultList;
    }

    private static TResult[] CastToArray<TSource, TResult>(
      this IEnumerable<TSource> collection,
      int count)
      where TSource : TResult
    {
      Func<TSource, TResult> transformer = (Func<TSource, TResult>) (a => (TResult) a);
      return collection.TransformToArray<TSource, TResult>(count, transformer);
    }

    private static T[] FilterToArray<T>(
      this IEnumerable<T> collection,
      int count,
      Func<T, bool> filter)
    {
      Func<T, int, bool> filter1 = (Func<T, int, bool>) ((item, index) => filter(item));
      Func<T, int, T> transformer = (Func<T, int, T>) ((item, index) => item);
      return collection.FilterToArray<T, T>(count, filter1, transformer);
    }

    private static T[] FilterToArray<T>(
      this IEnumerable<T> collection,
      int count,
      Func<T, int, bool> filter)
    {
      Func<T, int, T> transformer = (Func<T, int, T>) ((item, index) => item);
      return collection.FilterToArray<T, T>(count, filter, transformer);
    }

    private static TResult[] FilterToArray<TSource, TResult>(
      this IEnumerable<TSource> collection,
      int count,
      Func<TSource, bool> filter,
      Func<TSource, TResult> transformer)
    {
      Func<TSource, int, bool> filter1 = (Func<TSource, int, bool>) ((item, index) => filter(item));
      Func<TSource, int, TResult> transformer1 = (Func<TSource, int, TResult>) ((item, index) => transformer(item));
      return collection.FilterToArray<TSource, TResult>(count, filter1, transformer1);
    }

    private static TResult[] FilterToArray<TSource, TResult>(
      this IEnumerable<TSource> collection,
      int count,
      Func<TSource, int, bool> filter,
      Func<TSource, int, TResult> transformer)
    {
      List<TResult> resultList = new List<TResult>(count);
      using (IEnumerator<TSource> enumerator = collection.GetEnumerator())
      {
        int num = 0;
        while (enumerator.MoveNext())
        {
          TSource current = enumerator.Current;
          if (filter(current, num))
          {
            TResult result = transformer(current, num);
            resultList.Add(result);
          }
          ++num;
        }
      }
      return resultList.ToArray();
    }

    private static T[] ConcatToArray<T>(
      this IEnumerable<T> items1,
      int count1,
      IEnumerable<T> items2,
      int count2)
    {
      T[] array = new T[count1 + count2];
      items1.CopyToArray<T>(array, 0);
      items2.CopyToArray<T>(array, count1);
      return array;
    }

    private static T[] DistinctToArray<T>(this IEnumerable<T> items, int count)
    {
      return items.DistinctToArray<T>(count, (IEqualityComparer<T>) EqualityComparer<T>.Default);
    }

    private static T[] DistinctToArray<T>(
      this IEnumerable<T> items,
      int count,
      IEqualityComparer<T> comparer)
    {
      List<T> objList = new List<T>(count);
      HashSet<T> objSet = new HashSet<T>(comparer);
      foreach (T obj in items)
      {
        if (objSet.Add(obj))
          objList.Add(obj);
      }
      return objList.ToArray();
    }

    private static Dictionary<TKey, List<T>> TransformToDictionaryWithMany<T, TKey>(
      this IEnumerable<T> items,
      Func<T, TKey> keySelector)
    {
      return items.TransformToDictionaryWithMany<T, TKey>(keySelector, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default);
    }

    private static Dictionary<TKey, List<T>> TransformToDictionaryWithMany<T, TKey>(
      this IEnumerable<T> items,
      Func<T, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      Dictionary<TKey, List<T>> dictionary = new Dictionary<TKey, List<T>>(comparer);
      foreach (T obj in items)
      {
        TKey key = keySelector(obj);
        List<T> objList;
        if (!dictionary.TryGetValue(key, out objList))
        {
          objList = new List<T>();
          dictionary[key] = objList;
        }
        objList.Add(obj);
      }
      return dictionary;
    }

    private static void CopyToArray<T>(this IEnumerable<T> items, T[] array, int startIndex)
    {
      using (IEnumerator<T> enumerator = items.GetEnumerator())
      {
        int index = startIndex;
        while (enumerator.MoveNext())
        {
          array[index] = enumerator.Current;
          ++index;
        }
      }
    }

    private static int GetSliceItemsCount(int itemsCount, int startIndex, int count)
    {
      return Math.Min(count, itemsCount - startIndex);
    }
  }
}
