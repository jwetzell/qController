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

        public string Name
        {
            get
            {
                return workspace.name;
            }
            set
            {

            }
        }

        public bool HasPasscode
        {
            get
            {
                return workspace.hasPasscode;
            }
            set
            {

            }
        }
    }
}
