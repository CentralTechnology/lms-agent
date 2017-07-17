namespace Core.Factory
{
    using DirectoryServices;

    public static class DirectoryServicesFactory
    {
        public static DirectoryServicesManager DirectoryServicesManager()
        {
            return new DirectoryServicesManager();
        }
    }
}