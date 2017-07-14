namespace Core.Factory
{
    using Administration;

    public static class SettingFactory
    {
        public static SettingManager SettingsManager()
        {
            return new SettingManager();
        }
    }
}