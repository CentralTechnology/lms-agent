using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Factory
{
    using Abp;
    using Common.Constants;
    using Veeam;

    public static class VeeamVersionFactory
    {
        public static Veeam Get(string version)
        {
            switch (version)
            {
                case Constants.VeeamVersion900902:
                    return new VeeamVersion900902().Build();
                case Constants.VeeamVersion9501038:
                    return new VeeamVersion9501038().Build();
                default:
                    throw new AbpException($"Unsupported version of Veeam: {version}");
            }
        }
    }
}
