using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace qController
{
    public class QController
    {

        QUpdater qUpdater;
        QCue selectedCue;
        public QClient qClient;
        public QWorkSpace qWorkspace;
        public string playbackPosition;
		public QController(string address, int port)
        {
            qClient = new QClient(address, port);

            qUpdater = new QUpdater(this);
            qUpdater.Start();

            qClient.qParser.SelectedCueUpdated += this.OnSelectedCueUpdated;
            qClient.qParser.WorkspaceUpdated += this.OnWorkspaceUpdated;
            qClient.qParser.PlaybackPositionUpdated += this.OnPlaybackPositionUpdated;
            qClient.qParser.CueInfoUpdated += this.OnCueUpdateReceived;
            qClient.sendArgsUDP("/updates", 1);
            qClient.sendAndReceiveString("/cueLists");
        }

        public void sendCommand(string cmd){
            qClient.sendString(cmd);
        }

        public void updateCueValue(QCue cue, string property, object newValue){
           // qSender.sendArgs("/cue_id/"+cue.uniqueID+"/"+property, newValue);
        }

        public void OnSelectedCueUpdated(object source, CueEventArgs args)
        {
            selectedCue = args.Cue;
            Console.WriteLine("Selected cue updated: " + selectedCue.uniqueID);
        }

        public void OnWorkspaceUpdated(object source, WorkspaceEventArgs args)
        {
            qWorkspace = args.UpdatedWorkspace;
            Console.WriteLine("Workspace updated in QController: " + qWorkspace.workspace_id);
            qWorkspace.PrintStats();
            playbackPosition = null;
            qClient.UpdateSelectedCue();
        }

        public void OnPlaybackPositionUpdated(object source, PlaybackPositionArgs args)
        {
            playbackPosition = args.PlaybackPosition;
            Console.WriteLine("Playback Position updated in QController: " + playbackPosition);
        }

        public void OnCueUpdateReceived(object source, CueEventArgs args)
        {
            qWorkspace.UpdateCue(args.Cue);
            Console.WriteLine("Cue updated in QController: " + args.Cue.uniqueID );
            if (playbackPosition == null)
            {
                playbackPosition = args.Cue.uniqueID;
            }
        }
        public void Kill(){
            qUpdater.Kill();
            qClient.Close();
        }
    }
}
