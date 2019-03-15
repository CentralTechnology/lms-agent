using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Serialization.Xml
{
  internal sealed class XmlDeserializer
  {
    private readonly SerializableTypeDescriptor _descriptor;

    public XmlDeserializer(SerializableTypeDescriptor descriptor)
    {
      this._descriptor = descriptor;
    }

    internal object DeserializeInternal(DeserializationContext context, XmlDataReader reader)
    {
      object result = this._descriptor.Constructor.Create();
      this.DeserializeInternal(context, reader, ref result);
      return result;
    }

    private void DeserializeInternal(
      DeserializationContext context,
      XmlDataReader reader,
      ref object result)
    {
      this.CheckId(reader.XR);
      while (reader.XR.MoveToNextAttribute())
        this.TryDeserializeField(context, reader, ref result);
      reader.XRs.Push(reader.XR.MoveToSubtree());
      if (reader.XR.MoveToNextElement() && !this.TryDeserializeParent(context, reader, ref result))
        this.TryDeserializeField(context, reader, ref result);
      while (reader.XR.MoveToNextElement())
        this.TryDeserializeField(context, reader, ref result);
      reader.XRs.Pop().Close();
    }

    private bool TryDeserializeParent(
      DeserializationContext context,
      XmlDataReader reader,
      ref object result)
    {
      if (this._descriptor.SerializableParent == null)
        return false;
      new XmlDeserializer(this._descriptor.SerializableParent).DeserializeInternal(context, reader, ref result);
      return true;
    }

    private void TryDeserializeField(
      DeserializationContext context,
      XmlDataReader reader,
      ref object result)
    {
      SerializationBinding binding = this._descriptor.Bindings.FindBinding(reader.XR.Name);
      if (binding == null)
        return;
      this.DeserializeField(context, reader, binding, ref result);
    }

    private void DeserializeField(
      DeserializationContext context,
      XmlDataReader reader,
      SerializationBinding binding,
      ref object result)
    {
      object obj = (object) null;
      if (!this.TryUnserialAsNull(reader, binding))
        obj = reader.Read(context, binding.TypeDescriptor);
      binding.Expression.Set(ref result, obj);
    }

    private static ushort GetVersion(XmlReader xr)
    {
      xr.MoveToNextExpectedAttribute("Version", "vxs");
      ushort num = ushort.Parse(xr.Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (num != (ushort) 1)
        throw new NotSupportedException("Not supported version of the serialized data: " + (object) num);
      return num;
    }

    private void CheckId(XmlReader xr)
    {
      if (this._descriptor.Id == Guid.Empty)
        return;
      xr.MoveToNextExpectedAttribute("GUID", "vxs");
      Guid guid = Guid.Parse(xr.Value);
      if (guid != this._descriptor.Id)
        throw new InvalidDataException(string.Format("Readed identifier {0} doesn't match the identifier of the serializable type {1}.", (object) guid, (object) this._descriptor.Id));
    }

    private bool TryUnserialAsNull(XmlDataReader reader, SerializationBinding binding)
    {
      if (!binding.IsNullable || reader.XR.NodeType != XmlNodeType.Attribute)
        return false;
      return reader.XR.Value == "vxs:null";
    }
  }
}
