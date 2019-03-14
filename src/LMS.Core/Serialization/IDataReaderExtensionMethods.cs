using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Serilization;

namespace LMS.Core.Serialization
{
  internal static class IDataReaderExtensionMethods
  {
    public static object Deserialize(this IDataReader reader, TypeDescriptor descriptor)
    {
      DeserializationContext context = reader.TryBeginRead(descriptor);
      if (context == null)
        return (object) null;
      object obj = reader.Read(context, descriptor);
      reader.EndRead();
      return obj;
    }

    public static object Read(
      this IDataReader reader,
      DeserializationContext context,
      TypeDescriptor descriptor)
    {
      object obj = (object) null;
      if (descriptor.IsNullable && reader.TryUnserialAsNullOrReference(context, ref descriptor, ref obj))
        return obj;
      switch (descriptor.SerializationRoute)
      {
        case SerializationRoute.Array:
          return reader.ReadArray(context, (ArrayTypeDescriptor) descriptor);
        case SerializationRoute.List:
          return reader.ReadList(context, (ListTypeDescriptor) descriptor);
        case SerializationRoute.Dictionary:
          return reader.ReadDictionary(context, (DictionaryTypeDescriptor) descriptor);
        case SerializationRoute.Nullable:
          return reader.ReadNullable(context, descriptor);
        case SerializationRoute.Enum:
          return reader.ReadEnum(context, descriptor);
        case SerializationRoute.Boolean:
          return reader.ReadBoolean(context, descriptor);
        case SerializationRoute.Byte:
          return reader.ReadByte(context, descriptor);
        case SerializationRoute.Char:
          return reader.ReadChar(context, descriptor);
        case SerializationRoute.DateTime:
          return reader.ReadDateTime(context, descriptor);
        case SerializationRoute.DBNull:
          return reader.ReadDBNull(context, descriptor);
        case SerializationRoute.Decimal:
          return reader.ReadDecimal(context, descriptor);
        case SerializationRoute.Double:
          return reader.ReadDouble(context, descriptor);
        case SerializationRoute.Int16:
          return reader.ReadInt16(context, descriptor);
        case SerializationRoute.Int32:
          return reader.ReadInt32(context, descriptor);
        case SerializationRoute.Int64:
          return reader.ReadInt64(context, descriptor);
        case SerializationRoute.SByte:
          return reader.ReadSByte(context, descriptor);
        case SerializationRoute.Single:
          return reader.ReadSingle(context, descriptor);
        case SerializationRoute.String:
          return reader.ReadString(context, descriptor);
        case SerializationRoute.UInt16:
          return reader.ReadUInt16(context, descriptor);
        case SerializationRoute.UInt32:
          return reader.ReadUInt32(context, descriptor);
        case SerializationRoute.UInt64:
          return reader.ReadUInt64(context, descriptor);
        case SerializationRoute.Guid:
          return reader.ReadGuid(context, descriptor);
        case SerializationRoute.TimeSpan:
          return reader.ReadTimeSpan(context, descriptor);
        case SerializationRoute.SerializableObject:
          return reader.ReadSerializableObject(context, (SerializableTypeDescriptor) descriptor);
        case SerializationRoute.Convertable:
          return reader.ReadConvertable(context, descriptor);
        default:
          throw new NotImplementedException(descriptor.SerializationRoute.ToString());
      }
    }
  }
}
