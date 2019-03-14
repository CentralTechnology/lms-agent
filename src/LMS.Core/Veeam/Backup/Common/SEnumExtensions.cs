using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class SEnumExtensions
    {
        public static T Parse<T>(string stringValue) where T : struct, IConvertible
        {
            return SEnumExtensions.Parse<T>(stringValue, false);
        }

        public static T Parse<T>(string stringValue, bool ignoreCase) where T : struct, IConvertible
        {
            if (string.IsNullOrEmpty(stringValue))
                throw new ArgumentException("String is null or empty.");
            System.Type enumType = typeof (T);
            if (!enumType.IsEnum)
                throw new ArgumentException("Type must be Enum.");
            return (T) Enum.Parse(enumType, stringValue, ignoreCase);
        }

        public static T ToObject<T>(int value)
        {
            System.Type enumType = typeof (T);
            if (!enumType.IsEnum)
                throw new ArgumentException("Type must be Enum.");
            return (T) Enum.ToObject(enumType, value);
        }

        public static T[] GetValues<T>()
        {
            return (T[]) Enum.GetValues(typeof (T));
        }

        public static TAttribute[] GetEnumValueAttributes<TAttribute>(this Enum value) where TAttribute : System.Attribute
        {
            return ((ICollection<object>) value.GetType().GetMember(value.ToString())[0].GetCustomAttributes(typeof (TAttribute), false)).TransformToArray<object, TAttribute>((Func<object, TAttribute>) (a => (TAttribute) a));
        }

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            System.Type type = enumerationValue.GetType();
            if (!type.IsEnum)
                throw new ArgumentException("EnumerationValue must be of Enum type", nameof (enumerationValue));
            MemberInfo[] member = type.GetMember(enumerationValue.ToString());
            if (member.Length > 0)
            {
                object[] customAttributes = member[0].GetCustomAttributes(typeof (DescriptionAttribute), false);
                if (customAttributes.Length > 0)
                    return ((DescriptionAttribute) customAttributes[0]).Description;
            }
            return enumerationValue.ToString();
        }
    }
}
