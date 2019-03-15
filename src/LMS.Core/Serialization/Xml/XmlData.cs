namespace LMS.Core.Serialization.Xml
{
    using System;
    using System.Text;

    internal static class XmlData
  {
    public const string SystemNamespace = "vxs";
    public const string NullElementTag = "Null";
    public const string ItemElementTag = "Item";
    public const string VersionAttributeTag = "Version";
    public const string TypeIdAttributeTag = "GUID";
    public const string CountAttributeTag = "Count";
    public const string ExcludedTag = "Excluded";
    public const string IncludedTag = "Included";
    public const string KeyTag = "Key";
    public const string ValueTag = "Value";
    public const string NullValue = "vxs:null";
    public const char DateKindSeparator = ' ';
    public const char DateKindLocal = 'L';
    public const char DateKindUtc = 'G';
    public const char DateKindUnspecified = 'N';

    public static bool IsComplexType(TypeDescriptor descriptor)
    {
      return (XmlData.GetTypeFeature(descriptor) & XmlTypeFeature.Complex) == XmlTypeFeature.Complex;
    }

    public static XmlTypeFeature GetTypeFeature(TypeDescriptor descriptor)
    {
      XmlTypeFeature xmlTypeFeature = XmlTypeFeature.None;
      switch (descriptor.SerializationRoute)
      {
        case SerializationRoute.Array:
        case SerializationRoute.List:
        case SerializationRoute.Dictionary:
        case SerializationRoute.SerializableObject:
          xmlTypeFeature |= XmlTypeFeature.Complex;
          break;
      }
      return xmlTypeFeature;
    }

    public static char FormatDateTimeKind(DateTimeKind kind)
    {
      switch (kind)
      {
        case DateTimeKind.Unspecified:
          return 'N';
        case DateTimeKind.Utc:
          return 'G';
        case DateTimeKind.Local:
          return 'L';
        default:
          throw new NotSupportedException(kind.ToString());
      }
    }

    public static DateTimeKind ParseDateTimeKind(char ch)
    {
      switch (ch)
      {
        case 'G':
          return DateTimeKind.Utc;
        case 'L':
          return DateTimeKind.Local;
        case 'N':
          return DateTimeKind.Unspecified;
        default:
          throw new NotSupportedException(ch.ToString());
      }
    }

    public static string EscapeInvalidCharacters(string str)
    {
      char[] charArray = str.ToCharArray();
      StringBuilder stringBuilder = new StringBuilder(charArray.Length);
      for (int index = 0; index < charArray.Length; ++index)
      {
        char ch = charArray[index];
        if (ch >= 'A' && ch <= 'Z')
          stringBuilder.Append(ch);
        else if (ch >= 'a' && ch <= 'z')
          stringBuilder.Append(ch);
        else if (ch >= '0' && ch <= '9')
          stringBuilder.Append(ch);
        else if (ch == '[' && index < charArray.Length - 1 && charArray[index + 1] == ']')
          stringBuilder.Append("Array");
      }
      return stringBuilder.ToString();
    }
  }
}
