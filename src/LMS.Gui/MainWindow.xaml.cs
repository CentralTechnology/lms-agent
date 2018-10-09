using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LMS.Gui
{
    using System.Reactive.Linq;
    using System.ServiceProcess;
    using System.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();

            Loaded += (s, e) => DataContext = _viewModel;
            Closed += (s, e) => ((MainWindowViewModel) DataContext).Dispose();

            if (_viewModel.ServiceInstalled)
            {
               var restart = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                    h => RestartServiceButton.Click += h, h => RestartServiceButton.Click -= h);

                restart.Subscribe(_ =>
                {
                    var serviceController = new ServiceController("LicenseMonitoringSystem");
                    try
                    {
                        serviceController.Stop();
                        serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                        _viewModel.ServiceStatus = serviceController.Status.ToString();
                        serviceController.Start();
                        serviceController.WaitForStatus(ServiceControllerStatus.Running);
                        _viewModel.ServiceStatus =  serviceController.Status.ToString();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.InnerException?.Message, "oh dear!");
                    }

                    _viewModel.PendingChanges = false;
                });
            }

        }
    }
}
