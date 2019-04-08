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
        QWorkSpace qWorkspace;
		public QController(string address, int port)
        {
            qClient = new QClient(address, port);

            qUpdater = new QUpdater(this);
            qUpdater.Start();

            qClient.qParser.SelectedCueUpdated += this.OnSelectedCueUpdated;
            qClient.qParser.WorkspaceUpdated += this.OnWorkspaceUpdated;


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
            selectedCue = args.SelectedCue;
            Console.WriteLine("Selected cue updated: " + selectedCue.uniqueID);
            foreach (var cueList in qWorkspace.data)
            {
                foreach( var cue in cueList.cues)
                {
                    if(cue.uniqueID == selectedCue.uniqueID)
                    {
                        Console.WriteLine("Cue found in local workspace");
                    }
                }
            }
        }

        public void OnWorkspaceUpdated(object source, WorkspaceEventArgs args)
        {

            qWorkspace = args.UpdatedWorkspace;

        }
        public void Kill(){
            qUpdater.Kill();
            qClient.Close();
        }
    }
}
