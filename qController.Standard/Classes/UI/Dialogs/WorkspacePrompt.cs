using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using qController.QItems;
using Serilog;
using QControlKit;

namespace qController.UI.Dialogs
{
    public class WorkspacePromptArgs : EventArgs
    {
        public QOldWorkspace SelectedWorkspace
        {
            get;
            set;
        }
    }

    public class WorkspacePrompt
    {
        public delegate void WorkspaceSelectedHandler(object source, WorkspacePromptArgs args);
        public event WorkspaceSelectedHandler WorkspaceSelected;

        public WorkspacePrompt()
        {

        }

        public ActionSheetConfig getActionSheetConfigForWorkspaces(List<QWorkspaceInfo> workspaces)
        {
            ActionSheetConfig actionSheetConfig = new ActionSheetConfig();
            actionSheetConfig.SetTitle("Select Workspace");
            for (int i = 0; i < workspaces.Count; i++)
            {
                QWorkspaceInfo workspace = workspaces[i];
                actionSheetConfig.Add(workspace.displayName, new Action(() => {
                    Log.Debug("WorkspacePrompt - Workspace Selected " + workspace.displayName);

                    
                    if (!workspace.hasPasscode)
                    {
                        OnWorkspaceSelected(new QOldWorkspace(workspace.uniqueID));
                    }
                    else
                    {
                        promptWorkspacePasscode(workspace.uniqueID);
                    }
                }));
            }
            return actionSheetConfig;
        }

        

        public void promptWorkspacePasscode(string workspace_id)
        {
            UserDialogs.Instance.Prompt(new PromptConfig
            {
                InputType = InputType.Number,
                MaxLength = 4,
                Title = "Enter Workspace Passcode",
                OkText = "Connect",
                IsCancellable = true,
                OnAction = (resp) =>
                {
                    if (resp.Ok)
                    {
                        OnWorkspaceSelected(new QOldWorkspace(workspace_id,resp.Value));
                    }
                    else
                    {
                        OnWorkspaceSelected(null);
                    }
                }

            });
        }

        protected virtual void OnWorkspaceSelected(QOldWorkspace workspace)
        {
            if (WorkspaceSelected != null)
                WorkspaceSelected(this, new WorkspacePromptArgs() { SelectedWorkspace = workspace  });
        }



    }
}
