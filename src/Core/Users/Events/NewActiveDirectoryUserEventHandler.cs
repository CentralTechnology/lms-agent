namespace LMS.Users.Events
{
    using System;
    using System.Text.RegularExpressions;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Events.Bus.Handlers;
    using Abp.Extensions;
    using Castle.Core.Logging;
    using Common.Extensions;
    using Configuration;
    using Dto;
    using Managers;
    using OData;

    public class NewActiveDirectoryUserEventHandler : IEventHandler<NewActiveDirectoryUserEventData>, ITransientDependency
    {
        private readonly IActiveDirectoryManager _activeDirectoryManager;
        private readonly object _newUserLock = new object();
        private readonly IPortalManager _portalManager;
        private readonly ISettingManager _settingManager;
        private readonly IUserManager _userManager;

        public NewActiveDirectoryUserEventHandler(
            IActiveDirectoryManager activeDirectoryManager,
            IPortalManager portalManager,
            IUserManager userManager,
            ISettingManager settingManager)
        {
            Logger = NullLogger.Instance;

            _activeDirectoryManager = activeDirectoryManager;
            _portalManager = portalManager;
            _userManager = userManager;
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

                    LicenseUserDto user = _activeDirectoryManager.GetUserByPrincipalName(null, principalName);
                    if (user == null)
                    {
                        Logger.Warn($"Unable to locate the User Principal Name {principalName} in Active Directory. The current Event will be discarded.");
                        return;
                    }

                    int managedSupportId = _settingManager.GetSettingValue<int>(AppSettingNames.ManagedSupportId);
                    if (managedSupportId == default(int))
                    {
                        Logger.Warn("Managed Support Id has not been set in the database. This setting is set with the first successful run of the monitor. Automatic creation of users cannot happen until this has been set. The current Event will be discarded.");
                        return;
                    }

                    int accountId = _settingManager.GetSettingValue<int>(AppSettingNames.AutotaskAccountId);
                    if (_portalManager.UserExist(user.Id))
                    {
                        _userManager.Update(null, user);
                        Logger.Info($"Updated existing user {user.DisplayName}");
                        return;
                    }

                    _userManager.Add(null, user, managedSupportId, accountId);
                    Logger.Info($"Created new user {user.DisplayName}");
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