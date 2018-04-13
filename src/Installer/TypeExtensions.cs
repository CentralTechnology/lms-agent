namespace Installer
{
    using System;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static Assembly GetAssembly(this Type type)
        {
            return Assembly.GetAssembly(type);
        }
    }
}