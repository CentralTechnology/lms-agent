using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
  internal class TypeDescriptor
  {
    private readonly TypeDescriptor[] _childs;

    public Type Type { get; private set; }

    public SerializationRoute SerializationRoute { get; private set; }

    public Func<object, object> Unwrap { get; private set; }

    public ISerializationRoutine Converter { get; private set; }

    public TypeFeature Feature { get; private set; }

    protected TypeDescriptor(Type type, SerializationRoute route, params TypeDescriptor[] childs)
    {
      this._childs = childs;
      this.Type = type;
      this.SerializationRoute = route;
      this.InitializeFeatures();
    }

    private void InitializeFeatures()
    {
      if (!this.Type.IsValueType)
        return;
      this.Feature |= TypeFeature.IsValueType;
    }

    public TypeDescriptor Child
    {
      get
      {
        return this._childs[0];
      }
    }

    public TypeDescriptor KeyType
    {
      get
      {
        return this._childs[0];
      }
    }

    public TypeDescriptor ValueType
    {
      get
      {
        return this._childs[1];
      }
    }

    public bool IsNullable
    {
      get
      {
        if (this.SerializationRoute != SerializationRoute.Nullable)
          return !this.IsValueType;
        return true;
      }
    }

    public bool IsValueType
    {
      get
      {
        return (this.Feature & TypeFeature.IsValueType) == TypeFeature.IsValueType;
      }
    }

    public static TypeDescriptor CreateTypeDescriptor(
      Type type,
      DescriptorInitializationStack stack)
    {
      return TypeDescriptor.TryCreateKnownTypeDescriptor(type, stack) ?? (TypeDescriptor) TypeDescriptor.CreateSerializable(type, stack);
    }

    public static TypeDescriptor TryCreateKnownTypeDescriptor(
      Type type,
      DescriptorInitializationStack stack)
    {
      if (type.IsArray)
      {
        Type elementType = type.GetElementType();
        return (TypeDescriptor) TypeDescriptor.CreateArray(type, TypeDescriptor.CreateTypeDescriptor(elementType, stack));
      }
      if (type.IsGenericType)
      {
        if (type.IsGenericTypeDefinition)
          throw new NotSupportedException("Generic type definition cannot be serialized.");
        Type genericTypeDefinition = type.GetGenericTypeDefinition();
        if (object.ReferenceEquals((object) genericTypeDefinition, (object) ReflectionTypes.Nullable))
        {
          Type genericArgument = type.GetGenericArguments()[0];
          return TypeDescriptor.CreateNullable(type, TypeDescriptor.CreateTypeDescriptor(genericArgument, stack));
        }
        if (object.ReferenceEquals((object) genericTypeDefinition, (object) ReflectionTypes.List))
        {
          Type genericArgument = type.GetGenericArguments()[0];
          return TypeDescriptor.CreateList(type, TypeDescriptor.CreateTypeDescriptor(genericArgument, stack));
        }
        if (!object.ReferenceEquals((object) genericTypeDefinition, (object) ReflectionTypes.Dictionary))
          throw new NotSupportedException("Generic types are not supported except nullable.");
        TypeDescriptor typeDescriptor1 = TypeDescriptor.CreateTypeDescriptor(type.GetGenericArguments()[0], stack);
        TypeDescriptor typeDescriptor2 = TypeDescriptor.CreateTypeDescriptor(type.GetGenericArguments()[1], stack);
        return TypeDescriptor.CreateDictionary(type, typeDescriptor1, typeDescriptor2);
      }
      if (type.IsEnum)
      {
        Type enumUnderlyingType = type.GetEnumUnderlyingType();
        return TypeDescriptor.CreateEnum(type, TypeDescriptor.CreateTypeDescriptor(enumUnderlyingType, stack));
      }
      TypeCode typeCode = Type.GetTypeCode(type);
      switch (typeCode)
      {
        case TypeCode.DBNull:
          return new TypeDescriptor(type, SerializationRoute.DBNull, new TypeDescriptor[0]);
        case TypeCode.Boolean:
          return new TypeDescriptor(type, SerializationRoute.Boolean, new TypeDescriptor[0]);
        case TypeCode.Char:
          return new TypeDescriptor(type, SerializationRoute.Char, new TypeDescriptor[0]);
        case TypeCode.SByte:
          return new TypeDescriptor(type, SerializationRoute.SByte, new TypeDescriptor[0]);
        case TypeCode.Byte:
          return new TypeDescriptor(type, SerializationRoute.Byte, new TypeDescriptor[0]);
        case TypeCode.Int16:
          return new TypeDescriptor(type, SerializationRoute.Int16, new TypeDescriptor[0]);
        case TypeCode.UInt16:
          return new TypeDescriptor(type, SerializationRoute.UInt16, new TypeDescriptor[0]);
        case TypeCode.Int32:
          return new TypeDescriptor(type, SerializationRoute.Int32, new TypeDescriptor[0]);
        case TypeCode.UInt32:
          return new TypeDescriptor(type, SerializationRoute.UInt32, new TypeDescriptor[0]);
        case TypeCode.Int64:
          return new TypeDescriptor(type, SerializationRoute.Int64, new TypeDescriptor[0]);
        case TypeCode.UInt64:
          return new TypeDescriptor(type, SerializationRoute.UInt64, new TypeDescriptor[0]);
        case TypeCode.Single:
          return new TypeDescriptor(type, SerializationRoute.Single, new TypeDescriptor[0]);
        case TypeCode.Double:
          return new TypeDescriptor(type, SerializationRoute.Double, new TypeDescriptor[0]);
        case TypeCode.Decimal:
          return new TypeDescriptor(type, SerializationRoute.Decimal, new TypeDescriptor[0]);
        case TypeCode.DateTime:
          return new TypeDescriptor(type, SerializationRoute.DateTime, new TypeDescriptor[0]);
        case TypeCode.String:
          return new TypeDescriptor(type, SerializationRoute.String, new TypeDescriptor[0]);
        default:
          if (type.IsValueType)
          {
            if (type == WellKnownTypes.Guid)
              return new TypeDescriptor(type, SerializationRoute.Guid, new TypeDescriptor[0]);
            if (type == WellKnownTypes.TimeSpan)
              return new TypeDescriptor(type, SerializationRoute.TimeSpan, new TypeDescriptor[0]);
          }
          TypeDescriptor descriptor;
          if (TypeDescriptor.TryCreateCastableDescriptor(type, stack, out descriptor))
            return descriptor;
          if (typeCode != TypeCode.Object)
            throw new NotSupportedException(typeCode.ToString());
          return (TypeDescriptor) null;
      }
    }

    private static bool TryCreateCastableDescriptor(
      Type type,
      DescriptorInitializationStack stack,
      out TypeDescriptor descriptor)
    {
      descriptor = (TypeDescriptor) null;
      SerializableTypeAttribute customAttribute = (SerializableTypeAttribute) Attribute.GetCustomAttribute((MemberInfo) type, ReflectionTypes.SerializableTypeAttribute);
      if (customAttribute == null)
        return false;
      if (customAttribute.As != (Type) null)
      {
        if (customAttribute.Routine != (Type) null)
          throw new NotSupportedException("Cannot use a serialization routine together a cast for the type " + type.FullName);
        descriptor = TypeDescriptor.CreateTypeDescriptor(customAttribute.As, stack);
        return true;
      }
      if (!(customAttribute.Routine != (Type) null))
        return false;
      Type type1 = ((IEnumerable<Type>) customAttribute.Routine.GetInterfaces()).SingleOrDefault<Type>((Func<Type, bool>) (t =>
      {
        if (t.IsGenericType)
          return t.GetGenericTypeDefinition() == ReflectionTypes.ISerializationRoutine;
        return false;
      }));
      if (type1 == (Type) null)
        throw new NotSupportedException("Serialization routine must implement ISerializationRoutine. Type: " + type.FullName);
      Type[] genericArguments = type1.GetGenericArguments();
      if (genericArguments.Length != 2)
        throw new NotSupportedException("Unexpected number of the generic arguments: " + (object) genericArguments.Length);
      Type type2 = genericArguments[0];
      if (type2 != type)
        throw new NotSupportedException("An invalid first generic argument " + (object) type2 + " has been occurred. Expeced: " + (object) type);
      TypeDescriptor typeDescriptor = TypeDescriptor.CreateTypeDescriptor(genericArguments[1], stack);
      descriptor = TypeDescriptor.CreateConvertable(type, typeDescriptor, customAttribute.Routine);
      return true;
    }

    public static ArrayTypeDescriptor CreateArray(
      Type type,
      TypeDescriptor elementTypeDescriptor)
    {
      return new ArrayTypeDescriptor(type, elementTypeDescriptor);
    }

    public static TypeDescriptor CreateList(
      Type type,
      TypeDescriptor elementTypeDescriptor)
    {
      return (TypeDescriptor) new ListTypeDescriptor(type, elementTypeDescriptor);
    }

    public static TypeDescriptor CreateDictionary(
      Type type,
      TypeDescriptor keyTypeDescriptor,
      TypeDescriptor valueTypeDescriptor)
    {
      return (TypeDescriptor) new DictionaryTypeDescriptor(type, keyTypeDescriptor, valueTypeDescriptor);
    }

    public static TypeDescriptor CreateNullable(
      Type type,
      TypeDescriptor valueTypeDescriptor)
    {
      return new TypeDescriptor(type, SerializationRoute.Nullable, new TypeDescriptor[1]
      {
        valueTypeDescriptor
      })
      {
        Unwrap = Unwrapper.ProvideUnwrapper(type)
      };
    }

    public static TypeDescriptor CreateConvertable(
      Type type,
      TypeDescriptor underlyingTypeDescriptor,
      Type serializationRoutine)
    {
      return new TypeDescriptor(type, SerializationRoute.Convertable, new TypeDescriptor[1]
      {
        underlyingTypeDescriptor
      })
      {
        Converter = SerializationRoutine.ProvideConverter(serializationRoutine)
      };
    }

    public static TypeDescriptor CreateEnum(
      Type type,
      TypeDescriptor underlyingTypeDescriptor)
    {
      return new TypeDescriptor(type, SerializationRoute.Enum, new TypeDescriptor[1]
      {
        underlyingTypeDescriptor
      })
      {
        Unwrap = Unwrapper.ProvideUnwrapper(type)
      };
    }

    public static SerializableTypeDescriptor CreateSerializable(
      Type type,
      DescriptorInitializationStack stack)
    {
      return (SerializableTypeDescriptor) SerializableTypeDescriptorFactory.ProvideTypeDescriptor(type, stack);
    }
  }
}
