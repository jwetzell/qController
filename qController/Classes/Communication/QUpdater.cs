//Class used for updating local "copy" of workspace info, cue-lists, cues, etc.
using Serilog;

namespace qController.Communication
{
    public class QUpdater
    {
        private QController qController;  
        
        public QUpdater(QController controller)
        {
            qController = controller;
            qController.qClient.qParser.CueInfoUpdated += OnCueUpdateReceived;
            qController.qClient.qParser.WorkspaceUpdated += OnWorkspaceUpdated;
            qController.qClient.qParser.PlaybackPositionUpdated += OnPlaybackPositionUpdated;
            qController.qClient.qParser.WorkspaceLoadError += OnWorkspaceLoadError;
            qController.qClient.qParser.ConnectionStatusChanged += OnConnectionStatusChanged;

        }

        private void OnConnectionStatusChanged(object source, ConnectEventArgs args)
        {
            
            Log.Debug("QUPDATER - Connection Status Changed: " + args.Status);

            if (args.Status.Contains("ok"))
            {
                //new sort of connect format for QLab 5 ok:view|control|edit is connection good
                if(args.Status.Contains(":") && !args.Status.Contains("view"))
                {
                    return;
                }
                if (qController.qWorkspace.version.StartsWith("5"))
                {
                    qController.qClient.sendTCP("/updates", 1);
                }
                else
                {
                    qController.qClient.sendTCP("/workspace/" + qController.qWorkspace.workspace_id + "/updates", 1);
                }
                qController.qClient.sendTCP("/workspace/" + qController.qWorkspace.workspace_id + "/cueLists");
            }
        }

        private void OnWorkspaceLoadError(object source, WorkspaceEventArgs args)
        {
            Log.Debug("QUPDATER - Loading cuelists has failed for some reason retrying");
        }

        public void OnCueUpdateReceived(object source, CueEventArgs args)
        {
            qController.qWorkspace.UpdateCue(args.Cue);

            if (args.Cue.type == "Group")
            {
                //Log.Debug("QUpdater/Updated cue was group cue, sending children request");
                qController.qClient.sendTCP("/workspace/"+qController.qWorkspace.workspace_id+"/cue_id/" + args.Cue.uniqueID + "/children");
            }
        }

        public void OnWorkspaceUpdated(object source, WorkspaceEventArgs args)
        {
            qController.qWorkspace = args.UpdatedWorkspace;
            qController.playbackPosition = null;
            qController.qWorkspace.CheckPopulated();
            if (qController.qWorkspace.IsPopulated)
            {
                Log.Debug("QUPDATER - Workspace group cues are already populated");
                App.showToast("Workspace cues have been loaded....");
                //get selected cue
                qController.qClient.UpdateSelectedCue(qController.qWorkspace.workspace_id);
                return;
            }
        }

        public void OnPlaybackPositionUpdated(object source, PlaybackPositionArgs args)
        {
            qController.playbackPosition = args.PlaybackPosition;
            Log.Debug("QUPDATER - Update Specific Cue Called because of Playback Position Updated");
            qController.qClient.UpdateSpecificCue(qController.qWorkspace.workspace_id,args.PlaybackPosition);
        }
    }
}
