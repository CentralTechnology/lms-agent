using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    internal static class SOptionsLoader
    {
        public static void LoadOptions<TContainer>(TContainer container, IOptionsReader reader) where TContainer : class
        {
            SOptionsLoader<TContainer>.LoadOptions(container, reader);
        }
    }
}
