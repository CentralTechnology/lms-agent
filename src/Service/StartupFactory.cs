using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class StartupFactory
    {
        public static  StartupManager StartupManager()
        {
            return new StartupManager();
        }
    }
}
