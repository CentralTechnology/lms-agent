using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
  internal sealed class SerializableTypeDescriptor : TypeDescriptor
  {
    public const ushort Version = 1;
    public const ushort StopCode = 65535;
    public readonly Guid Id;
    public readonly SerializableTypeDescriptor SerializableParent;
    public readonly ConstructorAccessor Constructor;
    public readonly SerializationBindings Bindings;

    public SerializableTypeDescriptor(Type type, DescriptorInitializationStack stack)
      : base(type, SerializationRoute.SerializableObject)
    {
      stack.Add(type, this);
      try
      {
        bool isGenerated;
        this.Id = this.GetId(stack, out isGenerated);
        this.Constructor = this.CompileConstructor();
        this.Bindings = this.CompileBindings(stack, isGenerated);
        this.SerializableParent = this.TryGetValidSerializableParent(stack);
      }
      catch
      {
        stack.Remove(type);
        throw;
      }
    }

    private SerializableTypeDescriptor TryGetValidSerializableParent(
      DescriptorInitializationStack stack)
    {
      Type baseType = this.Type.BaseType;
      if (baseType == (Type) null || baseType == ReflectionTypes.Object || baseType == ReflectionTypes.ValueType)
        return (SerializableTypeDescriptor) null;
      return (SerializableTypeDescriptor) SerializableTypeDescriptorFactory.ProvideTypeDescriptor(baseType, stack);
    }

    private Guid GetId(DescriptorInitializationStack stack, out bool isGenerated)
    {
      SerializableTypeAttribute customAttribute = (SerializableTypeAttribute) Attribute.GetCustomAttribute((MemberInfo) this.Type, ReflectionTypes.SerializableTypeAttribute);
      if (customAttribute != null)
      {
        isGenerated = false;
        return customAttribute.Id;
      }
      if (stack.GenerationRules == DescriptorGenerationRules.None)
        throw new SerializationException(string.Format("Type {0} has not been marked with SerializableTypeAttribute.", (object) this.Type.FullName));
      isGenerated = true;
      return Guid.Empty;
    }

    private ConstructorAccessor CompileConstructor()
    {
      return new ConstructorAccessor(this.Type, this.Type.GetConstructor(new Type[0]));
    }

    private SerializationBindings CompileBindings(
      DescriptorInitializationStack stack,
      bool isGenerated)
    {
      if (!isGenerated)
        return this.CreateAccessors(stack);
      return this.GenerateAccessors(stack);
    }

    private SerializationBindings CreateAccessors(
      DescriptorInitializationStack stack)
    {
      SerializationBindingsBuilder builder = new SerializationBindingsBuilder();
      foreach (FieldInfo field in this.Type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        SerializableTypeDescriptor.TryCreateFieldAccessor(field, builder, stack);
      foreach (PropertyInfo property in this.Type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        SerializableTypeDescriptor.TryCreatePropertyAccessor(property, builder, stack);
      return builder.Build();
    }

    private SerializationBindings GenerateAccessors(
      DescriptorInitializationStack stack)
    {
      ushort generationNumber = 0;
      SerializationBindingsBuilder builder = new SerializationBindingsBuilder();
      if ((stack.GenerationRules & DescriptorGenerationRules.Fields) == DescriptorGenerationRules.Fields)
      {
        foreach (FieldInfo field in this.Type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
          SerializableTypeDescriptor.TryGenerateFieldAccessor(field, builder, stack, ref generationNumber);
      }
      if ((stack.GenerationRules & DescriptorGenerationRules.Properties) == DescriptorGenerationRules.Properties)
      {
        foreach (PropertyInfo property in this.Type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
          SerializableTypeDescriptor.GeneratePropertyAccessor(property, builder, stack, ref generationNumber);
      }
      return builder.Build();
    }

    private static void TryGenerateFieldAccessor(
      FieldInfo fieldInfo,
      SerializationBindingsBuilder builder,
      DescriptorInitializationStack stack,
      ref ushort generationNumber)
    {
      if (fieldInfo.Name[0] == '<')
        return;
      SerializableDataAttribute attribute = new SerializableDataAttribute(generationNumber++, fieldInfo.Name);
      SerializableTypeDescriptor.CreateFieldAccessor(fieldInfo, builder, stack, attribute);
    }

    private static void TryCreateFieldAccessor(
      FieldInfo fieldInfo,
      SerializationBindingsBuilder builder,
      DescriptorInitializationStack stack)
    {
      SerializableDataAttribute customAttribute = (SerializableDataAttribute) Attribute.GetCustomAttribute((MemberInfo) fieldInfo, ReflectionTypes.SerializableDataAttribute, false);
      if (customAttribute == null)
        return;
      customAttribute.Name = customAttribute.Name ?? fieldInfo.Name;
      SerializableTypeDescriptor.CreateFieldAccessor(fieldInfo, builder, stack, customAttribute);
    }

    private static void CreateFieldAccessor(
      FieldInfo fieldInfo,
      SerializationBindingsBuilder builder,
      DescriptorInitializationStack stack,
      SerializableDataAttribute attribute)
    {
      FieldAccessor fieldAccessor = new FieldAccessor(fieldInfo);
      SerializationBinding binding = new SerializationBinding(TypeDescriptor.CreateTypeDescriptor(fieldInfo.FieldType, stack), attribute.Index, attribute.Name, attribute.Options, (IExpressionAccessor) fieldAccessor);
      builder.Add(binding);
    }

    private static void GeneratePropertyAccessor(
      PropertyInfo propertyInfo,
      SerializationBindingsBuilder builder,
      DescriptorInitializationStack stack,
      ref ushort generationNumber)
    {
      SerializableDataAttribute attribute = new SerializableDataAttribute(generationNumber++, propertyInfo.Name);
      SerializableTypeDescriptor.CreatePropertyAccessor(propertyInfo, builder, stack, attribute);
    }

    private static void TryCreatePropertyAccessor(
      PropertyInfo propertyInfo,
      SerializationBindingsBuilder builder,
      DescriptorInitializationStack stack)
    {
      SerializableDataAttribute customAttribute = (SerializableDataAttribute) Attribute.GetCustomAttribute((MemberInfo) propertyInfo, ReflectionTypes.SerializableDataAttribute, false);
      if (customAttribute == null)
        return;
      customAttribute.Name = customAttribute.Name ?? propertyInfo.Name;
      SerializableTypeDescriptor.CreatePropertyAccessor(propertyInfo, builder, stack, customAttribute);
    }

    private static void CreatePropertyAccessor(
      PropertyInfo propertyInfo,
      SerializationBindingsBuilder builder,
      DescriptorInitializationStack stack,
      SerializableDataAttribute attribute)
    {
      IExpressionAccessor expression;
      if (propertyInfo.CanRead)
      {
        expression = !propertyInfo.CanWrite ? (IExpressionAccessor) new PropertyReader(propertyInfo) : (IExpressionAccessor) new PropertyAccessor(propertyInfo);
      }
      else
      {
        if (!propertyInfo.CanWrite)
          throw new NotSupportedException(string.Format("Property {0} is not accessible.", (object) propertyInfo.Name));
        expression = (IExpressionAccessor) new PropertyWriter(propertyInfo);
      }
      SerializationBinding binding = new SerializationBinding(TypeDescriptor.CreateTypeDescriptor(propertyInfo.PropertyType, stack), attribute.Index, attribute.Name, attribute.Options, expression);
      builder.Add(binding);
    }
  }
}
