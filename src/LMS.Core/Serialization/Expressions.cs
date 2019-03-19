namespace LMS.Core.Serialization
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class Expressions
  {
    public static GetValue CreateGetter(Type instanceType, string memberName)
    {
      ParameterExpression instance = Expression.Parameter(ReflectionTypes.Object);
      return Expression.Lambda<GetValue>((Expression) Expression.Convert((Expression) Expression.PropertyOrField((Expression) Expressions.UnboxInstanceFromObjectRef(instance, instanceType), memberName), ReflectionTypes.Object), instance).Compile();
    }

    public static SetValue CreatePropertySetter(
      Type instanceType,
      string memberName,
      Type valueType)
    {
      MemberExpression memberExpression = Expression.PropertyOrField((Expression) Expressions.UnboxInstanceFromObjectRef(Expression.Parameter(ReflectionTypes.ObjectRef), instanceType), memberName);
      UnaryExpression unaryExpression = Expression.Convert((Expression) Expression.Parameter(ReflectionTypes.Object), valueType);
      Expression<Expression<SetValue>> x = Expression.Lambda<Expression<SetValue>>(Expression.Assign(memberExpression, unaryExpression));
      return x.Compile().Compile();

      //return (((Expression<SetValue>) ((parameterExpression1, parameterExpression2) => Expression.Assign((Expression) memberExpression, (Expression) unaryExpression))).Compile());
    }

    public static SetValue CreateFieldSetter(FieldInfo fieldInfo)
    {
      Type declaringType = fieldInfo.DeclaringType;
      Type fieldType = fieldInfo.FieldType;
      DynamicMethod dynamicMethod = new DynamicMethod("Set_" + fieldInfo.Name, (Type) null, new Type[2]
      {
        ReflectionTypes.ObjectRef,
        ReflectionTypes.Object
      }, declaringType, true);
      ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldind_Ref);
      Expressions.UnboxInstanceFromObjectRef(ilGenerator, declaringType);
      ilGenerator.Emit(OpCodes.Ldarg_1);
      ilGenerator.Emit(OpCodes.Unbox_Any, fieldType);
      ilGenerator.Emit(OpCodes.Stfld, fieldInfo);
      ilGenerator.Emit(OpCodes.Ret);
      return (SetValue) dynamicMethod.CreateDelegate(typeof (SetValue));
    }

    private static UnaryExpression UnboxInstanceFromObjectRef(
      ParameterExpression instance,
      Type instanceType)
    {
      if (!instanceType.IsValueType)
        return Expression.Convert((Expression) instance, instanceType);
      return Expression.Unbox((Expression) instance, instanceType);
    }

    private static void UnboxInstanceFromObjectRef(ILGenerator il, Type instanceType)
    {
      if (instanceType.IsValueType)
        il.Emit(OpCodes.Unbox, instanceType);
      else
        il.Emit(OpCodes.Castclass, instanceType);
    }
  }
}
