using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace qController
{
    public class QController
    {

		public QSender qSender;
		QReceiver qReceiver;
        QUpdater qUpdater;
        public QParser qParser;
        QCue selectedCue;

		public QController(string address, int port)
        {
            
			qSender = new QSender(address, port);
            qReceiver = new QReceiver(port+1, this);
            qParser = new QParser();
            qUpdater = new QUpdater(this);
            qUpdater.Start();

            qParser.SelectedCueUpdated += this.OnSelectedCueUpdated;
            qReceiver.PacketReceived += qParser.OnPacketReceived;

        }

        public void sendCommand(string cmd){
            qSender.sendString(cmd);
        }

        public void updateCueValue(QCue cue, string property, object newValue){
           // qSender.sendArgs("/cue_id/"+cue.uniqueID+"/"+property, newValue);
        }

        public void OnSelectedCueUpdated(object source, CueEventArgs args)
        {
            selectedCue = args.SelectedCue;
            //Console.WriteLine("Selected Cue Updated: Name=" + selectedCue.listName);
        }

        public void Kill(){
            qReceiver.Close();
            qUpdater.Kill();
        }
    }
}
