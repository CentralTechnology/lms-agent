using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.CentraStage
{
    using Abp.Configuration;
    using Abp.Domain.Services;
    using Common.Extensions;
    using Core.Configuration;
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

            foreach (var key in keys)
            {
                try
                {
                    var data = key.GetSubKeyValue(key.GetSubKeyNames(), requestedKeyName: "CentraStage", requestedValue: "DeviceID");
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

        /// <inheritdoc />
        public bool IsValid()
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
                        Logger.Warn("Check Centrastage: FAIL");
                        Logger.Error("Failed to get the centrastage device id from the registry. This application cannot work without the centrastage device id. Please enter it manually through the menu system.");
                        return false;
                    }

                    SettingManager.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, reportedDevice.ToString());
                    deviceId = reportedDevice.To<Guid>();
                }
                else
                {
                    deviceId = storedDevice;
                }

                Logger.Info("Check Centrastage: OK");
                Logger.Info($"Device: {deviceId}");
                return true;
            }
            catch (Exception)
            {
                Logger.Warn("Check Centrastage: FAIL");
                Logger.Error("Failed to get the centrastage device id from the registry. This application cannot work without the centrastage device id. Please enter it manually through the menu system.");
                return false;
            }
        }
    }
}
