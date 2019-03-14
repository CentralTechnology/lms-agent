using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
   internal interface IDataWriter
  {
    SerializationContext TryBeginSerialization(
      TypeDescriptor descriptor,
      object value);

    void EndSerialization();

    void WriteSerializableObject(
      SerializationContext context,
      SerializableTypeDescriptor descriptor,
      object value);

    void WriteNullable(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteConvertable(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteEnum(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteBoolean(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteByte(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteChar(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteDateTime(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteDBNull(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteDecimal(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteDouble(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteInt16(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteInt32(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteInt64(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteSByte(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteSingle(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteString(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteUInt16(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteUInt32(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteUInt64(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteGuid(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteTimeSpan(SerializationContext context, TypeDescriptor descriptor, object value);

    void WriteArray(SerializationContext context, ArrayTypeDescriptor descriptor, object value);

    void WriteList(SerializationContext context, ListTypeDescriptor descriptor, object value);

    void WriteDictionary(
      SerializationContext context,
      DictionaryTypeDescriptor descriptor,
      object value);

    bool TrySerialAsNullOrReference(
      SerializationContext context,
      ref TypeDescriptor descriptor,
      object value);
  }
}
