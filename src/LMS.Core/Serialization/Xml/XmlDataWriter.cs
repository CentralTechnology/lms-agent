namespace LMS.Core.Serialization.Xml
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml;

    internal sealed class XmlDataWriter : IDataWriter
  {
    private readonly Stack<XmlWriter> _writers;

    public XmlDataWriter(XmlWriter xw)
    {
      this._writers = new Stack<XmlWriter>(1);
      this._writers.Push(xw);
    }

    public XmlWriter XW
    {
      get
      {
        return this._writers.Peek();
      }
    }

    public SerializationContext TryBeginSerialization(
      TypeDescriptor descriptor,
      object value)
    {
      if (object.ReferenceEquals(value, (object) null))
      {
        this.XW.WriteStartElement("Null", "vxs");
        this.XW.WriteEndElement();
        return (SerializationContext) null;
      }
      SerializationContext serializationContext = new SerializationContext();
      this.XW.WriteStartElement(XmlData.EscapeInvalidCharacters(descriptor.Type.Name));
      this.XW.WriteAttributeString("Version", "vxs", 1.ToString((IFormatProvider) CultureInfo.InvariantCulture).ToString());
      return serializationContext;
    }

    public void EndSerialization()
    {
      this.XW.WriteEndElement();
    }

    public bool TrySerialAsNullOrReference(
      SerializationContext context,
      ref TypeDescriptor descriptor,
      object value)
    {
      return false;
    }

    public void WriteSerializableObject(
      SerializationContext context,
      SerializableTypeDescriptor descriptor,
      object value)
    {
      new XmlSerializer(descriptor).SerializeInternal(context, this, value);
    }

    public void WriteNullable(
      SerializationContext context,
      TypeDescriptor descriptor,
      object value)
    {
      this.Write(context, descriptor.Child, value);
    }

    public void WriteConvertable(
      SerializationContext context,
      TypeDescriptor descriptor,
      object value)
    {
      this.Write(context, descriptor.Child, descriptor.Converter.ToTarget(value));
    }

    public void WriteEnum(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.Write(context, descriptor.Child, value);
    }

    public void WriteBoolean(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((bool) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteByte(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((byte) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteChar(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((ushort) (char) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteDateTime(
      SerializationContext context,
      TypeDescriptor descriptor,
      object value)
    {
      DateTime dateTime = (DateTime) value;
      DateTimeKind kind = dateTime.Kind;
      string str = XmlData.FormatDateTimeKind(kind).ToString() + (object) ' ';
      if (kind == DateTimeKind.Local)
        dateTime = dateTime.ToUniversalTime();
      this.XW.WriteValue(str + dateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteDBNull(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue((value == DBNull.Value ? (byte) 1 : (byte) 0).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteDecimal(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((Decimal) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteDouble(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((double) value).ToString("R", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteInt16(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((short) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteInt32(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((int) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteInt64(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((long) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteSByte(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((sbyte) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteSingle(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((float) value).ToString("R", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteString(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue((string) value);
    }

    public void WriteUInt16(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((ushort) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteUInt32(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((uint) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteUInt64(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((ulong) value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteGuid(SerializationContext context, TypeDescriptor descriptor, object value)
    {
      this.XW.WriteValue(((Guid) value).ToString("D"));
    }

    public void WriteTimeSpan(
      SerializationContext context,
      TypeDescriptor descriptor,
      object value)
    {
      this.XW.WriteValue(((TimeSpan) value).ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public void WriteArray(
      SerializationContext context,
      ArrayTypeDescriptor descriptor,
      object value)
    {
      this.WriteIList(context, (IListTypeDescriptor) descriptor, value);
    }

    public void WriteList(
      SerializationContext context,
      ListTypeDescriptor descriptor,
      object value)
    {
      this.WriteIList(context, (IListTypeDescriptor) descriptor, value);
    }

    private void WriteIList(
      SerializationContext context,
      IListTypeDescriptor descriptor,
      object value)
    {
      List<object> includedObjects;
      List<int> includedIndices;
      List<int> excludedIndices;
      this.PrepareForWrite((IList) value, out includedObjects, out includedIndices, out excludedIndices);
      this.WriteArrayInfo(includedIndices, excludedIndices);
      this.WriteArrayElements(context, descriptor.Child, (IEnumerable<object>) includedObjects);
    }

    public void WriteDictionary(
      SerializationContext context,
      DictionaryTypeDescriptor descriptor,
      object value)
    {
      List<object> includedValues;
      List<int> includedIndices;
      List<int> excludedIndices;
      List<object> keys;
      this.PrepareDictionaryForWrite((IDictionary) value, out keys, out includedValues, out includedIndices, out excludedIndices);
      this.XW.WriteAttributeString("Count", "vxs", (includedIndices.Count + excludedIndices.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (excludedIndices.Count > 0)
      {
        this.WriteDictionaryNode("Excluded", excludedIndices.Count);
        this.WriteArrayElements(context, descriptor.KeyType, excludedIndices.Select<int, object>((Func<int, object>) (i => keys[i])));
        this.XW.WriteEndElement();
      }
      if (includedIndices.Count <= 0)
        return;
      this.WriteDictionaryNode("Included", includedIndices.Count);
      this.WriteDictionaryElements(context, descriptor.KeyType, descriptor.ValueType, keys, includedValues, includedIndices);
      this.XW.WriteEndElement();
    }

    private void WriteDictionaryElements(
      SerializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      List<object> keys,
      List<object> includedValues,
      List<int> includedIndices)
    {
      Action<SerializationContext, TypeDescriptor, TypeDescriptor, object, object> dictionaryItemDelegate = this.GetWriteDictionaryItemDelegate(keyDescriptor, valueDescriptor);
      for (int index = 0; index < includedIndices.Count; ++index)
      {
        int includedIndex = includedIndices[index];
        object key = keys[includedIndex];
        object includedValue = includedValues[index];
        this.XW.WriteStartElement("Item");
        dictionaryItemDelegate(context, keyDescriptor, valueDescriptor, key, includedValue);
        this.XW.WriteEndElement();
      }
    }

    private Action<SerializationContext, TypeDescriptor, TypeDescriptor, object, object> GetWriteDictionaryItemDelegate(
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor)
    {
      if (XmlData.IsComplexType(keyDescriptor))
      {
        if (XmlData.IsComplexType(valueDescriptor))
          return new Action<SerializationContext, TypeDescriptor, TypeDescriptor, object, object>(this.WriteComplexDictionaryItem);
        return new Action<SerializationContext, TypeDescriptor, TypeDescriptor, object, object>(this.WriteComplexKeyDictionaryItem);
      }
      if (XmlData.IsComplexType(valueDescriptor))
        return new Action<SerializationContext, TypeDescriptor, TypeDescriptor, object, object>(this.WriteComplexValueDictionaryItem);
      return new Action<SerializationContext, TypeDescriptor, TypeDescriptor, object, object>(this.WriteSimpleDictionaryItem);
    }

    private void WriteComplexDictionaryItem(
      SerializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      object key,
      object value)
    {
      this.XW.WriteStartElement("Key");
      this.Write(context, keyDescriptor, key);
      this.XW.WriteEndElement();
      this.XW.WriteStartElement("Value");
      this.Write(context, valueDescriptor, value);
      this.XW.WriteEndElement();
    }

    private void WriteComplexKeyDictionaryItem(
      SerializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      object key,
      object value)
    {
      this.XW.WriteStartAttribute("Value");
      this.Write(context, valueDescriptor, value);
      this.XW.WriteEndAttribute();
      this.XW.WriteStartElement("Key");
      this.Write(context, keyDescriptor, key);
      this.XW.WriteEndElement();
    }

    private void WriteComplexValueDictionaryItem(
      SerializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      object key,
      object value)
    {
      this.XW.WriteStartAttribute("Key");
      this.Write(context, keyDescriptor, key);
      this.XW.WriteEndAttribute();
      this.XW.WriteStartElement("Value");
      this.Write(context, valueDescriptor, value);
      this.XW.WriteEndElement();
    }

    private void WriteSimpleDictionaryItem(
      SerializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      object key,
      object value)
    {
      this.XW.WriteStartAttribute("Key");
      this.Write(context, keyDescriptor, key);
      this.XW.WriteEndAttribute();
      this.XW.WriteStartAttribute("Value");
      this.Write(context, valueDescriptor, value);
      this.XW.WriteEndAttribute();
    }

    private void WriteDictionaryNode(string tag, int count)
    {
      this.XW.WriteStartElement(tag, "vxs");
      this.XW.WriteAttributeString("Count", "vxs", count.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private void PrepareForWrite(
      IList array,
      out List<object> includedObjects,
      out List<int> includedIndices,
      out List<int> excludedIndices)
    {
      int count = array.Count;
      includedObjects = new List<object>(count);
      includedIndices = new List<int>(count);
      excludedIndices = new List<int>(count);
      for (int index = 0; index < count; ++index)
      {
        object obj = array[index];
        if (obj == null)
        {
          excludedIndices.Add(index);
        }
        else
        {
          includedIndices.Add(index);
          includedObjects.Add(obj);
        }
      }
    }

    private void PrepareDictionaryForWrite(
      IDictionary dictionary,
      out List<object> keys,
      out List<object> includedValues,
      out List<int> includedIndices,
      out List<int> excludedIndices)
    {
      int count = dictionary.Count;
      keys = new List<object>(count);
      includedValues = new List<object>(count);
      includedIndices = new List<int>(count);
      excludedIndices = new List<int>(count);
      int num = -1;
      IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
      while (enumerator.MoveNext())
      {
        ++num;
        keys.Add(enumerator.Key);
        object obj = enumerator.Value;
        if (obj == null)
        {
          excludedIndices.Add(num);
        }
        else
        {
          includedIndices.Add(num);
          includedValues.Add(obj);
        }
      }
    }

    private void WriteArrayInfo(List<int> includedIndices, List<int> excludedIndices)
    {
      this.XW.WriteAttributeString("Count", "vxs", (includedIndices.Count + excludedIndices.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (excludedIndices.Count <= 0)
        return;
      if (excludedIndices.Count < includedIndices.Count)
        this.XW.WriteAttributeString("Excluded", "vxs", this.FormatIndicesForWrite(excludedIndices));
      else
        this.XW.WriteAttributeString("Included", "vxs", this.FormatIndicesForWrite(includedIndices));
    }

    private string FormatIndicesForWrite(List<int> indices)
    {
      StringBuilder stringBuilder = new StringBuilder(indices.Count * 4);
      foreach (int index in indices)
        stringBuilder.Append(index.ToString((IFormatProvider) CultureInfo.InvariantCulture)).Append(',');
      if (stringBuilder.Length > 0)
        --stringBuilder.Length;
      return stringBuilder.ToString();
    }

    private void WriteArrayElements(
      SerializationContext context,
      TypeDescriptor elementDescriptor,
      IEnumerable<object> includedObjects)
    {
      if (XmlData.IsComplexType(elementDescriptor))
      {
        foreach (object includedObject in includedObjects)
          this.WriteComplexItem(context, elementDescriptor, includedObject);
      }
      else
      {
        this._writers.Push((XmlWriter) new ArrayStringSequenceXmlWriter(this.XW));
        foreach (object includedObject in includedObjects)
          this.WriteSimpleItem(context, elementDescriptor, includedObject);
        this._writers.Pop().Flush();
      }
    }

    private void WriteComplexItem(
      SerializationContext context,
      TypeDescriptor elementDescriptor,
      object item)
    {
      this.XW.WriteStartElement("Item");
      this.Write(context, elementDescriptor, item);
      this.XW.WriteEndElement();
    }

    private void WriteSimpleItem(
      SerializationContext context,
      TypeDescriptor elementDescriptor,
      object item)
    {
      this.Write(context, elementDescriptor, item);
    }
  }
}
