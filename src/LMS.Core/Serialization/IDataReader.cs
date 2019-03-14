using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Serilization;

namespace LMS.Core.Serialization
{
  internal interface IDataReader
  {
    DeserializationContext TryBeginRead(TypeDescriptor descriptor);

    void EndRead();

    bool TryUnserialAsNullOrReference(
      DeserializationContext context,
      ref TypeDescriptor descriptor,
      ref object value);

    object ReadSerializableObject(
      DeserializationContext context,
      SerializableTypeDescriptor descriptor);

    object ReadNullable(DeserializationContext context, TypeDescriptor descriptor);

    object ReadConvertable(DeserializationContext context, TypeDescriptor descriptor);

    object ReadEnum(DeserializationContext context, TypeDescriptor descriptor);

    object ReadBoolean(DeserializationContext context, TypeDescriptor descriptor);

    object ReadByte(DeserializationContext context, TypeDescriptor descriptor);

    object ReadChar(DeserializationContext context, TypeDescriptor descriptor);

    object ReadDateTime(DeserializationContext context, TypeDescriptor descriptor);

    object ReadDBNull(DeserializationContext context, TypeDescriptor descriptor);

    object ReadDecimal(DeserializationContext context, TypeDescriptor descriptor);

    object ReadDouble(DeserializationContext context, TypeDescriptor descriptor);

    object ReadInt16(DeserializationContext context, TypeDescriptor descriptor);

    object ReadInt32(DeserializationContext context, TypeDescriptor descriptor);

    object ReadInt64(DeserializationContext context, TypeDescriptor descriptor);

    object ReadSByte(DeserializationContext context, TypeDescriptor descriptor);

    object ReadSingle(DeserializationContext context, TypeDescriptor descriptor);

    object ReadString(DeserializationContext context, TypeDescriptor descriptor);

    object ReadUInt16(DeserializationContext context, TypeDescriptor descriptor);

    object ReadUInt32(DeserializationContext context, TypeDescriptor descriptor);

    object ReadUInt64(DeserializationContext context, TypeDescriptor descriptor);

    object ReadGuid(DeserializationContext context, TypeDescriptor descriptor);

    object ReadTimeSpan(DeserializationContext context, TypeDescriptor descriptor);

    object ReadArray(DeserializationContext context, ArrayTypeDescriptor descriptor);

    object ReadList(DeserializationContext context, ListTypeDescriptor descriptor);

    object ReadDictionary(DeserializationContext context, DictionaryTypeDescriptor descriptor);
  }
}
