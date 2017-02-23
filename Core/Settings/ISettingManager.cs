namespace LicenseMonitoringSystem.Core.Settings
{
    using System;
    using System.Collections.Generic;

    public interface ISettingManager
    {

        /// <summary>
        /// </summary>
        void ClearCache();

        /// <summary>
        ///     Creates a default Settings file with an optional
        ///     <param name="accountId">Autotask Account Id</param>
        /// </summary>
        /// <param name="accountId">Autotask Account Id</param>
        void Create(int accountId = 0);

        /// <summary>
        ///     Simple check to see if the Settings file exists.
        /// </summary>
        bool Exists();

        /// <summary>
        ///     This metthod runs each time the service is started. It checks that the required details have been entered into the
        ///     settings file.
        /// </summary>
        void FirstRun();

        /// <summary>
        ///     Gets the account id from the settings file, then fallsback to the api if it is not found.
        /// </summary>
        /// <param name="deviceId"></param>
        int GetAccountId();

        /// <summary>
        ///     Gets the account id from the settings file, then fallsback to the api if it is not found.
        /// </summary>
       // int GetAccountId();

        /// <summary>
        ///     If the <paramref name="registry" /> is false, then it will search in the settings file.
        ///     If the <paramref name="registry" /> is true, then it will search in the registry, avoiding the settings file.
        /// </summary>
        /// <param name="registry"></param>
     //   Guid GetDeviceId(bool registry);

        Guid GetDeviceId();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        List<Monitor> GetMonitors();

        /// <summary>
        ///     Gets the Service Url used for the API.
        /// </summary>
        string GetServiceUrl();

        /// <summary>
        /// </summary>
        void ResetOnStartUp(bool value);

        /// <summary>
        /// </summary>
        /// <param name="accountId"></param>
        void SetAccountId(int accountId);

        /// <summary>
        ///     Sets the application to Debug mode.
        /// </summary>
        /// <param name="value">True or False.</param>
        void SetDebug(bool value);

        /// <summary>
        /// </summary>
        /// <param name="deviceId"></param>
        void SetDeviceId(Guid deviceId);

        bool GetDebug();
    }
}