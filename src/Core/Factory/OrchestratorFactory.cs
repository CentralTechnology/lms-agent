namespace Core.Factory
{
    using Users;
    using Veeam;

    public static class OrchestratorFactory
    {
        public static OrchestratorManager Orchestrator()
        {
            return new OrchestratorManager();
        }

        public static UserOrchestrator UserOrchestrator()
        {
            return new UserOrchestrator();
        }

        public static VeeamOrchestrator VeeamOrchestrator()
        {
            return new VeeamOrchestrator();
        }
    }
}