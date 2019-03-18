using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class StringHelper
  {
    public static string ConvertKeySetIdToString(this byte[] bytes)
    {
      return BitConverter.ToString(bytes).ToLower().Replace("-", "");
    }

    public static bool ContainsCaseInsensitive(string str, string subStr)
    {
      return !string.IsNullOrEmpty(str) && (string.IsNullOrEmpty(subStr) || str.ToLowerInvariant().Contains(subStr.ToLowerInvariant()));
    }

    public static bool ContainsCaseInsensitive(IEnumerable<string> strs, string str)
    {
      if (string.IsNullOrEmpty(str))
        return false;
      foreach (string str1 in strs)
      {
        if (string.Compare(str1, str, true) == 0)
          return true;
      }
      return false;
    }

    public static string GetSingleLineStr(string multiLineStr)
    {
      return multiLineStr.Replace(Environment.NewLine, " ").Replace("\n", " ");
    }

    public static bool TreatAsBool(string str)
    {
      if (string.IsNullOrEmpty(str))
        return false;
      if (!(str == "1"))
        return string.Compare(str, "true", true) == 0;
      return true;
    }



    public static string ReplaceSpecificFormatItem(string str, string arg, int formatItemNumber)
    {
      string str1 = "{" + formatItemNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "}";
      int num = 0;
      while (true)
      {
        num = str.IndexOf(str1, num, StringComparison.InvariantCulture);
        if (num != -1)
        {
          int index = num - 1;
          int startIndex = num + str1.Length;
          if (index >= 0 && str[index] == '{' && (startIndex < str.Length && str[startIndex] == '}'))
            ++num;
          else
            str = str.Substring(0, num) + arg + str.Substring(startIndex);
        }
        else
          break;
      }
      return str;
    }

    public static string Format<T>(IEnumerable<T> objs, string separator)
    {
      StringBuilder self = new StringBuilder();
      foreach (T obj in objs)
      {
        if ((object) obj != null)
        {
          self.Append(obj.ToString());
          self.Append(separator);
        }
      }
      self.TryRemoveEnding(separator);
      return self.ToString();
    }

    public static string FormatByComma<T>(IEnumerable<T> objs)
    {
      return StringHelper.Format<T>(objs, ", ");
    }

    public static string FormatBySemicolon<T>(IEnumerable<T> objs)
    {
      return StringHelper.Format<T>(objs, "; ");
    }

    public static string IpListToStr(IPAddress[] ips)
    {
      if (ips.Length == 0)
        return "";
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < ips.Length; ++index)
      {
        if (index != 0)
          stringBuilder.Append(";");
        stringBuilder.Append(ips[index].ToString());
      }
      return stringBuilder.ToString();
    }

    public static string NullToEmpty(string str)
    {
      if (str == null)
        return string.Empty;
      return str;
    }

    public static string[] NullToEmpty(string[] strs)
    {
      if (strs == null)
        return new string[0];
      for (int index = 0; index < strs.Length; ++index)
      {
        if (strs[index] == null)
          strs[index] = string.Empty;
      }
      return strs;
    }

    public static bool Compare(string[] array1, string[] array2)
    {
      if (array1 == null && array2 != null || array1 != null && array2 == null)
        return false;
      if (array1 == null && array2 == null)
        return true;
      if (array1.Length != array2.Length)
        return false;
      foreach (string str in array1)
      {
        if (!StringHelper.IsExists(array2, str, true))
          return false;
      }
      return true;
    }

    public static bool IsExists(string[] array, string str, bool ignoreCase)
    {
      foreach (string strA in array)
      {
        if (string.Compare(strA, str, ignoreCase) == 0)
          return true;
      }
      return false;
    }

    public static string[] ToString(object[] values)
    {
      List<string> stringList = new List<string>();
      foreach (object obj in values)
      {
        if (obj != null)
          stringList.Add(obj.ToString());
      }
      return stringList.ToArray();
    }

    public static string ToHexStr(byte[] bytes)
    {
      return BitConverter.ToString(bytes).Replace("-", "");
    }


    public static string Format2LogLine(Guid[] ids)
    {
      return string.Join<Guid>(", ", (IEnumerable<Guid>) ids);
    }

    public static string GetDefaultDescription()
    {
      DateTime now = SManagedDateTime.Now;
      return string.Format("Created by {0}\\{1} at {2}.", (object) Environment.UserDomainName, (object) Environment.UserName, (object) string.Format("{0} {1}", (object) now.ToShortDateString(), (object) now.ToShortTimeString()));
    }

    public static string GetDefaultName(string defaultNameTemplate, string[] existingNames)
    {
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) existingNames, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      int num = 0;
      string str;
      do
      {
        str = string.Format("{0} {1}", (object) defaultNameTemplate, (object) ++num);
      }
      while (stringSet.Contains(str));
      return str;
    }

    public static string ReplaceAt(this string input, int index, char newChar)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      char[] charArray = input.ToCharArray();
      charArray[index] = newChar;
      return new string(charArray);
    }

    public static bool IsStrValid(string str, string restrictedSymbols)
    {
      return str.IndexOfAny(restrictedSymbols.ToCharArray()) == -1;
    }

    public static string GeneratePath(string parentItemPath, string childItemName)
    {
      if (string.IsNullOrEmpty(parentItemPath))
        return childItemName;
      return string.Format("{0}\\{1}", (object) parentItemPath, (object) childItemName);
    }

    public static SecureString StringToSecureString(string value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      SecureString secureString = new SecureString();
      try
      {
        foreach (char c in value)
          secureString.AppendChar(c);
        secureString.MakeReadOnly();
        return secureString;
      }
      catch (Exception)
      {
        secureString.Dispose();
        throw;
      }
    }

    public static string SecureStringToString(SecureString value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      IntPtr num = IntPtr.Zero;
      try
      {
        num = Marshal.SecureStringToGlobalAllocUnicode(value);
        return Marshal.PtrToStringUni(num);
      }
      finally
      {
        Marshal.ZeroFreeGlobalAllocUnicode(num);
      }
    }

    public static string ShiftStringByTabs(int tabsNum, string text)
    {
      if (tabsNum <= 0)
        return text;
      string str = new string(' ', tabsNum * 4);
      StringBuilder stringBuilder = new StringBuilder(text.Length + str.Length);
      stringBuilder.Append(str);
      stringBuilder.Append(text);
      stringBuilder.Replace("\n", "\n" + str);
      return stringBuilder.ToString();
    }

    public static string Capitalize(string input)
    {
      if (string.IsNullOrEmpty(input))
        return input;
      char[] charArray = input.ToCharArray();
      charArray[0] = char.ToUpper(charArray[0]);
      return new string(charArray);
    }

    public static string Limit(string str, int maxLength)
    {
      return str?.Substring(0, Math.Min(str.Length, maxLength));
    }
  }
}
