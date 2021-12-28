using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

using QControlKit;
using QControlKit.Events;
using System;
using System.Threading.Tasks;
using Serilog;

namespace qController.ViewModels
{
    public class QBrowserViewModel : INotifyPropertyChanged
    {
        QBrowser browser;
        
        public event PropertyChangedEventHandler PropertyChanged;
        public bool autoUpdate = false;
        public ObservableCollection<QServerViewModel> ServersGrouped { get; set; }
        public QBrowserViewModel(QBrowser browser)
        {
            this.browser = browser;
            ServersGrouped = new ObservableCollection<QServerViewModel>();

            this.browser.ServerFound += OnServerFound;
            this.browser.ServerLost += OnServerLost;
            Device.StartTimer(TimeSpan.FromSeconds(4), () =>
            {
                Task.Run(() =>
                {
                    if (autoUpdate)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Log.Debug("[QBrowserViewModel] QBrowser probe triggered");
                            this.browser.ProbeForQLabInstances();
                        });
                    }
                });
                return true;
            });

            
            
        }

        private void OnServerFound(object source, QServerFoundArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Log.Debug($"[QBrowserViewModel] adding server: {args.server.description}");
                ServersGrouped.Add(new QServerViewModel(args.server));
            });
        }


        private void OnServerLost(object source, QServerLostArgs args)
        {
            QServerViewModel serverToRemove = null;

            foreach(QServerViewModel serverGroup in ServersGrouped)
            {
                if (serverGroup.host.Equals(args.server.host))
                {
                    serverToRemove = serverGroup;
                    break;
                }
            }

            if(serverToRemove != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Log.Debug($"[QBrowserViewModel] removing server: {args.server.description}");
                    ServersGrouped.Remove(serverToRemove);
                });
            }
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void InitiateScan()
        {
            if (!autoUpdate)
            {
                Log.Debug("[QBrowserViewModel] Manual Scan Initiated");
                browser.ProbeForQLabInstances();
            }
        }
    }
}
