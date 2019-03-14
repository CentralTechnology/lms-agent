using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LMS.Core.Serialization;
using LMS.Core.Serialization.Xml;
using LMS.Core.Serilization.Xml;

namespace LMS.Core.Serilization
{
  public static class XmlSerialization
  {
    public static void SerializeObject<T>(this XmlWriter writer, T instance)
    {
      Type type = (object) instance == null ? typeof (T) : instance.GetType();
      writer.SerializeObject(type, (object) instance);
    }

    public static T DeserializeObject<T>(this XmlReader reader)
    {
      return (T) reader.DeserializeObject(typeof (T));
    }

    public static void SerializeArray<T>(this XmlWriter writer, T[] instance)
    {
      writer.SerializeArray(typeof (T[]), typeof (T), (object) instance);
    }

    public static T[] DeserializeArray<T>(this XmlReader reader)
    {
      return (T[]) reader.DeserializeArray(typeof (T[]), typeof (T));
    }

    public static void SerializeObject(this XmlWriter xw, Type type, object instance)
    {
      TypeDescriptor descriptor = SerializableTypeDescriptorFactory.ProvideTypeDescriptor(type, DescriptorGenerationRules.None);
      new XmlDataWriter(xw).Serialize(descriptor, instance);
    }

    public static object DeserializeObject(this XmlReader xr, Type type)
    {
      TypeDescriptor descriptor = SerializableTypeDescriptorFactory.ProvideTypeDescriptor(type, DescriptorGenerationRules.None);
      return new XmlDataReader(xr).Deserialize(descriptor);
    }

    public static void SerializeArray(
      this XmlWriter xw,
      Type arrayType,
      Type elementType,
      object instance)
    {
      TypeDescriptor elementTypeDescriptor = SerializableTypeDescriptorFactory.ProvideTypeDescriptor(elementType, DescriptorGenerationRules.None);
      ArrayTypeDescriptor array = TypeDescriptor.CreateArray(arrayType, elementTypeDescriptor);
      new XmlDataWriter(xw).Serialize((TypeDescriptor) array, instance);
    }

    public static object DeserializeArray(this XmlReader xr, Type arrayType, Type elementType)
    {
      TypeDescriptor elementTypeDescriptor = SerializableTypeDescriptorFactory.ProvideTypeDescriptor(elementType, DescriptorGenerationRules.None);
      ArrayTypeDescriptor array = TypeDescriptor.CreateArray(arrayType, elementTypeDescriptor);
      return new XmlDataReader(xr).Deserialize((TypeDescriptor) array);
    }
  }
}
