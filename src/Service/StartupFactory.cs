namespace Service
{
    public static class StartupFactory
    {
        public static StartupManager StartupManager()
        {
            return new StartupManager();
        }
    }
}