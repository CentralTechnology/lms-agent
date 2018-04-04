namespace LMS.CentraStage
{
    using System;
    using Abp.Configuration;
    using Abp.Domain.Services;
    using Abp.Logging;
    using Common.Extensions;
    using Configuration;
    using Core.Common.Extensions;
    using global::Hangfire.Server;
    using Microsoft.Win32;

    [Obsolete]
    public class CentraStageManager : DomainService, ICentraStageManager
    {
        /// <inheritdoc />
        public Guid? GetIdFromRegistry()
        {
#if DEBUG
            return new Guid("2a5d23dc-1b9a-9341-32c6-1160a5df7883");
#endif
            var keys = new[]
            {
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE"),
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE")
            };

            foreach (RegistryKey key in keys)
            {
                try
                {
                    (bool exist, string value) data = key.GetSubKeyValue(key.GetSubKeyNames(), requestedKeyName: "CentraStage", requestedValue: "DeviceID");
                    if (data.exist)
                    {
                        bool valid = Guid.TryParse(data.value, out Guid csId);
                        if (valid)
                        {
                            return csId;
                        }
                    }
                }
                catch (NullReferenceException)
                {
                }
            }

            return null;
        }

        private void HandleNullDeviceId(PerformContext performContext)
        {
            Logger.Log(LogSeverity.Warn, performContext, "Check Centrastage: FAIL");
            Logger.Log(LogSeverity.Error,
                performContext, "Failed to get the centrastage device id from the registry. This application cannot work without the centrastage device id. Please enter it manually through the menu system.");
        }

        private void HandleFoundDeviceId(PerformContext performContext, Guid deviceId)
        {
            SettingManager.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, deviceId.ToString());
            Logger.Log(LogSeverity.Info, performContext, "Check Centrastage: OK");
            Logger.Log(LogSeverity.Info, performContext, $"Device: {deviceId}");
        }

        public bool IsValid(PerformContext performContext)
        {
            try
            {
                var centraStageDeviceId = SettingManager.GetSettingValueForApplication(AppSettingNames.CentrastageDeviceId);
                if (Guid.TryParse(centraStageDeviceId, out Guid deviceId))
                {
                    if (deviceId == default(Guid))
                    {
                        return LookHarder();
                    }

                    HandleFoundDeviceId(performContext, deviceId);
                    return true;
                }

                if (string.IsNullOrEmpty(centraStageDeviceId) || string.IsNullOrWhiteSpace(centraStageDeviceId))
                {
                    return LookHarder();
                }

                return false;
            }
            catch (Exception ex)
            {
                HandleNullDeviceId(performContext);
                Logger.Log(LogSeverity.Error, performContext, ex.Message, ex);
                return false;
            }

            bool LookHarder()
            {
                Guid? reportedDevice = GetIdFromRegistry();
                if (reportedDevice == null)
                {
                    HandleNullDeviceId(performContext);
                    return false;
                }

                HandleFoundDeviceId(performContext, reportedDevice.Value);
                return true;
            }
        }
    }
}