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

            if (true)
            {
                var userMonitorWorker = new UserMonitorWorker();
                RegisterWorker(userMonitorWorker);
            }

            return true;
        }
    }
}