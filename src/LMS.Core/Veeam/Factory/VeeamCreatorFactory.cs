using LMS.Core.Common;
using System;

namespace LMS.Core.Veeam.Factory
{
    public class VeeamCreatorFactory
    {
        private VeeamCreatorFactory() { }

        private readonly Version _applicationVersion;

        public VeeamCreatorFactory(Version applicationVersion) => _applicationVersion = applicationVersion;

        public VeeamCreator GetCreator()
        {
            if (_applicationVersion.Minor >= 0 && _applicationVersion.Minor < 5)
            {
                return new Veeam90Creator(_applicationVersion);
            }

            if (_applicationVersion.Minor == 5 && _applicationVersion.Minor == 5 && _applicationVersion.Revision <= 1038)
            {
                return new Veeam9501038Creator(_applicationVersion);
            }

            if (_applicationVersion.Minor == 5 && _applicationVersion.Minor == 5 && _applicationVersion.Build < 4)
            {
                return new Veeam95Creator(_applicationVersion);
            }

            if (_applicationVersion.Minor == 5 && _applicationVersion.Minor == 5 && _applicationVersion.Build == 4)
            {
                return new Veeam9542615Creator(_applicationVersion);
            }

            throw ExceptionFactory.CreateUnsupportedVersionException("version {0} of Veeam Backup & Replication is unsupported", _applicationVersion);
        }
    }
}
