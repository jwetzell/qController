using QControlKit;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace qController.ViewModels
{
    public class QServerViewModel : ObservableCollection<QWorkspaceViewModel>, INotifyPropertyChanged
    {
        QServer server;
        public new event PropertyChangedEventHandler PropertyChanged;
        public string name
        {
            get
            {
                return server.name;
            }
            set
            {
                server.name = value;
            }
        }

        public string host
        {
            get
            {
                return server.host;
            }
        }

        public string version
        {
            get
            {
                if (server.version == null)
                {
                    return "";
                }
                else
                {
                    return $"v{server.version}";

                }
            }
        }

        public string GroupName
        {
            get
            {
                return $"{name} ({host})";
            }
        }

        public QServerViewModel(QServer server)
        {
            this.server = server;
            name = this.server.name;
            foreach (var workspace in this.server.workspaces)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.Add(new QWorkspaceViewModel(workspace));
                });
            }
            this.server.ServerUpdated += OnServerUpdated;
        }

        private void OnServerUpdated(object source, QServerUpdatedArgs args)
        {
            OnPropertyChanged("GroupName");
            OnPropertyChanged("version");

            foreach (var workspace in this.server.workspaces)
            {
                QWorkspaceViewModel workspaceViewModel = new QWorkspaceViewModel(workspace);
                if (!Contains(workspaceViewModel))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Add(workspaceViewModel);
                    });
                }
            }
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }   
}
