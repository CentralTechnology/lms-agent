namespace LMS.Autotask
{
    using System;
    using Abp.Configuration;
    using Abp.Domain.Services;
    using Abp.Logging;
    using Common.Extensions;
    using Configuration;
    using global::Hangfire.Server;
    using OData;

    public class AutotaskManager : DomainService, IAutotaskManager
    {
        private readonly IPortalManager _portalManager;

        public AutotaskManager(IPortalManager portalManager)
        {
            _portalManager = portalManager;
        }
        public int GetId()
        {
            return SettingManager.GetSettingValue<int>(AppSettingNames.AutotaskAccountId);
        }

        public bool IsValid(PerformContext performContext)
        {
            try
            {
                var deviceId = SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId).To<Guid>();

                int accountId;
                int storedAccount;
                try
                {
                    storedAccount = SettingManager.GetSettingValue<int>(AppSettingNames.AutotaskAccountId);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    storedAccount = default(int);
                }

                if (storedAccount == default(int))
                {
                    int reportedAccount = _portalManager.GetAccountIdByDeviceId(deviceId);

                    if (reportedAccount == default(int))
                    {
                        Logger.Log(LogSeverity.Warn, performContext, "Check Account: FAIL");
                        Logger.Log(LogSeverity.Error,
                            performContext, "Failed to get the autotask account id from the api. This application cannot work without the autotask account id. Please enter it manually through the menu system.");

                        return false;
                    }

                    SettingManager.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, reportedAccount.ToString());
                    accountId = reportedAccount.To<int>();
                }
                else
                {
                    accountId = storedAccount;
                }

                Logger.Log(LogSeverity.Info, performContext, "Check Account: OK");
                Logger.Log(LogSeverity.Info, performContext, $"Account: {accountId}");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, performContext, "Check Account: FAIL");
                Logger.Log(LogSeverity.Error, performContext, "Failed to get the autotask account id from the api. This application cannot work without the autotask account id. Please enter it manually through the menu system.");
                Logger.Log(LogSeverity.Error, performContext, ex.Message);
                Logger.Log(LogSeverity.Debug, performContext, ex.Message, ex);

                return false;
            }
        }
    }
}