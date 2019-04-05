using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace qController
{
    public class QController
    {

        QUpdater qUpdater;
        public QParser qParser;
        QCue selectedCue;
        public QClient qClient;
        QWorkSpace qWorkspace;
		public QController(string address, int port)
        {
            qClient = new QClient(address, port);

            qParser = new QParser();
            qUpdater = new QUpdater(this);
            qUpdater.Start();

            qParser.SelectedCueUpdated += this.OnSelectedCueUpdated;
            qParser.WorkspaceUpdated += this.OnWorkspaceUpdated;


        }

        public void sendCommand(string cmd){
            qClient.sendString(cmd);
        }

        public void sendCommandUDP(string cmd)
        {
            qClient.sendStringUDP(cmd);
        }
        public void updateCueValue(QCue cue, string property, object newValue){
           // qSender.sendArgs("/cue_id/"+cue.uniqueID+"/"+property, newValue);
        }

        public void OnSelectedCueUpdated(object source, CueEventArgs args)
        {
            selectedCue = args.SelectedCue;
            //Console.WriteLine("Selected Cue Updated: Name=" + selectedCue.listName);
        }

        public void OnWorkspaceUpdated(object source, WorkspaceEventArgs args)
        {

            qWorkspace = args.UpdatedWorkspace;

        }
        public void Kill(){
            qUpdater.Kill();
        }
    }
}
