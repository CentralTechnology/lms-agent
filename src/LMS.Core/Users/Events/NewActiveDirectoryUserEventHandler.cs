namespace LMS.Core.Users.Events
{
    using System;
    using System.Text.RegularExpressions;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Events.Bus.Handlers;
    using Abp.Extensions;
    using Abp.Threading;
    using Castle.Core.Logging;
    using Configuration;
    using Core.Extensions;
    using Managers;
    using Newtonsoft.Json;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Serilog;
    using Services;
    using ILogger = Serilog.ILogger;

    public class NewActiveDirectoryUserEventHandler : IEventHandler<NewActiveDirectoryUserEventData>, ITransientDependency
    {
        private readonly IActiveDirectoryManager _activeDirectoryManager;
        private readonly object _newUserLock = new object();
        private readonly IPortalService _portalService;
        private readonly ISettingManager _settingManager;

        public NewActiveDirectoryUserEventHandler(
            IActiveDirectoryManager activeDirectoryManager,
            IPortalService portalService,
            ISettingManager settingManager)
        {
            _activeDirectoryManager = activeDirectoryManager;
            _portalService = portalService;
            _settingManager = settingManager;
        }

        private readonly ILogger Logger = Log.ForContext<NewActiveDirectoryUserEventHandler>();

        public void HandleEvent(NewActiveDirectoryUserEventData eventData)
        {
            lock (_newUserLock)
            {
                try
                {
                    string message = eventData.Entry.Message;
                    string principalName = GetUserPrincipalName(message);
                    if (principalName.IsNullOrEmpty())
                    {
                        Logger.Warning("Could not determine the User Principal Name from the event. The current Event will be discarded.");
                        return;
                    }

                    Logger.Information("User Principal Name extracted from the Event as {PrincipleName}", principalName);

                    LicenseUser user = _activeDirectoryManager.GetUserByPrincipalName(principalName);
                    if (user == null)
                    {
                        Logger.Warning("Unable to locate the User Principal Name {PrincipalName} in Active Directory. The current Event will be discarded.", principalName);
                        return;
                    }

                    var remoteUser = _portalService.GetUserById(user.Id);
                    if (remoteUser.Count != 1)
                    {
                        var newUser = LicenseUser.Create(
                            user,
                            _settingManager.GetSettingValue<int>(AppSettingNames.ManagedSupportId),
                            _settingManager.GetSettingValue<int>(AppSettingNames.AutotaskAccountId)
                        );

                        AsyncHelper.RunSync(() => _portalService.AddUserAsync(newUser));
                        Logger.Information("Created: {@NewUser}", newUser);
                        return;
                    }

                    remoteUser[0].UpdateValues(user);
                    AsyncHelper.RunSync(() => _portalService.UpdateUserAsync(remoteUser[0]));

                    Logger.Information("Updated:  {RemoteUser}", remoteUser[0]);
                    Logger.Debug("Update {@RemoteUser}", remoteUser[0]);
                }
                catch (Exception ex)
                {
                    Logger.Error("There was an error while processing the new user.");
                    Logger.Error(ex.Message);
                    Logger.Debug(ex,ex.Message);
                }
            }
        }

        private static string GetUserPrincipalName(string eventMessage)
        {
            const string fieldName = "User Principal Name";

            var expression = new Regex($@"\s*{fieldName}:\s*(.+)\r\n");

            Match match = expression.Match(eventMessage);
            if (match.Success)
            {
                string fullMatch = match.Value.Replace(" ", string.Empty);
                string val = fullMatch.Split(':')[1].RemoveWhitespace();

                return val;
            }

            return string.Empty;
        }
    }
}