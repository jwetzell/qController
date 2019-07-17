//Class used for overall Workspace control (ONLY REPRESENTS ONE WORKSPACE)
//Contains all necessary items to facilitate communication (sending, receiving) to a QLab workspace 
//Also contains the local QWorkspace object which stores all the information that is used in displaying
//STILL NEED TO WORK ON PASSWORD PROTECTED WORKSPACES
using System;
using System.Diagnostics;

namespace qController
{
    public class QController
    {

        public QUpdater qUpdater;
        public QClient qClient;
        public QWorkspace qWorkspace;
        public string playbackPosition;
        public Stopwatch sw;
		public QController(string address, int port)
        {
            qClient = new QClient(address, port);
            qUpdater = new QUpdater(this);

        }

        public void Connect(string workspace_id)
        {
            Console.WriteLine("QCONTROLLER CONNECT CALLED: " + workspace_id);
            qWorkspace = new QWorkspace();
            qWorkspace.workspace_id = workspace_id;
            qUpdater.Start();
            qClient.sendArgsUDP("/workspace/"+workspace_id+"/connect");
            qClient.sendArgsUDP("/workspace/"+workspace_id+"/updates", 1);
            qClient.sendAndReceiveString("/workspace/"+workspace_id+"/cueLists");
        }

        public void KickOff()
        {
            qClient.sendArgsUDP("/workspaces");
        }

        public void Disconnect()
        {
            qClient.sendStringUDP("/workspace/"+qWorkspace.workspace_id+"/disconnect");
        }

        public void ConnectWithPass(string pass)
        {
            qClient.sendArgsUDP("/workspace/" + qWorkspace.workspace_id + "/connect", pass);
        }

        public void Kill(){
            if (qUpdater != null)
                qUpdater.Kill();
            if (qClient != null)
                qClient.Close();
        }
    }
}
