namespace Core.Administration
{
    using Abp.Domain.Services;
    using Castle.Core.Logging;

    public interface ISettingsManager : IDomainService
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        string GetClientVersion();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        SettingsData Read();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        LoggerLevel ReadLoggerLevel();

        /// <summary>
        /// </summary>
        /// <param name="settings"></param>
        SettingsData Update(SettingsData settings);

        /// <summary>
        /// </summary>
        /// <param name="enableDebug"></param>
        /// <returns></returns>
        LoggerLevel UpdateLoggerLevel(bool enableDebug);

        /// <summary>
        /// </summary>
        void Validate();
    }
}