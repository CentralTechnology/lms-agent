using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
