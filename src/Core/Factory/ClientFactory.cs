namespace Core.Factory
{
    using Common.Client;

    public static class ClientFactory
    {
        public static ProfileClient ProfileClient()
        {
            return new ProfileClient();
        }

        public static PortalClient PortalClient()
        {
            return new PortalClient();
        }

        public static SupportUploadClient SupportUploadClient()
        {
            return new Common.Client.SupportUploadClient();
        }

        public static LicenseUserClient LicenseUserClient()
        {
            return new Common.Client.LicenseUserClient();
        }

        public static LicenseGroupClient LicenseGroupClient()
        {
            return new Common.Client.LicenseGroupClient();
        }

        public static LicenseUserGroupClient LicenseUserGroupClient()
        {
            return new Common.Client.LicenseUserGroupClient();
        }
    }
}