using qController.Pages;
using QSharp;
using System.Collections.ObjectModel;

namespace qController.ViewModels
{
    public class QServerViewModel : ObservableCollection<QWorkspaceViewModel>
    {
        QServer server;
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

        public QServerViewModel(QServer server)
        {
            this.server = server;
            name = this.server.name;
            foreach (var workspace in this.server.workspaces)
            {
                this.Add(new QWorkspaceViewModel(workspace));
            }
            this.server.ServerUpdated += OnServerUpdated;
        }

        private void OnServerUpdated(object source, QServerUpdatedArgs args)
        {
            foreach (var workspace in this.server.workspaces)
            {
                QWorkspaceViewModel workspaceViewModel = new QWorkspaceViewModel(workspace);
                if (!Contains(workspaceViewModel))
                    Add(workspaceViewModel);
            }
        }
    }   
}
