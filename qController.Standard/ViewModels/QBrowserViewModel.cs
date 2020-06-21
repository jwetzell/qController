using QSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace qController.ViewModels
{
    public class QBrowserViewModel : INotifyPropertyChanged
    {
        QBrowser browser;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<QServerViewModel> ServersGrouped { get; set; }
        public QBrowserViewModel(QBrowser browser)
        {
            this.browser = browser;
            ServersGrouped = new ObservableCollection<QServerViewModel>();

            this.browser.ServerFound += OnServerFound;
        }

        private void OnServerFound(object source, QServerFoundArgs args)
        {
            ServersGrouped.Add(new QServerViewModel(args.server));

        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
