namespace Service
{
    using ServiceTimer;
    using Workers;

    public class LmsService : TimerServiceBase
    {
        public override bool Start()
        {
            DefaultLog();

            StartupFactory.StartupManager().Init();

            var veeamMonitorWorker = new VeeamMonitorWorker();
            RegisterWorker(veeamMonitorWorker);

            //if (true)
            //{
            //    var userMonitorWorker = new UserMonitorWorker();
            //    RegisterWorker(userMonitorWorker);
            //}

            return true;
        }
    }
}