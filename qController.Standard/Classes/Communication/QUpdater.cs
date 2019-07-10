//Class used for updating local "copy" of workspace info, cue-lists, cues, etc.
//CLEAN UP WORKSPACE LOADING STUFF STILL SENDING INDIVIDUAL REQUESTS FOR GROUP CUES?

using System;
using System.Threading;
using Xamarin.Forms;
using SharpOSC;
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

            qController.qClient.qParser.ChildrenUpdated += OnChildrenUpdated;
            qController.qClient.qParser.CueInfoUpdated += OnCueUpdateReceived;
            qController.qClient.qParser.WorkspaceUpdated += OnWorkspaceUpdated;
            qController.qClient.qParser.PlaybackPositionUpdated += OnPlaybackPositionUpdated;

        }

        public void Start(){
            updateThread.Start();
        }

        //Attemp at a "keep alive" message
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
            if (qController.playbackPosition == null)
            {
                qController.playbackPosition = args.Cue.uniqueID;
            }
        }

        private void OnChildrenUpdated(object source, ChildrenEventArgs args)
        {
            qController.qWorkspace.UpdateChildren(args.cue_id, args.children);
            Console.WriteLine("Populated: " + qController.qWorkspace.IsPopulated);
            if (!qController.qWorkspace.IsPopulated)
            {
                Console.WriteLine("IS THIS BEING CALLED");
                QCue cue = qController.qWorkspace.GetEmptyGroup();
                if(cue != null)
                {
                    qController.qClient.sendStringUDP("/cue_id/" + cue.uniqueID + "/children");
                }
            }
            else
            {
                qController.qClient.UpdateSelectedCue(qController.qWorkspace.workspace_id);
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
            //qController.qClient.UpdateSelectedCue();

        }

        public void OnPlaybackPositionUpdated(object source, PlaybackPositionArgs args)
        {
            qController.playbackPosition = args.PlaybackPosition;
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
