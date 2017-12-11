namespace LMS.CentraStage
{
    using System;
    using Abp.Configuration;
    using Abp.Domain.Services;
    using Abp.Logging;
    using Common.Extensions;
    using Configuration;
    using global::Hangfire.Server;
    using Microsoft.Win32;

    public class CentraStageManager : DomainService, ICentraStageManager
    {
        /// <inheritdoc />
        public Guid? GetId()
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

        /// <param name="performContext"></param>
        /// <inheritdoc />
        public bool IsValid(PerformContext performContext)
        {
            try
            {
                Guid deviceId;
                var storedDevice = SettingManager.GetSettingValueForApplication(AppSettingNames.CentrastageDeviceId).To<Guid>();
                if (storedDevice == default(Guid))
                {
                    Guid? reportedDevice = GetId();

                    if (reportedDevice == null)
                    {
                        Logger.Log(LogSeverity.Warn, performContext, "Check Centrastage: FAIL");
                        Logger.Log(LogSeverity.Error,
                            performContext, "Failed to get the centrastage device id from the registry. This application cannot work without the centrastage device id. Please enter it manually through the menu system.");

                        return false;
                    }

                    SettingManager.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, reportedDevice.ToString());
                    deviceId = reportedDevice.To<Guid>();
                }
                else
                {
                    deviceId = storedDevice;
                }

                Logger.Log(LogSeverity.Info, performContext, "Check Centrastage: OK");
                Logger.Log(LogSeverity.Info, performContext, $"Device: {deviceId}");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, performContext, "Check Centrastage: FAIL");
                Logger.Log(LogSeverity.Error, performContext, "Failed to get the centrastage device id from the registry. This application cannot work without the centrastage device id. Please enter it manually through the menu system.");
                Logger.Log(LogSeverity.Error, performContext, ex.Message);
                Logger.Log(LogSeverity.Debug, performContext, ex.Message, ex);

                return false;
            }
        }
    }
}