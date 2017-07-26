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

        /// <summary>
        ///  Returns the number of Protected Vms
        /// </summary>
        /// <returns></returns>
        public abstract int GetProtectedVms();

        /// <summary>
        /// Returns the number of Protected Vms
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public abstract int GetProtectedVms(EPlatform platform);

        /// <summary>
        ///  Returns the <see cref="Veeam"/> object, ready for the api.
        /// </summary>
        /// <returns></returns>
        public abstract Veeam Build();
    }
}