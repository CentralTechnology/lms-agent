namespace LMS.Autotask
{
    using System;
    using Abp.Configuration;
    using Abp.Domain.Services;
    using Common.Extensions;
    using Core.Configuration;
    using OData;

    public class AutotaskManager : DomainService, IAutotaskManager
    {
        private readonly IPortalManager _portalManager;

        public AutotaskManager(IPortalManager portalManager)
        {
            _portalManager = portalManager;
        }

        /// <inheritdoc />
        public int GetId()
        {
            return SettingManager.GetSettingValue<int>(AppSettingNames.AutotaskAccountId);
        }

        public bool IsValid()
        {
            try
            {
                Guid deviceId = SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();

                int accountId;
                int storedAccount = SettingManager.GetSettingValue<int>(AppSettingNames.AutotaskAccountId);

                if (storedAccount == default(int))
                {
                    int reportedAccount = _portalManager.GetAccountIdByDeviceId(deviceId);

                    if (reportedAccount == default(int))
                    {
                        Logger.Warn("Check Account: FAIL");
                        Logger.Error("Failed to get the autotask account id from the api. This application cannot work without the autotask account id. Please enter it manually through the menu system.");
                        return false;
                    }

                    SettingManager.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, reportedAccount.ToString());
                    accountId = reportedAccount.To<int>();
                }
                else
                {
                    accountId = storedAccount;
                }

                Logger.Info("Check Account: OK");
                Logger.Info($"Account: {accountId}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn("Check Account: FAIL");
                Logger.Error("Failed to get the autotask account id from the api. This application cannot work without the autotask account id. Please enter it manually through the menu system.");
                Logger.Error(ex.Message);
                Logger.Debug("Exception", ex);
                return false;
            }
        }
    }
}
