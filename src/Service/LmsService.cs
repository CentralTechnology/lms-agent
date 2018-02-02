namespace LMS.Service
{
    using System;
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Reflection;
    using Abp.Dependency;
    using Common.Extensions;
    using global::Hangfire;
    using LMS.Startup;
    using Microsoft.Owin.Hosting;
    using Topshelf;

    public class LMSService : ServiceControl
    {
        private IDisposable _webapp;
        public bool Start(HostControl hostControl)
        {
            int port = ConfigurationManager.AppSettings.Get("Port").To<int>();
            if (port == default(int))
            {
                port = 9000; // set default
            }
            _webapp = WebApp.Start<Startup>($"http://localhost:{port}");

            using (var startupManager = IocManager.Instance.ResolveAsDisposable<IStartupManager>())
            {
                startupManager.Object.Init(null);
            }
           
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _webapp.Dispose();
            return DisposeServers();
        }

        private static bool DisposeServers()
        {
            try
            {
                var type = Type.GetType("Hangfire.AppBuilderExtensions, Hangfire.Core", throwOnError: false);
                if (type == null)
                {
                    return false;
                }

                var field = type.GetField("Servers", BindingFlags.Static | BindingFlags.NonPublic);
                if (field == null)
                {
                    return false;
                }

                if (!(field.GetValue(null) is ConcurrentBag<BackgroundJobServer> value))
                {
                    return false;
                }

                var servers = value.ToArray();

                foreach (var server in servers)
                {
                    // Dispose method is a blocking one. It's better to send stop
                    // signals first, to let them stop at once, instead of one by one.
                    server.SendStop();
                }

                foreach (var server in servers)
                {
                    server.Dispose();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}