namespace Core.Veeam
{
    using Backup.Common;
    using DBManager;
    using Factory;

    public abstract class VeeamVersion
    {
        protected LocalDbAccessor LocalDbAccessor;

        protected VeeamVersion()
        {
            LocalDbAccessor = new LocalDbAccessor(VeeamFactory.VeeamManager().GetConnectionString());
        }

        protected VeeamVersion(string connectionString)
        {
            LocalDbAccessor = new LocalDbAccessor(connectionString);
        }

        public abstract int GetProtectedVms();

        public abstract int GetProtectedVms(EPlatform platform);
    }
}