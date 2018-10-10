using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Gui
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using System.ServiceProcess;
    using Abp.Configuration;
    using Abp.Dependency;
    using Annotations;
    using Core.Configuration;
    using Core.Services.Authentication;
    using Infrastructure;

    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : IDisposable
    {
        private readonly ISettingManager _settingManager;
        private readonly IPortalAuthenticationService _authService;
        private readonly ServiceController _serviceController;
        public Guid DeviceId { get; set; }
        public long AccountId { get; set; }

        public bool IsBusy { get; set; }
        public bool PendingChanges { get; set; }

        public string ServiceStatus { get; set; }

        public bool ServiceInstalled { get; set; }

        private bool _canChangeDeviceId;
        private bool _canChangeAccountId;

        public MainWindowViewModel()
        {
            // resolve dependencies
            _settingManager = IocManager.Instance.Resolve<ISettingManager>();
            _authService = IocManager.Instance.Resolve<IPortalAuthenticationService>();

            try
            {
                _serviceController = new ServiceController("LicenseMonitoringSystem");
                if (_serviceController == null)
                {
                    ServiceInstalled = false;
                    ServiceStatus = "Program not installed.";
                }
                else
                {
                    ServiceInstalled = true;
                    ServiceStatus = _serviceController.Status.ToString();
                }
            }
            catch (Exception ex)
            {
                ServiceInstalled = false;
                ServiceStatus = ex.Message;
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

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public void Dispose()
        {
            IocManager.Instance.Release(_authService);
            IocManager.Instance.Release(_settingManager);
            _disposable.Dispose();
        }
    }
}
