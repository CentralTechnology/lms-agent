namespace Core.Factory
{
    using Users;

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
    }
}