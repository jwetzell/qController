using QControlKit;

namespace qController.ViewModels
{
    public class QWorkspaceViewModel
    {
        public QWorkspace workspace;
        public QWorkspaceViewModel(QWorkspace workspace)
        {
            this.workspace = workspace;
        }

        public string Name => workspace.name;

        public bool HasPasscode => workspace.hasPasscode;
    }
}
