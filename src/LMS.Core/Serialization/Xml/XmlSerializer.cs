using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LMS.Core.Serilization;
using LMS.Core.Serilization.Xml;

namespace LMS.Core.Serialization.Xml
{

  internal sealed class XmlSerializer
  {
    private readonly SerializableTypeDescriptor _descriptor;

    public XmlSerializer(SerializableTypeDescriptor descriptor)
    {
      this._descriptor = descriptor;
    }

    internal void SerializeInternal(SerializationContext context, XmlDataWriter writer, object obj)
    {
      this.TryWriteId(writer.XW);
      List<Tuple<SerializationBinding, object>> simpleFields;
      List<Tuple<SerializationBinding, object>> complexNotNull;
      List<SerializationBinding> complexNulls;
      this.SplitBindings(obj, out simpleFields, out complexNotNull, out complexNulls);
      this.SerialSimpleFields(context, writer, simpleFields);
      this.SerialComplexNulls(writer, complexNulls);
      this.SerializeParent(context, writer, obj);
      this.SerialComplexOther(context, writer, complexNotNull);
    }

    private void SerializeParent(SerializationContext context, XmlDataWriter writer, object obj)
    {
      if (this._descriptor.SerializableParent == null)
        return;
      writer.XW.WriteStartElement(XmlData.EscapeInvalidCharacters(this._descriptor.SerializableParent.Type.Name));
      new XmlSerializer(this._descriptor.SerializableParent).SerializeInternal(context, writer, obj);
      writer.XW.WriteEndElement();
    }

    private void SerialSimpleFields(
      SerializationContext context,
      XmlDataWriter writer,
      List<Tuple<SerializationBinding, object>> simpleFields)
    {
      foreach (Tuple<SerializationBinding, object> simpleField in simpleFields)
      {
        SerializationBinding binding = simpleField.Item1;
        object obj = simpleField.Item2;
        if (!this.TrySerialAsNull(writer, binding, obj))
        {
          writer.XW.WriteStartAttribute(binding.Name);
          writer.Write(context, binding.TypeDescriptor, obj);
          writer.XW.WriteEndAttribute();
        }
      }
    }

    private void SerialComplexNulls(XmlDataWriter writer, List<SerializationBinding> complexNulls)
    {
      foreach (SerializationBinding complexNull in complexNulls)
        this.TrySerialAsNull(writer, complexNull, (object) null);
    }

    private void SerialComplexOther(
      SerializationContext context,
      XmlDataWriter writer,
      List<Tuple<SerializationBinding, object>> complexNotNull)
    {
      foreach (Tuple<SerializationBinding, object> tuple in complexNotNull)
      {
        SerializationBinding serializationBinding = tuple.Item1;
        object obj = tuple.Item2;
        writer.XW.WriteStartElement(serializationBinding.Name);
        writer.Write(context, serializationBinding.TypeDescriptor, obj);
        writer.XW.WriteEndElement();
      }
    }

    private void SplitBindings(
      object obj,
      out List<Tuple<SerializationBinding, object>> simpleFields,
      out List<Tuple<SerializationBinding, object>> complexNotNull,
      out List<SerializationBinding> complexNulls)
    {
      simpleFields = new List<Tuple<SerializationBinding, object>>();
      complexNotNull = new List<Tuple<SerializationBinding, object>>();
      complexNulls = new List<SerializationBinding>();
      foreach (SerializationBinding serializableBinding in this._descriptor.Bindings.EnumerateSerializableBindings())
      {
        object obj1 = serializableBinding.Expression.Get(obj);
        if (XmlData.IsComplexType(serializableBinding.TypeDescriptor))
        {
          if (serializableBinding.ValueIsNull(obj1))
            complexNulls.Add(serializableBinding);
          else
            complexNotNull.Add(Tuple.Create<SerializationBinding, object>(serializableBinding, obj1));
        }
        else
          simpleFields.Add(Tuple.Create<SerializationBinding, object>(serializableBinding, obj1));
      }
    }

    private void TryWriteId(XmlWriter sw)
    {
      if (!(this._descriptor.Id != Guid.Empty))
        return;
      string str = this._descriptor.Id.ToString("D");
      sw.WriteAttributeString("GUID", "vxs", str);
    }

    private bool TrySerialAsNull(XmlDataWriter writer, SerializationBinding binding, object value)
    {
      if (!binding.ValueIsNull(value))
        return false;
      writer.XW.WriteAttributeString(binding.Name, "vxs:null");
      return true;
    }
  }
}
