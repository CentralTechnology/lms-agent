using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     removes all whitespace from a string
        /// </summary>
        /// <param name="input">this string object</param>
        /// <returns></returns>
        public static string RemoveWhitespace(this string input)
        {
            if (input == null)
            {
                return null;
            }

            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
