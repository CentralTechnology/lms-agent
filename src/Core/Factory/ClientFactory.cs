namespace Core.Factory
{
    using Common.Client;

    public static class ClientFactory
    {
        public static LicenseGroupClient LicenseGroupClient()
        {
            return new LicenseGroupClient();
        }

        public static LicenseUserClient LicenseUserClient()
        {
            return new LicenseUserClient();
        }

        public static LicenseUserGroupClient LicenseUserGroupClient()
        {
            return new LicenseUserGroupClient();
        }

        public static PortalClient PortalClient()
        {
            return new PortalClient();
        }

        public static ProfileClient ProfileClient()
        {
            return new ProfileClient();
        }

        public static SupportUploadClient SupportUploadClient()
        {
            return new SupportUploadClient();
        }
    }
}