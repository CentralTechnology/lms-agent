namespace Core.Common.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    public static class ObjectExtensions
    {
        /// <summary>
        ///     Converts given object to a value type using <see cref="Convert.ChangeType(object,TypeCode)" /> method.
        /// </summary>
        /// <param name="obj">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static T To<T>(this object obj)
            where T : struct
        {
            if (typeof(T) == typeof(Guid))
            {
                return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
            }

            if (typeof(T).IsEnum)
            {
                return (T) Enum.Parse(typeof(T), obj.ToString());
            }

            return (T) Convert.ChangeType(obj, typeof(T), CultureInfo.CurrentCulture);
        }
    }
}