namespace LMS.Service
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using global::Hangfire;
    using Microsoft.Owin.Hosting;
    using Topshelf;

    public class LMSService : ServiceControl
    {
        private IDisposable _webapp;
        public bool Start(HostControl hostControl)
        { 
            _webapp = WebApp.Start<Startup>("http://localhost:9000");

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