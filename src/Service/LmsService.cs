using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    using Core.Administration;
    using Core.Common.Extensions;
    using Core.Factory;
    using ServiceTimer;
    using Topshelf;
    using Workers;

    public class LmsService : TimerServiceBase
    {
        public override bool Start()
        {
            DefaultLog();

            StartupFactory.StartupManager().Init();

            if (true)
            {
                var userMonitorWorker = new UserMonitorWorker();
                RegisterWorker(userMonitorWorker);
            }

            return true;
        }
    }
}
