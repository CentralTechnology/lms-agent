using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LMS.Core.Serilization;
using LMS.Core.Serilization.Xml;
using RestSharp.Deserializers;

namespace LMS.Core.Serialization.Xml
{
  internal sealed class XmlDataReader : IDataReader
  {
    public readonly Stack<XmlReader> XRs;

    public XmlDataReader(XmlReader xw)
    {
      this.XRs = new Stack<XmlReader>(1);
      this.XRs.Push(xw);
    }

    public XmlReader XR
    {
      get
      {
        return this.XRs.Peek();
      }
    }

    public DeserializationContext TryBeginRead(TypeDescriptor descriptor)
    {
      int content = (int) this.XR.MoveToContent();
      this.XR.ExpectElement();
      if (this.XR.LocalName == "Null" && this.XR.NamespaceURI == "vxs")
        return (DeserializationContext) null;
      this.XR.MoveToNextExpectedAttribute("Version", "vxs");
      ushort version = ushort.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (version != (ushort) 1)
        throw new NotSupportedException("Not supported version of the serialized data: " + (object) version);
      return new DeserializationContext(version);
    }

    public void EndRead()
    {
    }

    public bool TryUnserialAsNullOrReference(
      DeserializationContext context,
      ref TypeDescriptor descriptor,
      ref object value)
    {
      return false;
    }

    public object ReadSerializableObject(
      DeserializationContext context,
      SerializableTypeDescriptor descriptor)
    {
      return new XmlDeserializer(descriptor).DeserializeInternal(context, this);
    }

    public object ReadNullable(DeserializationContext context, TypeDescriptor descriptor)
    {
      object obj = this.Read(context, descriptor.Child);
      return descriptor.Unwrap(obj);
    }

    public object ReadConvertable(DeserializationContext context, TypeDescriptor descriptor)
    {
      object source = this.Read(context, descriptor.Child);
      return descriptor.Converter.ToSource(source);
    }

    public object ReadEnum(DeserializationContext context, TypeDescriptor descriptor)
    {
      object obj = this.Read(context, descriptor.Child);
      return descriptor.Unwrap(obj);
    }

    public object ReadBoolean(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) bool.Parse(this.XR.Value);
    }

    public object ReadByte(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) byte.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadChar(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) (char) ushort.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadDateTime(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      string message = this.XR.Value;
      int num = message.IndexOf(' ');
      if (num != 1)
        throw new InvalidDataException(message);
      char ch = message[0];
      string s = message.Substring(num + 1);
      DateTimeKind dateTimeKind = XmlData.ParseDateTimeKind(ch);
      DateTime exact = DateTime.ParseExact(s, "o", (IFormatProvider) CultureInfo.InvariantCulture);
      switch (dateTimeKind)
      {
        case DateTimeKind.Utc:
          return (object) exact.ToUniversalTime();
        case DateTimeKind.Local:
          return (object) exact.ToLocalTime();
        default:
          return (object) DateTime.SpecifyKind(exact, dateTimeKind);
      }
    }

