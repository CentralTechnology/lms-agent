namespace Core.Factory
{
    using Administration;

    public static class SettingFactory
    {
        public static SettingsManager SettingsManager()
        {
            return new SettingsManager();
        }
    }
}