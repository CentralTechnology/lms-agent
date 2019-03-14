using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  internal static class SOptionsLoader<TContainer> where TContainer : class
  {
    private static ConcurrentDictionary<System.Type, IReadOnlyList<SOptionsLoader<TContainer>.DLoadOptionalValue>> s_cache = new ConcurrentDictionary<System.Type, IReadOnlyList<SOptionsLoader<TContainer>.DLoadOptionalValue>>();

    public static void LoadOptions(TContainer container, IOptionsReader reader)
    {
      foreach (SOptionsLoader<TContainer>.DLoadOptionalValue dloadOptionalValue in (IEnumerable<SOptionsLoader<TContainer>.DLoadOptionalValue>) SOptionsLoader<TContainer>.s_cache.GetOrAdd(TypeCache<TContainer>.Type, (Func<System.Type, IReadOnlyList<SOptionsLoader<TContainer>.DLoadOptionalValue>>) (t => SOptionsLoader<TContainer>.CreateOptionLoaders())))
        dloadOptionalValue(container, reader);
    }

    private static IReadOnlyList<SOptionsLoader<TContainer>.DLoadOptionalValue> CreateOptionLoaders()
    {
      COptionsScopeAttribute customAttribute1 = TypeCache<TContainer>.Type.GetCustomAttribute<COptionsScopeAttribute>(true);
      if (customAttribute1 == null)
        throw new InvalidOperationException(string.Format("Type [{0}] is not marked by CRegistryOptionalScopeAttribute", (object) TypeCache<TContainer>.Type.FullName));
      MemberInfo[] members = TypeCache<TContainer>.Type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      List<SOptionsLoader<TContainer>.DLoadOptionalValue> dloadOptionalValueList = new List<SOptionsLoader<TContainer>.DLoadOptionalValue>(members.Length);
      foreach (MemberInfo memberInfo in members)
      {
        COptionsValueAttribute customAttribute2 = memberInfo.GetCustomAttribute<COptionsValueAttribute>(true);
        if (customAttribute2 != null)
        {
          SOptionsLoader<TContainer>.DLoadOptionalValue loader = SOptionsLoader<TContainer>.CreateLoader(memberInfo, customAttribute2, customAttribute1);
          dloadOptionalValueList.Add(loader);
        }
      }
      return (IReadOnlyList<SOptionsLoader<TContainer>.DLoadOptionalValue>) dloadOptionalValueList.ToArray();
    }

    private static SOptionsLoader<TContainer>.DLoadOptionalValue CreateLoader(
      MemberInfo member,
      COptionsValueAttribute memberAttribute,
      COptionsScopeAttribute scopeAttribute)
    {
      System.Type type;
      switch (member.MemberType)
      {
        case MemberTypes.Field:
          type = ((FieldInfo) member).FieldType;
          break;
        case MemberTypes.Property:
          type = ((PropertyInfo) member).PropertyType;
          break;
        default:
          throw new NotSupportedException(member.MemberType.ToString());
      }
      string optionName = memberAttribute.ResolveOptionName(scopeAttribute.Prefix, member.Name);
      COptionsTimeSpanAttribute attributes = memberAttribute as COptionsTimeSpanAttribute;
      if (attributes != null)
        return SOptionsLoader<TContainer>.MakeTimeSpan(member, optionName, attributes);
      if (type == TypeCache<Version>.Type)
        return SOptionsLoader<TContainer>.MakeVersion(member, optionName);
      TypeCode typeCode = System.Type.GetTypeCode(type);
      switch (typeCode)
      {
        case TypeCode.Boolean:
          return SOptionsLoader<TContainer>.MakeBoolean(member, optionName);
        case TypeCode.Int32:
          return SOptionsLoader<TContainer>.MakeInt32(member, optionName);
        case TypeCode.String:
          return SOptionsLoader<TContainer>.MakeString(member, optionName);
        default:
          throw new NotSupportedException(typeCode.ToString());
      }
    }

    private static SOptionsLoader<TContainer>.DLoadOptionalValue MakeVersion(
      MemberInfo member,
      string optionName)
    {
      SOptionsLoader<TContainer>.Expressions.SetValue<Version> setter = SOptionsLoader<TContainer>.Expressions.CreateSetter<Version>(member);
      return (SOptionsLoader<TContainer>.DLoadOptionalValue) ((instance, reader) =>
      {
        object obj;
        if (!reader.TryGetOptionalValue(optionName, out obj))
          return;
        string input = obj as string;
        if (input != null)
        {
          if (!(input != string.Empty))
            return;
          setter(instance, Version.Parse(input));
        }
        else
        {
          int major = (int) obj;
          setter(instance, new Version(major, 0, 0, 0));
        }
      });
    }

    private static SOptionsLoader<TContainer>.DLoadOptionalValue MakeTimeSpan(
      MemberInfo member,
      string optionName,
      COptionsTimeSpanAttribute attributes)
    {
      SOptionsLoader<TContainer>.Expressions.SetValue<TimeSpan> setter = SOptionsLoader<TContainer>.Expressions.CreateSetter<TimeSpan>(member);
      long defaultValue = attributes.DefaultValue;
      Func<double, TimeSpan> fromInteger;
      switch (attributes.From)
      {
        case EOptionsTimeSpanFormat.Ticks:
          fromInteger = (Func<double, TimeSpan>) (ticks => TimeSpan.FromTicks((long) (int) ticks));
          break;
        case EOptionsTimeSpanFormat.Milliseconds:
          fromInteger = new Func<double, TimeSpan>(TimeSpan.FromMilliseconds);
          break;
        case EOptionsTimeSpanFormat.Seconds:
          fromInteger = new Func<double, TimeSpan>(TimeSpan.FromSeconds);
          break;
        case EOptionsTimeSpanFormat.Minutes:
          fromInteger = new Func<double, TimeSpan>(TimeSpan.FromMinutes);
          break;
        case EOptionsTimeSpanFormat.Hours:
          fromInteger = new Func<double, TimeSpan>(TimeSpan.FromHours);
          break;
        case EOptionsTimeSpanFormat.Days:
          fromInteger = new Func<double, TimeSpan>(TimeSpan.FromDays);
          break;
        default:
          throw new NotSupportedException(attributes.From.ToString());
      }
      return (SOptionsLoader<TContainer>.DLoadOptionalValue) ((instance, reader) =>
      {
        TimeSpan timeSpan = fromInteger((double) reader.GetOptionalInt64(optionName, defaultValue));
        setter(instance, timeSpan);
      });
    }

    private static SOptionsLoader<TContainer>.DLoadOptionalValue MakeBoolean(
      MemberInfo member,
      string optionName)
    {
      SOptionsLoader<TContainer>.Expressions.GetValue<bool> getter = SOptionsLoader<TContainer>.Expressions.CreateGetter<bool>(member);
      SOptionsLoader<TContainer>.Expressions.SetValue<bool> setter = SOptionsLoader<TContainer>.Expressions.CreateSetter<bool>(member);
      return (SOptionsLoader<TContainer>.DLoadOptionalValue) ((instance, reader) =>
      {
        bool defaultValue = getter(instance);
        bool optionalBoolean = reader.GetOptionalBoolean(optionName, defaultValue);
        if (defaultValue == optionalBoolean)
          return;
        setter(instance, optionalBoolean);
      });
    }

    private static SOptionsLoader<TContainer>.DLoadOptionalValue MakeInt32(
      MemberInfo member,
      string optionName)
    {
      SOptionsLoader<TContainer>.Expressions.GetValue<int> getter = SOptionsLoader<TContainer>.Expressions.CreateGetter<int>(member);
      SOptionsLoader<TContainer>.Expressions.SetValue<int> setter = SOptionsLoader<TContainer>.Expressions.CreateSetter<int>(member);
      return (SOptionsLoader<TContainer>.DLoadOptionalValue) ((instance, reader) =>
      {
        int defaultValue = getter(instance);
        int optionalInt32 = reader.GetOptionalInt32(optionName, defaultValue);
        if (defaultValue == optionalInt32)
          return;
        setter(instance, optionalInt32);
      });
    }

    private static SOptionsLoader<TContainer>.DLoadOptionalValue MakeString(
      MemberInfo member,
      string optionName)
    {
      SOptionsLoader<TContainer>.Expressions.GetValue<string> getter = SOptionsLoader<TContainer>.Expressions.CreateGetter<string>(member);
      SOptionsLoader<TContainer>.Expressions.SetValue<string> setter = SOptionsLoader<TContainer>.Expressions.CreateSetter<string>(member);
      return (SOptionsLoader<TContainer>.DLoadOptionalValue) ((instance, reader) =>
      {
        string defaultValue = getter(instance);
        string optionalString = reader.GetOptionalString(optionName, defaultValue);
        if (!(defaultValue != optionalString))
          return;
        setter(instance, optionalString);
      });
    }

    private delegate void DLoadOptionalValue(TContainer instance, IOptionsReader reader);

    private static class Expressions
    {
      public static SOptionsLoader<TContainer>.Expressions.GetValue<TValue> CreateGetter<TValue>(
        MemberInfo memberInfo)
      {
        if (memberInfo.MemberType == MemberTypes.Property && !((PropertyInfo) memberInfo).CanRead)
          throw new InvalidOperationException(string.Format("The property [{0}.{1}] is not readable.", (object) memberInfo.DeclaringType, (object) memberInfo.Name));
        ParameterExpression parameterExpression = Expression.Parameter(TypeCache<TContainer>.Type);
        return Expression.Lambda<SOptionsLoader<TContainer>.Expressions.GetValue<TValue>>((Expression) Expression.PropertyOrField((Expression) parameterExpression, memberInfo.Name), parameterExpression).Compile();
      }

      public static SOptionsLoader<TContainer>.Expressions.SetValue<TValue> CreateSetter<TValue>(
        MemberInfo member)
      {
        switch (member.MemberType)
        {
          case MemberTypes.Field:
            return SOptionsLoader<TContainer>.Expressions.CreateFieldSetter<TValue>((FieldInfo) member);
          case MemberTypes.Property:
            return SOptionsLoader<TContainer>.Expressions.CreatePropertySetter<TValue>((PropertyInfo) member);
          default:
            throw new NotSupportedException(member.MemberType.ToString());
        }
      }

      private static SOptionsLoader<TContainer>.Expressions.SetValue<TValue> CreatePropertySetter<TValue>(
        PropertyInfo propertyInfo)
      {
        if (!propertyInfo.CanWrite)
          throw new InvalidOperationException(string.Format("The property [{0}.{1}] is not readable.", (object) propertyInfo.DeclaringType, (object) propertyInfo.Name));
        ParameterExpression parameterExpression1 = Expression.Parameter(TypeCache<TContainer>.Type);
        MemberExpression memberExpression = Expression.PropertyOrField((Expression) parameterExpression1, propertyInfo.Name);
        ParameterExpression parameterExpression2 = Expression.Parameter(TypeCache<TValue>.Type);
        return Expression.Lambda<SOptionsLoader<TContainer>.Expressions.SetValue<TValue>>((Expression) Expression.Assign((Expression) memberExpression, (Expression) parameterExpression2), parameterExpression1, parameterExpression2).Compile();
      }

      private static SOptionsLoader<TContainer>.Expressions.SetValue<TValue> CreateFieldSetter<TValue>(
        FieldInfo fieldInfo)
      {
        System.Type type1 = TypeCache<TContainer>.Type;
        System.Type type2 = TypeCache<TValue>.Type;
        DynamicMethod dynamicMethod = new DynamicMethod("Set_" + fieldInfo.Name, (System.Type) null, new System.Type[2]
        {
          type1,
          type2
        }, type1, true);
        ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Stfld, fieldInfo);
        ilGenerator.Emit(OpCodes.Ret);
        return (SOptionsLoader<TContainer>.Expressions.SetValue<TValue>) dynamicMethod.CreateDelegate(typeof (SOptionsLoader<TContainer>.Expressions.SetValue<TValue>));
      }

      internal delegate TValue GetValue<out TValue>(TContainer instance);

      internal delegate void SetValue<in TValue>(TContainer instance, TValue value);
    }
  }
}
