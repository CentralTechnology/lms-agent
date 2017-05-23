using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration
{
    using Abp.Domain.Services;
    using Castle.Core.Logging;

    public interface ISettingsManager : IDomainService
    {
        /// <summary>
        /// </summary>
        /// <param name="settings"></param>
        SettingsData Update(SettingsData settings);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        SettingsData Read();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enableDebug"></param>
        /// <returns></returns>
        LoggerLevel UpdateLoggerLevel(bool enableDebug);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        LoggerLevel ReadLoggerLevel();

        /// <summary>
        /// 
        /// </summary>
        void Validate();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetClientVersion();
    }
}
