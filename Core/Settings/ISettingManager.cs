using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Core.Settings
{
    public interface ISettingManager
    {
        /// <summary>
        /// Creates a default Settings file with an optional <param name="accountId">Autotask Account Id</param>.
        /// </summary>
        /// <param name="accountId">Autotask Account Id</param>
        void Create(long accountId = 0);

        /// <summary>
        ///     Simple check to see if the Settings file exists.
        /// </summary>
        /// <returns>Returns True or False.</returns>
        bool Exists();

        /// <summary>
        ///     Sets the application to Debug mode.
        /// </summary>
        /// <param name="value">True or False.</param>
        void SetDebug(bool value);

        /// <summary>
        ///  Gets the CentraStage DeviceID.
        /// </summary>
        /// <returns><see cref="Guid">DeviceID</see></returns>
        Guid GetDeviceId();

        /// <summary>
        /// Gets the Autotask AccountID.
        /// </summary>
        /// <returns><see cref="long">AccountID</see></returns>
        long GetAccountId();

        /// <summary>
        ///  Gets the Service Url used for the API.
        /// </summary>
        /// <returns>Returns the Service Url as a <see cref="string"/></returns>
        string GetServiceUrl();
    }
}
