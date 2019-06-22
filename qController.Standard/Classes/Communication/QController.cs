using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace qController
{
    public class QController
    {

        public QUpdater qUpdater;
        public QClient qClient;
        public QWorkSpace qWorkspace;
        public string playbackPosition;
        public Stopwatch sw;
		public QController(string address, int port)
        {
            qClient = new QClient(address, port);
            qUpdater = new QUpdater(this);
            qUpdater.Start();
            //Connect();
            //Connect();
            //KickOff();
           
        }

        public void Connect(string workspace_id)
        {
            Console.WriteLine("QCONTROLLER CONNECT CALLED: " + workspace_id);
            qWorkspace = new QWorkSpace();
            qWorkspace.workspace_id = workspace_id;
            //qClient.sendStringUDP("/connect");
            qClient.sendArgsUDP("/workspace/"+workspace_id+"/updates", 1);
            qClient.sendAndReceiveString("/workspace/"+workspace_id+"/cueLists");
        }

        public void KickOff()
        {
            qClient.sendArgsUDP("/workspaces");
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
