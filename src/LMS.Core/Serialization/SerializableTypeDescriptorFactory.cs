using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
  internal static class SerializableTypeDescriptorFactory
  {
    private static readonly ConcurrentDictionary<string, Type> NameToType = new ConcurrentDictionary<string, Type>();
    private static readonly ConcurrentDictionary<Type, TypeDescriptor> TypeToDescriptor = new ConcurrentDictionary<Type, TypeDescriptor>();

    internal static TypeDescriptor ProvideTypeDescriptor(
      Type type,
      DescriptorGenerationRules rules)
    {
      return SerializableTypeDescriptorFactory.TypeToDescriptor.GetOrAdd(type, (Func<Type, TypeDescriptor>) (t => SerializableTypeDescriptorFactory.CreateTypeDescriptor(type, rules)));
    }

    internal static TypeDescriptor ProvideTypeDescriptor(
      string typeName,
      DescriptorGenerationRules rules)
    {
      return SerializableTypeDescriptorFactory.TypeToDescriptor.GetOrAdd(SerializableTypeDescriptorFactory.NameToType.GetOrAdd(typeName, new Func<string, Type>(SerializableTypeDescriptorFactory.ResolveType)), (Func<Type, TypeDescriptor>) (t => SerializableTypeDescriptorFactory.CreateTypeDescriptor(t, rules)));
    }

    internal static TypeDescriptor ProvideTypeDescriptor(
      Type type,
      DescriptorInitializationStack stack)
    {
      SerializableTypeDescriptor descriptor;
      if (stack.TryGetPendingDescriptor(type, out descriptor))
        return (TypeDescriptor) descriptor;
      return SerializableTypeDescriptorFactory.TypeToDescriptor.GetOrAdd(type, (Func<Type, TypeDescriptor>) (t => SerializableTypeDescriptorFactory.CreateTypeDescriptor(t, stack)));
    }

    private static TypeDescriptor CreateTypeDescriptor(
      Type type,
      DescriptorGenerationRules rules)
    {
      DescriptorInitializationStack stack = new DescriptorInitializationStack()
      {
        GenerationRules = rules
      };
      return SerializableTypeDescriptorFactory.CreateTypeDescriptor(type, stack);
    }

    private static TypeDescriptor CreateTypeDescriptor(
      Type type,
      DescriptorInitializationStack stack)
    {
      return TypeDescriptor.TryCreateKnownTypeDescriptor(type, stack) ?? (TypeDescriptor) new SerializableTypeDescriptor(type, stack);
    }

    private static Type ResolveType(string typeName)
    {
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        Type type = assembly.GetType(typeName);
        if (type != (Type) null)
          return type;
      }
      throw new TypeLoadException(string.Format("Type [{0}] not found.", (object) typeName));
    }
  }
}
