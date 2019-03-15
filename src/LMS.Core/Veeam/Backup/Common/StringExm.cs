using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class StringExm
    {
        public static bool EqualsNoCase(this string str, string testStr)
        {
            return str.Equals(testStr, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool Contains(this string str, string substring, StringComparison comparisonType)
        {
            if (object.ReferenceEquals((object) str, (object) substring))
                return true;
            if (object.ReferenceEquals((object) str, (object) null) || object.ReferenceEquals((object) substring, (object) null))
                return false;
            return str.IndexOf(substring, comparisonType) >= 0;
        }
    }
}
