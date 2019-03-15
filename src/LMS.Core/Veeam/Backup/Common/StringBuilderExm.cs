using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class StringBuilderExm
  {
    public static string Substring(this StringBuilder self, int startIndex, int length)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < length; ++index)
        stringBuilder.Append(self[index + startIndex]);
      return stringBuilder.ToString();
    }

    public static int IndexOf(this StringBuilder self, char c, int startIndex = 0)
    {
      for (int index = startIndex; index < self.Length; ++index)
      {
        if ((int) self[index] == (int) c)
          return index;
      }
      return -1;
    }

    public static int IndexOf(this StringBuilder self, string str, int startIndex = 0)
    {
      for (int index1 = startIndex; index1 < self.Length; ++index1)
      {
        for (int index2 = 0; index2 < str.Length && (int) self[index1] == (int) str[index2]; ++index2)
        {
          if (index2 == str.Length - 1)
            return index1;
        }
      }
      return -1;
    }

    public static bool EndsWithAny(this StringBuilder self, params string[] strs)
    {
      foreach (string str1 in strs)
      {
        string str = str1;
        if (self.Length >= str.Length && !str.Where<char>((Func<char, int, bool>) ((t, i) => (int) self[self.Length - str.Length + i] != (int) t)).Any<char>())
          return true;
      }
      return false;
    }

    public static bool EndsWith(this StringBuilder sb, string ending)
    {
      if (sb.Length < ending.Length)
        return false;
      int num = sb.Length - ending.Length;
      for (int index = 0; index < ending.Length; ++index)
      {
        if ((int) sb[num + index] != (int) ending[index])
          return false;
      }
      return true;
    }

    public static bool TryRemoveEnding(this StringBuilder self, string ending)
    {
      if (self == null || ending == null || self.Length < ending.Length)
        return false;
      for (int index = 0; index < ending.Length; ++index)
      {
        if ((int) self[self.Length - ending.Length + index] != (int) ending[index])
          return false;
      }
      self.Length -= ending.Length;
      return true;
    }

    public static StringBuilder Append<T>(
      this StringBuilder self,
      IEnumerable<T> enumerable,
      string separator,
      Func<T, string> selector = null)
    {
      if (self == null)
        throw new ArgumentNullException(nameof (self));
      if (separator == null)
        throw new ArgumentNullException(nameof (separator));
      if (selector == null)
        selector = (Func<T, string>) (v => v.ToString());
      int length = self.Length;
      foreach (T obj in enumerable)
        self.Append(selector(obj)).Append(separator);
      if (self.Length != length)
        self.Remove(self.Length - separator.Length, separator.Length);
      return self;
    }

    public static StringBuilder AppendFormatLine(
      this StringBuilder self,
      string format,
      params object[] args)
    {
      self.AppendFormat(format, args);
      return self.AppendLine();
    }

    public static StringBuilder AppendFormatLine(
      this StringBuilder self,
      IFormatProvider provider,
      string format,
      params object[] args)
    {
      self.AppendFormat(provider, format, args);
      return self.AppendLine();
    }

    public static string Evict(this StringBuilder self)
    {
      string str = self.ToString();
      self.Clear();
      return str;
    }
  }
}
