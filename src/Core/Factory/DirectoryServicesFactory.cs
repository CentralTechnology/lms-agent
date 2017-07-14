using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Factory
{
    using DirectoryServices;

    public static class DirectoryServicesFactory
    {
        public static DirectoryServicesManager DirectoryServicesManager()
        {
            return new DirectoryServicesManager();
        }

    }
}
