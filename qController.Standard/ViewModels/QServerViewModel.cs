using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

using QControlKit;
using QControlKit.Events;

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
            this.server.WorkspaceAdded += Server_WorkspaceAdded;
            this.server.WorkspaceRemoved += Server_WorkspaceRemoved;
            this.server.ServerUpdated += OnServerUpdated;
        }

        private void Server_WorkspaceRemoved(object source, QServerWorkspaceChangedArgs args)
        {
            QWorkspaceViewModel workspaceToRemove = null;

            foreach(var workspaceViewModel in this)
            {
                if(workspaceViewModel.workspace.uniqueID == args.workspace.uniqueID)
                {
                    workspaceToRemove = workspaceViewModel;
                    break;
                }
            }
            if(workspaceToRemove != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Remove(workspaceToRemove);
                });
            }
 
        }

        private void Server_WorkspaceAdded(object source, QServerWorkspaceChangedArgs args)
        {
            QWorkspaceViewModel workspaceViewModel = new QWorkspaceViewModel(args.workspace);
            Device.BeginInvokeOnMainThread(() =>
            {
                Add(workspaceViewModel);
            });
        }



        private void OnServerUpdated(object source, QServerUpdatedArgs args)
        {
            OnPropertyChanged("GroupName");
            OnPropertyChanged("version");
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }   
}
