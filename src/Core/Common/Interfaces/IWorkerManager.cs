using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Common.Interfaces
{
    using global::Hangfire.Server;

    public interface IWorkerManager
    {
        void Start(PerformContext performContext);
    }
}
