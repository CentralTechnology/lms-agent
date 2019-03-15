using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Serilog;

namespace LMS.Core.Veeam.Backup.Core
{
  public static class CProxyBinaryFormatter
  {
    public static Encoding _Encoding = Encoding.Unicode;

    public static string Serialize(object obj)
    {
      try
      {
        return Convert.ToBase64String(CProxyBinaryFormatter.BinarySerializeObject(obj));
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Binary serialization failed");
        throw;
      }
    }

    public static string[] Serialize<T>(T[] itemsArray)
    {
      List<string> stringList = new List<string>();
      foreach (T items in itemsArray)
      {
        string str = CProxyBinaryFormatter.Serialize((object) items);
        stringList.Add(str);
      }
      return stringList.ToArray();
    }

    public static T[] Deserialize<T>(string[] itemsArray)
    {
      List<T> objList = new List<T>();
      foreach (string items in itemsArray)
      {
        T obj = CProxyBinaryFormatter.Deserialize<T>(items);
        objList.Add(obj);
      }
      return objList.ToArray();
    }

    public static T Deserialize<T>(string input)
    {
      try
      {
        return CProxyBinaryFormatter.BinaryDeserializeObject<T>(Convert.FromBase64String(input));
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Binary deserialization failed");
        throw;
      }
    }

    public static T BinaryDeserializeObject<T>(byte[] serializedType)
    {
      if (serializedType == null)
        throw new ArgumentNullException(nameof (serializedType));
      if (serializedType.Length.Equals(0))
        throw new ArgumentException(nameof (serializedType));
      using (MemoryStream memoryStream = new MemoryStream(serializedType))
      {
        object obj = new BinaryFormatter().Deserialize((Stream) memoryStream);
        return obj == DBNull.Value ? default (T) : (T) obj;
      }
    }

    public static byte[] BinarySerializeObject(object objectToSerialize)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new BinaryFormatter().Serialize((Stream) memoryStream, objectToSerialize ?? (object) DBNull.Value);
        return memoryStream.ToArray();
      }
    }
  }
}
