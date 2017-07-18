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
        public static VeeamVersion Get(string version)
        {
            switch (version)
            {
                case Constants.VeeamVersion900902:
                    return new VeeamVersion900902();
                default:
                    throw new AbpException($"Unsupported version of Veeam: {version}");
            }
        }
    }
}
