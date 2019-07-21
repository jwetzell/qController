//Class used for updating local "copy" of workspace info, cue-lists, cues, etc.
using System;
using System.Threading;
using Xamarin.Forms;

namespace qController
{
    public class QUpdater
    {

        private bool active = true;
        private QController qController;
        private Thread updateThread; 
        public QUpdater(QController controller)
        {
            qController = controller;
            updateThread = new Thread(new ThreadStart(UpdateLoop));
            qController.qClient.qParser.CueInfoUpdated += OnCueUpdateReceived;
            qController.qClient.qParser.WorkspaceUpdated += OnWorkspaceUpdated;
            qController.qClient.qParser.PlaybackPositionUpdated += OnPlaybackPositionUpdated;
            qController.qClient.qParser.WorkspaceLoadError += OnWorkspaceLoadError;

        }

        private void OnWorkspaceLoadError(object source, WorkspaceEventArgs args)
        {
            Console.WriteLine("QUpdater: Loading cuelists has failed for some reason retrying");
            qController.qClient.sendArgsUDP("/workspace/" + args.UpdatedWorkspace.workspace_id + "/connect");
            qController.qClient.sendArgsUDP("/workspace/" + args.UpdatedWorkspace.workspace_id + "/updates", 1);
            qController.qClient.sendAndReceiveString("/workspace/" + args.UpdatedWorkspace.workspace_id + "/cueLists");

        }

        public void Start(){
            updateThread.Start();
        }

        //Attempt at a "keep alive" message
        public void UpdateLoop()
        {
            active = true;
            Device.StartTimer(TimeSpan.FromSeconds(5.0), () => {
                SendThump();
                return active;
            });
        }

        public void OnCueUpdateReceived(object source, CueEventArgs args)
        {
            qController.qWorkspace.UpdateCue(args.Cue);

            if (args.Cue.type == "Group")
            {
                //Console.WriteLine("QUpdater/Updated cue was group cue, sending children request");
                qController.qClient.sendStringUDP("/workspace/"+qController.qWorkspace.workspace_id+"/cue_id/" + args.Cue.uniqueID + "/children");
            }
        }

        public void OnWorkspaceUpdated(object source, WorkspaceEventArgs args)
        {
            qController.qWorkspace = args.UpdatedWorkspace;
            qController.playbackPosition = null;
            qController.qWorkspace.CheckPopulated();
            if (qController.qWorkspace.IsPopulated)
            {
                Console.WriteLine("Workspace group cues are already populated");
                App.showToast("Workspace cues have been loaded....");
                //get selected cue
                qController.qClient.UpdateSelectedCue(qController.qWorkspace.workspace_id);
                return;
            }
        }

        public void OnPlaybackPositionUpdated(object source, PlaybackPositionArgs args)
        {
            qController.playbackPosition = args.PlaybackPosition;
            Console.WriteLine("ControlPage: Update Specific Cue Called because of Playback Position Updated");
            qController.qClient.UpdateSpecificCue(qController.qWorkspace.workspace_id,args.PlaybackPosition);
        }

        public void SendThump()
        {
            qController.qClient.sendStringUDP("/workspace/" + qController.qWorkspace.workspace_id + "/thump");
        }

        public void Kill(){
            active = false;
            updateThread.Abort();
        }
    }
}
