namespace LMS.Common.Extensions
{
    using System;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }
    }
}
