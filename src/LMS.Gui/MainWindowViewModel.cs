namespace LMS.Gui
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.ServiceProcess;
    using Abp.Configuration;
    using Abp.Dependency;
    using Core.Configuration;
    using Core.Services.Authentication;
    using Infrastructure;
    using PropertyChanged;

    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : IDisposable
    {
        private readonly IPortalAuthenticationService _authService;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly ISettingManager _settingManager;
        private bool _canChangeAccountId;

        private bool _canChangeDeviceId;

        public MainWindowViewModel()
        {
            // resolve dependencies
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();
            _authService = IocManager.Instance.Resolve<IPortalAuthenticationService>();

            try
            {
                ServiceController serviceController = new ServiceController("LicenseMonitoringSystem");

                ServiceInstalled = true;
                ServiceStatus = serviceController.Status.ToString();
                serviceController.DisposeWith(_disposable);
            }
            catch (Exception ex)
            {
                ServiceInstalled = false;
                ServiceStatus = ex.Message;
                return;
            }

            // set initial values
            AccountId = _authService.GetAccount();
            DeviceId = _authService.GetDevice();

            // listen for changes
            this.Changed(p => p.AccountId)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOnDispatcher()
                .Subscribe(accountId =>
                {
                    IsBusy = true;

                    if (_canChangeAccountId)
                    {
                        var prev = _authService.GetAccount();
                        if (prev != AccountId)
                        {
                            PendingChanges = true;
                        }
                    }

                    Debug.WriteLine($"Account ID: {accountId}");
                    _settingManager.ChangeSettingForApplication(AppSettingNames.AutotaskAccountId, accountId.ToString());

                    IsBusy = false;
                    _canChangeAccountId = true;
                })
                .DisposeWith(_disposable);

            this.Changed(p => p.DeviceId)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOnDispatcher()
                .Subscribe(deviceId =>
                {
                    IsBusy = true;
                    if (_canChangeDeviceId)
                    {
                        var prev = _authService.GetDevice();
                        if (prev != DeviceId)
                        {
                            PendingChanges = true;
                        }
                    }

                    Debug.WriteLine($"Device ID: {deviceId}");
                    _settingManager.ChangeSettingForApplication(AppSettingNames.CentrastageDeviceId, deviceId.ToString());

                    IsBusy = false;
                    _canChangeDeviceId = true;
                })
                .DisposeWith(_disposable);

            PendingChanges = false;
        }

        public long AccountId { get; set; }
        public Guid DeviceId { get; set; }

        public bool IsBusy { get; set; }
        public bool PendingChanges { get; set; }

        public bool ServiceInstalled { get; set; }

        public string ServiceStatus { get; set; }

        public void Dispose()
        {
            IocManager.Instance.Release(_authService);
            IocManager.Instance.Release(_settingManager);
            _disposable.Dispose();
        }
    }
}