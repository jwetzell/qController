using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace qController
{
    public class QController
    {

        QUpdater qUpdater;
        public QClient qClient;
        public QWorkSpace qWorkspace;
        public string playbackPosition;
        public Stopwatch sw;
		public QController(string address, int port)
        {
            qClient = new QClient(address, port);
            qUpdater = new QUpdater(this);
            qUpdater.Start();

            qClient.sendArgsUDP("/updates", 1);
            qClient.sendAndReceiveString("/cueLists");
        }

        public void Connect()
        {
            qClient.sendStringUDP("/connect");
        }

        public void Disconnect()
        {
            qClient.sendStringUDP("/disconnect");
        }

        public void ConnectWithPass(string pass)
        {
            qClient.sendArgsUDP("/connect", pass);
        }

        public void Kill(){
            qUpdater.Kill();
            qClient.Close();
        }
    }
}
