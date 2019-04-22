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

            if (!qController.qWorkspace.IsPopulated)
            {
                QCue cue = qController.qWorkspace.GetEmptyGroup();
                if(cue != null)
                {
                    qController.qClient.sendStringUDP("/cue_id/" + cue.uniqueID + "/children");
                }
            }
        }

        public void OnWorkspaceUpdated(object source, WorkspaceEventArgs args)
        {
            qController.qWorkspace = args.UpdatedWorkspace;
            qController.playbackPosition = null;
            qController.qClient.UpdateSelectedCue();
        }

        public void OnPlaybackPositionUpdated(object source, PlaybackPositionArgs args)
        {
            qController.playbackPosition = args.PlaybackPosition;
        }

        public void RefreshGroupCues()
        {
            if (qController.qWorkspace.ChildrenPopulated())
                return;
            foreach (var cueList in qController.qWorkspace.data)
            {
                foreach (var cue in cueList.cues)
                {
                    if (cue.type == "Group")
                    {
                        Console.WriteLine("First Group Cue Found");
                        qController.qClient.sendStringUDP("/cue_id/" + cue.uniqueID + "/children");
                        break;
                    }
                }
            }
        }

        public void SendThump()
        {
            qController.qClient.sendStringUDP("/thump");
        }

        public void Kill(){
            active = false;
            updateThread.Abort();
        }
    }
}