    public object ReadDBNull(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      if (byte.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture) != (byte) 1)
        return (object) null;
      return (object) DBNull.Value;
    }

    public object ReadDecimal(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) Decimal.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadDouble(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) double.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadInt16(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) short.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadInt32(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) int.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadInt64(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) long.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadSByte(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) sbyte.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadSingle(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) float.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadString(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) this.XR.Value;
    }

    public object ReadUInt16(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) ushort.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadUInt32(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) uint.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadUInt64(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) ulong.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadGuid(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) Guid.Parse(this.XR.Value);
    }

    public object ReadTimeSpan(DeserializationContext context, TypeDescriptor descriptor)
    {
      this.XR.ExpectAttribute();
      return (object) TimeSpan.Parse(this.XR.Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object ReadArray(DeserializationContext context, ArrayTypeDescriptor descriptor)
    {
      return this.ReadIList(context, (IListTypeDescriptor) descriptor);
    }

    public object ReadList(DeserializationContext context, ListTypeDescriptor descriptor)
    {
      return this.ReadIList(context, (IListTypeDescriptor) descriptor);
    }

    private object ReadIList(DeserializationContext context, IListTypeDescriptor descriptor)
    {
      int arrayLength;
      IEnumerable<int> indices;
      this.ReadArrayInfo(out arrayLength, out indices);
      TypeDescriptor child = descriptor.Child;
      IList list = descriptor.CreateList(arrayLength);
      if (arrayLength == 0)
        return (object) list;
      if (XmlData.IsComplexType(child))
      {
        this.ReadArrayElements(context, list, arrayLength, indices, descriptor, child, true);
      }
      else
      {
        this.XRs.Push((XmlReader) new ArrayStringSequenceXmlReader(this.XR));
        this.ReadArrayElements(context, list, arrayLength, indices, descriptor, child, false);
        this.XRs.Pop().Close();
      }
      descriptor.CommitList(list, arrayLength);
      return (object) list;
    }

    private void ReadArrayElements(
      DeserializationContext context,
      IList list,
      int count,
      IEnumerable<int> indices,
      IListTypeDescriptor listDescriptor,
      TypeDescriptor elementDescriptor,
      bool isComplex)
    {
      if (indices == null)
      {
        for (int index = 0; index < count; ++index)
        {
          object obj = this.ReadItem(context, elementDescriptor, isComplex);
          listDescriptor.SetValue(list, index, obj);
        }
      }
      else
      {
        foreach (int index in indices)
        {
          object obj = this.ReadItem(context, elementDescriptor, isComplex);
          listDescriptor.SetValue(list, index, obj);
        }
      }
      this.XR.MoveToEndOfElement();
    }

    private void ReadArrayInfo(out int arrayLength, out IEnumerable<int> indices)
    {
      this.XR.MoveToNextExpectedAttribute("Count", "vxs");
      arrayLength = int.Parse(this.XR.Value);
      indices = (IEnumerable<int>) null;
      if (arrayLength == 0 || !this.XR.MoveToNextAttribute() || !(this.XR.NamespaceURI == "vxs"))
        return;
      if (this.XR.LocalName == "Included")
      {
        indices = (IEnumerable<int>) this.ParseIndicesWhileReading(this.XR.Value);
      }
      else
      {
        if (!(this.XR.LocalName == "Excluded"))
          throw this.XR.CreateUnexpectNodeException();
        indices = (IEnumerable<int>) this.ParseIndicesWhileReading(this.XR.Value);
        indices = Enumerable.Range(0, arrayLength).Except<int>(indices);
      }
    }

    private List<int> ParseIndicesWhileReading(string value)
    {
      List<int> intList = new List<int>();
      if (string.IsNullOrEmpty(value))
        return intList;
      StringBuilder stringBuilder = new StringBuilder(10);
      foreach (char ch in value)
      {
        if (ch != ',')
          stringBuilder.Append(ch);
        else if (stringBuilder.Length > 0)
        {
          int num = int.Parse(stringBuilder.ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
          intList.Add(num);
          stringBuilder.Length = 0;
        }
      }
      if (stringBuilder.Length > 0)
      {
        int num = int.Parse(stringBuilder.ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
        intList.Add(num);
      }
      return intList;
    }

    private object ReadItem(
      DeserializationContext context,
      TypeDescriptor elementDescriptor,
      bool isComplex)
    {
      if (isComplex)
        return this.ReadComplexItem(context, elementDescriptor);
      return this.ReadSimpleItem(context, elementDescriptor);
    }

    private object ReadComplexItem(DeserializationContext context, TypeDescriptor elementDescriptor)
    {
      this.XR.MoveToNextExpectedElement("Item");
      return this.Read(context, elementDescriptor);
    }

    private object ReadSimpleItem(DeserializationContext context, TypeDescriptor elementDescriptor)
    {
      this.XR.MoveToNextAttribute();
      return this.Read(context, elementDescriptor);
    }

    public object ReadDictionary(
      DeserializationContext context,
      DictionaryTypeDescriptor descriptor)
    {
      int num = this.ReadDictionaryElementCount();
      IDictionary dictionary = descriptor.CreateDictionary(num);
      if (num == 0)
        return (object) dictionary;
      object[] excludedKeys;
      object[] includedKeys;
      object[] includedValues;
      this.ReadDictionaryElements(context, descriptor, num, out excludedKeys, out includedKeys, out includedValues);
      if (excludedKeys != null)
      {
        foreach (object key in excludedKeys)
          dictionary.Add(key, (object) null);
      }
      if (includedKeys != null)
      {
        for (int index = 0; index < includedKeys.Length; ++index)
          dictionary.Add(includedKeys[index], includedValues[index]);
      }
      this.XR.MoveToEndOfElement();
      return (object) dictionary;
    }

    private int ReadDictionaryElementCount()
    {
      this.XR.MoveToNextExpectedAttribute("Count", "vxs");
      return int.Parse(this.XR.Value);
    }

    private void ReadDictionaryElements(
      DeserializationContext context,
      DictionaryTypeDescriptor dictionaryDescriptor,
      int itemCount,
      out object[] excludedKeys,
      out object[] includedKeys,
      out object[] includedValues)
    {
      excludedKeys = (object[]) null;
      includedKeys = (object[]) null;
      includedValues = (object[]) null;
      while (itemCount > 0)
      {
        this.XR.MoveToNextElement();
        if (!(this.XR.NamespaceURI != "vxs"))
        {
          if (this.XR.LocalName == "Included")
          {
            itemCount -= this.ReadIncludedKeys(context, dictionaryDescriptor, out includedKeys, out includedValues);
          }
          else
          {
            if (!(this.XR.LocalName == "Excluded"))
              throw this.XR.CreateUnexpectNodeException();
            excludedKeys = this.ReadExcludedKeys(context, dictionaryDescriptor);
            itemCount -= excludedKeys.Length;
          }
        }
      }
    }

    private object[] ReadExcludedKeys(
      DeserializationContext context,
      DictionaryTypeDescriptor dictionaryDescriptor)
    {
      int length = this.ReadDictionaryElementCount();
      object[] objArray = new object[length];
      TypeDescriptor keyType = dictionaryDescriptor.KeyType;
      if (XmlData.IsComplexType(keyType))
      {
        for (int index = 0; index < length; ++index)
          objArray[index] = this.ReadItem(context, keyType, true);
      }
      else
      {
        this.XRs.Push((XmlReader) new ArrayStringSequenceXmlReader(this.XR));
        for (int index = 0; index < length; ++index)
          objArray[index] = this.ReadItem(context, keyType, false);
        this.XRs.Pop().Close();
      }
      return objArray;
    }

    private int ReadIncludedKeys(
      DeserializationContext context,
      DictionaryTypeDescriptor dictionaryDescriptor,
      out object[] includedKeys,
      out object[] includedValues)
    {
      int length = this.ReadDictionaryElementCount();
      includedKeys = new object[length];
      includedValues = new object[length];
      TypeDescriptor keyType = dictionaryDescriptor.KeyType;
      TypeDescriptor valueType = dictionaryDescriptor.ValueType;
      XmlDataReader.ReadDictionaryItemDelegate dictionaryItemDelegate = this.GetReadDictionaryItemDelegate(keyType, valueType);
      for (int index = 0; index < length; ++index)
      {
        this.XR.MoveToNextExpectedElement("Item");
        object key;
        object obj;
        dictionaryItemDelegate(context, keyType, valueType, out key, out obj);
        includedKeys[index] = key;
        includedValues[index] = obj;
      }
      return length;
    }

    private XmlDataReader.ReadDictionaryItemDelegate GetReadDictionaryItemDelegate(
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor)
    {
      if (XmlData.IsComplexType(keyDescriptor))
      {
        if (XmlData.IsComplexType(valueDescriptor))
          return new XmlDataReader.ReadDictionaryItemDelegate(this.ReadComplexDictionaryItem);
        return new XmlDataReader.ReadDictionaryItemDelegate(this.ReadComplexKeyDictionaryItem);
      }
      if (XmlData.IsComplexType(valueDescriptor))
        return new XmlDataReader.ReadDictionaryItemDelegate(this.ReadComplexValueDictionaryItem);
      return new XmlDataReader.ReadDictionaryItemDelegate(this.ReadSimpleDictionaryItem);
    }

    private void ReadComplexDictionaryItem(
      DeserializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      out object key,
      out object value)
    {
      this.XR.MoveToNextExpectedElement("Key");
      key = this.Read(context, keyDescriptor);
      this.XR.MoveToNextExpectedElement("Value");
      value = this.Read(context, valueDescriptor);
    }

    private void ReadComplexKeyDictionaryItem(
      DeserializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      out object key,
      out object value)
    {
      this.XR.MoveToNextExpectedAttribute("Value");
      value = this.Read(context, valueDescriptor);
      this.XR.MoveToNextExpectedElement("Key");
      key = this.Read(context, keyDescriptor);
    }

    private void ReadComplexValueDictionaryItem(
      DeserializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      out object key,
      out object value)
    {
      this.XR.MoveToNextExpectedAttribute("Key");
      key = this.Read(context, keyDescriptor);
      this.XR.MoveToNextExpectedElement("Value");
      value = this.Read(context, valueDescriptor);
    }

    private void ReadSimpleDictionaryItem(
      DeserializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      out object key,
      out object value)
    {
      this.XR.MoveToNextExpectedAttribute("Key");
      key = this.Read(context, keyDescriptor);
      this.XR.MoveToNextExpectedAttribute("Value");
      value = this.Read(context, valueDescriptor);
    }

    private delegate void ReadDictionaryItemDelegate(
      DeserializationContext context,
      TypeDescriptor keyDescriptor,
      TypeDescriptor valueDescriptor,
      out object key,
      out object value);
  }
}
