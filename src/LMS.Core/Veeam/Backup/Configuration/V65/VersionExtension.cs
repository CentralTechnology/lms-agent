using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public static class VersionExtension
    {
        public static int[] ToArray(this Version version)
        {
            if (version == (Version) null)
                throw new ArgumentNullException(nameof (version));
            return new int[4]
            {
                version.Major,
                version.Minor,
                version.Build,
                version.Revision
            };
        }

        public static bool IsEmpty(this Version version)
        {
            return !((IEnumerable<int>) version.ToArray()).Any<int>((Func<int, bool>) (value =>
            {
                if (value != 0)
                    return value != -1;
                return false;
            }));
        }

        public static bool IsMoreThan(this Version version, Version compareVersion)
        {
            return version.CompareTo(compareVersion) > 0;
        }

        public static bool IsLessThan(this Version version, Version compareVersion)
        {
            return version.CompareTo(compareVersion) < 0;
        }
    }
}
