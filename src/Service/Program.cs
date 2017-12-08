namespace LMS.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Abp;
    using Abp.Collections.Extensions;
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Timing;
    using Castle.Facilities.Logging;
    using CommandLine;
    using CommandLine.Text;
    using Common.Constants;
    using Core.Configuration;
    using Topshelf;

    class Runner
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Clock.Provider = ClockProviders.Utc;

            if (args.IsNullOrEmpty())
            {
                HostFactory.Run(x =>
                {
                    x.Service<LMSService>(sc =>
                    {
                        sc.ConstructUsing<LMSService>(s => new LMSService());

                        sc.WhenStarted((tc, hostControl) => tc.Start(hostControl));
                        sc.WhenStopped((tc, hostControl) => tc.Stop(hostControl));
                    });

                    x.UseLog4Net();
                    x.RunAsLocalSystem();
                    x.SetServiceName(Constants.ServiceName);
                    x.SetDisplayName(Constants.ServiceDisplayName);
                    x.SetDescription(Constants.ServiceDescription);
                    x.StartAutomatically();
                });
            }

            Parser.Default.ParseArguments<UpdateOptions, RunOptions>(args)
                .WithParsed<UpdateOptions>(opts =>
                {
                    using (var bootstrapper = AbpBootstrapper.Create<LMSServiceModule>())
                    {
                        bootstrapper.IocManager
                            .IocContainer
                            .AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));

                        bootstrapper.Initialize();

                        using (var settingsManager = bootstrapper.IocManager.ResolveAsDisposable<ISettingManager>())
                        {
                            if (opts.AccountId != default(int))
                            {
                                settingsManager.Object.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, opts.AccountId.ToString());
                                Console.WriteLine($"Account ID: {opts.AccountId}");
                            }

                            if (opts.DeviceId != default(Guid))
                            {
                                settingsManager.Object.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, opts.DeviceId.ToString());
                                Console.WriteLine($"Device ID: {opts.DeviceId}");
                            }

                            if (opts.PdcOverride.HasValue)
                            {
                                settingsManager.Object.ChangeSettingForApplication(AppSettingNames.PrimaryDomainControllerOverride, opts.PdcOverride.Value.ToString());
                                Console.WriteLine($"PDC Override: {opts.PdcOverride.Value}");
                            }

                            if (opts.UsersOverride.HasValue)
                            {
                                settingsManager.Object.ChangeSettingForApplication(AppSettingNames.UsersOverride, opts.UsersOverride.Value.ToString());
                                Console.WriteLine($"Users Override: {opts.UsersOverride.Value}");
                            }

                            if (opts.VeeamOverride.HasValue)
                            {
                                settingsManager.Object.ChangeSettingForApplication(AppSettingNames.VeeamOverride, opts.VeeamOverride.Value.ToString());
                                Console.WriteLine($"Veeam Override: {opts.VeeamOverride.Value}");
                            }
                        }
                    }

                })
                .WithParsed<RunOptions>(ConsoleHost.Run);
        }
    }

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

    [Verb("update", HelpText = "Update settings")]
    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class UpdateOptions
    {
        private readonly bool? _pdcOverride;
        private readonly bool? _usersOverride;
        private readonly bool? _veeamOverride;
        public UpdateOptions()
        {

        }
        public UpdateOptions(bool? pdcOverride = null, bool? usersOverride =null, bool? veeamOverride = null)
        {
            _pdcOverride = pdcOverride;
            _usersOverride = usersOverride;
            _veeamOverride = veeamOverride;
        }
        public UpdateOptions(int accountId)
        {
            AccountId = accountId;
        }

        public UpdateOptions(Guid deviceId)
        {
            DeviceId = deviceId;
        }

        public UpdateOptions(int accountId, Guid deviceId)
        {
            AccountId = accountId;
            DeviceId = deviceId;
        }

        [Option('p', "pdc-override", HelpText = "Allows the users monitor to run from a member server")]
        public bool? PdcOverride => _pdcOverride;

        [Option("force-users", HelpText = "Enables the user monitoring to run even if the startup checks fail")]
        public bool? UsersOverride => _usersOverride;

        [Option("force-veeam", HelpText = "Enables the veeam monitoring to run even if the startup checks fail")]
        public bool? VeeamOverride => _veeamOverride;

        [Option('a', "account", HelpText = "Autotask account id")]
        public int AccountId { get; set; }

        [Option('d', "device", HelpText = "CentraStage device id")]
        public Guid DeviceId { get; set; }

        [Usage(ApplicationAlias = "lms.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Update Autotask Account", new UpdateOptions(accountId: 12345));
                yield return new Example("Update CentraStage Device", new UpdateOptions(deviceId: Guid.NewGuid()));
                yield return new Example("Enable PDC override", new UpdateOptions(pdcOverride: true));
                yield return new Example("Force user monitoring", new UpdateOptions(usersOverride: true));
                yield return new Example("Force veeam monitoring", new UpdateOptions(veeamOverride: true));
            }
        }
    }

    public enum Monitor
    {
        None = 0,
        Users = 1,
        Veeam = 2
    }
}