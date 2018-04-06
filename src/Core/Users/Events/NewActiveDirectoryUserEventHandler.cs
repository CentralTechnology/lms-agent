namespace LMS.Users.Events
{
    using System;
    using System.Text.RegularExpressions;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Events.Bus.Handlers;
    using Abp.Extensions;
    using Abp.Threading;
    using Castle.Core.Logging;
    using Common.Extensions;
    using Configuration;
    using Core.Services;
    using Dto;
    using Managers;
    using Newtonsoft.Json;
    using Portal.LicenseMonitoringSystem.Users.Entities;

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
            Logger = NullLogger.Instance;

            _activeDirectoryManager = activeDirectoryManager;
            _portalService = portalService;
            _settingManager = settingManager;
        }

        public ILogger Logger { get; set; }

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
                        Logger.Warn("Could not determine the User Principal Name from the event. The current Event will be discarded.");
                        return;
                    }

                    Logger.Info($"User Principal Name extracted from the Event as {principalName}");

                    LicenseUser user = _activeDirectoryManager.GetUserByPrincipalName(null, principalName);
                    if (user == null)
                    {
                        Logger.Warn($"Unable to locate the User Principal Name {principalName} in Active Directory. The current Event will be discarded.");
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
                        Logger.Info($"Created: {newUser}");
                        Logger.Debug($"{JsonConvert.SerializeObject(newUser, Formatting.Indented)}");
                        return;
                    }

                    remoteUser[0].UpdateValues(user);
                    AsyncHelper.RunSync(() => _portalService.UpdateUserAsync(remoteUser[0]));


                    Logger.Info($"Updated:  {remoteUser}");
                    Logger.Debug($"{JsonConvert.SerializeObject(remoteUser, Formatting.Indented)}");
                }
                catch (Exception ex)
                {
                    Logger.Error("There was an error while processing the new user.");
                    Logger.Error(ex.Message, ex);
                    Logger.Debug(ex.Message, ex);
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