using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal static class DatabaseVersionParser
    {
        public static IDatabaseVersion Parse(string version)
        {
            if (string.IsNullOrEmpty(version))
                throw new ArgumentNullException(nameof (version));
            int[] array = ((IEnumerable<string>) version.Split(';')).Select<string, int>(new Func<string, int>(int.Parse)).ToArray<int>();
            switch (array.Length)
            {
                case 1:
                    return ProductDatabaseVersion.Create(array[0], array[0], new DatabaseVersionType[1]);
                case 2:
                    return ProductDatabaseVersion.Create(array[0], array[1], DatabaseVersionType.Schema, DatabaseVersionType.Content);
                default:
                    throw new ArgumentException(nameof (version));
            }
        }

        private static class Consts
        {
            public const char Separator = ';';
        }
    }
}
