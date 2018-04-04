namespace LMS.Service
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using CommandLine;
    using CommandLine.Text;

        [Verb("run", HelpText = "Run a monitor")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public class RunOptions
        {
            public RunOptions(Monitor monitor = Monitor.None, bool verbose = false, bool skipStartup = false)
            {
                Monitor = monitor;
                SkipStartup = skipStartup;
                Verbose = verbose;
            }

            [Option('m', "monitor", Default = Monitor.None, HelpText = "Runs the requested monitor.")]
            public Monitor Monitor { get; }

            [Option('s', "skip-startup", Default = false, HelpText = "Avoids running the startup process which checks for api credentials.")]
            public bool SkipStartup { get; }

            [Option('v', "verbose", Default = false)]
            public bool Verbose { get; }

            [Usage(ApplicationAlias = "lms.exe")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    yield return new Example("Run \"Users\" monitor", new RunOptions(monitor: Monitor.Users));
                    yield return new Example("Run \"Veeam\" monitor", new RunOptions(monitor: Monitor.Veeam));
                    yield return new Example("Run monitor verbose", new RunOptions(monitor: Monitor.Users, verbose: true));
                    yield return new Example("Run monitor, but skip startup", new RunOptions(monitor: Monitor.Users, skipStartup: true));
                }
            }
        }
    
}