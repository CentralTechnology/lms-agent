namespace LMS.Common.Extensions
{
    using System;
    using System.Threading.Tasks;
    using Abp.Threading;
    using Core.Administration;

    [Obsolete]
    public static class SettingManagerExtensions
    {
        public static void ChangeSetting(this SettingManager settingManager, string name, string value)
        {
            AsyncHelper.RunSync(() => settingManager.ChangeSettingAsync(name, value));
        }

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
    }
}