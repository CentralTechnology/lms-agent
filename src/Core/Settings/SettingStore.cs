using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Settings
{
    using Abp.Configuration;

    public class SettingStore : ISettingStore
    {
        public Task<SettingInfo> GetSettingOrNullAsync(int? tenantId, long? userId, string name)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(SettingInfo setting)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(SettingInfo setting)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(SettingInfo setting)
        {
            throw new NotImplementedException();
        }

        public Task<List<SettingInfo>> GetAllListAsync(int? tenantId, long? userId)
        {
            throw new NotImplementedException();
        }
    }
}
