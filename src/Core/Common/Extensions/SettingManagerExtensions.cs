using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    using Abp.Extensions;
    using Abp.Threading;
    using Administration;

    public static class SettingManagerExtensions
    {
        public static string GetSettingValue(this SettingManager settingManager, string name)
        {
            return AsyncHelper.RunSync(() => settingManager.GetSettingValueAsync(name));
        }

        public static T GetSettingValue<T>(this SettingManager settingManager, string name)
            where T : struct
        {
            return AsyncHelper.RunSync(() => settingManager.GetSettingValueAsync<T>(name));
        }

        public static async Task<T> GetSettingValueAsync<T>(this SettingManager settingManager, string name)
            where T : struct
        {
            return (await settingManager.GetSettingValueAsync(name)).To<T>();
        }

        public static void ChangeSettingForApplication(this SettingManager settingManager, string name, string value)
        {
            AsyncHelper.RunSync(() => settingManager.ChangeSettingForApplicationAsync(name, value));
        }
    }
}
