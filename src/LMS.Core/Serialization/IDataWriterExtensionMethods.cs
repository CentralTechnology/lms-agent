using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serialization
{
  internal static class IDataWriterExtensionMethods
  {
    public static void Serialize(this IDataWriter writer, TypeDescriptor descriptor, object value)
    {
      SerializationContext context = writer.TryBeginSerialization(descriptor, value);
      if (context == null)
        return;
      writer.Write(context, descriptor, value);
      writer.EndSerialization();
    }

    public static void Write(
      this IDataWriter writer,
      SerializationContext context,
      TypeDescriptor descriptor,
      object value)
    {
      if (descriptor.IsNullable && writer.TrySerialAsNullOrReference(context, ref descriptor, value))
        return;
      switch (descriptor.SerializationRoute)
      {
        case SerializationRoute.Array:
          writer.WriteArray(context, (ArrayTypeDescriptor) descriptor, value);
          break;
        case SerializationRoute.List:
          writer.WriteList(context, (ListTypeDescriptor) descriptor, value);
          break;
        case SerializationRoute.Dictionary:
          writer.WriteDictionary(context, (DictionaryTypeDescriptor) descriptor, value);
          break;
        case SerializationRoute.Nullable:
          writer.WriteNullable(context, descriptor, value);
          break;
        case SerializationRoute.Enum:
          writer.WriteEnum(context, descriptor, value);
          break;
        case SerializationRoute.Boolean:
          writer.WriteBoolean(context, descriptor, value);
          break;
        case SerializationRoute.Byte:
          writer.WriteByte(context, descriptor, value);
          break;
        case SerializationRoute.Char:
          writer.WriteChar(context, descriptor, value);
          break;
        case SerializationRoute.DateTime:
          writer.WriteDateTime(context, descriptor, value);
          break;
        case SerializationRoute.DBNull:
          writer.WriteDBNull(context, descriptor, value);
          break;
        case SerializationRoute.Decimal:
          writer.WriteDecimal(context, descriptor, value);
          break;
        case SerializationRoute.Double:
          writer.WriteDouble(context, descriptor, value);
          break;
        case SerializationRoute.Int16:
          writer.WriteInt16(context, descriptor, value);
          break;
        case SerializationRoute.Int32:
          writer.WriteInt32(context, descriptor, value);
          break;
        case SerializationRoute.Int64:
          writer.WriteInt64(context, descriptor, value);
          break;
        case SerializationRoute.SByte:
          writer.WriteSByte(context, descriptor, value);
          break;
        case SerializationRoute.Single:
          writer.WriteSingle(context, descriptor, value);
          break;
        case SerializationRoute.String:
          writer.WriteString(context, descriptor, value);
          break;
        case SerializationRoute.UInt16:
          writer.WriteUInt16(context, descriptor, value);
          break;
        case SerializationRoute.UInt32:
          writer.WriteUInt32(context, descriptor, value);
          break;
        case SerializationRoute.UInt64:
          writer.WriteUInt64(context, descriptor, value);
          break;
        case SerializationRoute.Guid:
          writer.WriteGuid(context, descriptor, value);
          break;
        case SerializationRoute.TimeSpan:
          writer.WriteTimeSpan(context, descriptor, value);
          break;
        case SerializationRoute.SerializableObject:
          writer.WriteSerializableObject(context, (SerializableTypeDescriptor) descriptor, value);
          break;
        case SerializationRoute.Convertable:
          writer.WriteConvertable(context, descriptor, value);
          break;
        default:
          throw new NotImplementedException(descriptor.SerializationRoute.ToString());
      }
    }
  }
}
