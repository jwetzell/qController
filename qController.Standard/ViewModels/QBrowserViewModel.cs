using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

using QControlKit;
using QControlKit.Events;
using System;
using System.Threading.Tasks;

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
                Task.Run(async () =>
                {
                    if(autoUpdate)
                        this.browser.ProbeForQLabInstances();
                });
                return true;
            });

            
            
        }

        private void OnServerFound(object source, QServerFoundArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                System.Console.WriteLine($"[QBrowserViewModel] adding server: {args.server.description}");
                ServersGrouped.Add(new QServerViewModel(args.server));
            });
        }


        private void OnServerLost(object source, QServerLostArgs args)
        {
            QServerViewModel serverToRemove = null;

            foreach(var serverGroup in ServersGrouped)
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
                    System.Console.WriteLine($"[QBrowserViewModel] removing server: {args.server.description}");
                    ServersGrouped.Remove(serverToRemove);
                });
            }
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
