namespace Core.Factory
{
    using Veeam;

    public static class VeeamFactory
    {
        public static VeeamManager VeeamManager()
        {
            return new VeeamManager();
        }
    }
}
