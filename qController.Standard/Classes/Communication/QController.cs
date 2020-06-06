//Class used for overall Workspace control (ONLY REPRESENTS ONE WORKSPACE)
//Contains all necessary items to facilitate communication (sending, receiving) to a QLab workspace 
//Also contains the local QWorkspace object which stores all the information that is used in displaying
//TODO: STILL NEED TO WORK ON PASSWORD PROTECTED WORKSPACES
using System;
using Serilog;

namespace qController
{
    public class QController
    {

        public QUpdater qUpdater;
        public QClient qClient;
        public QWorkspace qWorkspace;
        public string playbackPosition;
        private string ipAddress;
        private int port;

		public QController(string address, int port)
        {
            this.port = port;
            this.ipAddress = address;

            qClient = new QClient(ipAddress, port);
            qUpdater = new QUpdater(this);

        }

        public void Connect(string workspace_id)
        {
            Log.Debug($"QCONTROLLER - Connect Called: {workspace_id}");
            qWorkspace = new QWorkspace(workspace_id);
            qClient.sendUDP("/workspace/"+workspace_id+"/connect");
        }

        public void Connect(string workspace_id, string passcode)
        {
            Log.Debug($"QCONTROLLER - Connect with Passcode Called: {workspace_id}:{passcode}");
            qWorkspace = new QWorkspace(workspace_id);
            qClient.sendUDP("/workspace/" + workspace_id + "/connect", passcode);
            qClient.sendTCP("/workspace/" + workspace_id + "/connect", passcode);

        }

        public void KickOff()
        {
            qClient.sendUDP("/workspaces");
        }

        public void Disconnect()
        {
            qClient.sendUDP("/workspace/"+qWorkspace.workspace_id+"/disconnect");
        }

        public void Resume()
        {
            //qClient = new QClient(ipAddress, port);
            //qUpdater = new QUpdater(this);
            if(qWorkspace != null)
            {
                Log.Debug($"QCONTROLLER - Resuming: {qWorkspace.workspace_id}");
            }
        }

        public void Kill(){
            if (qUpdater != null)
                qUpdater.Kill();
            if (qClient != null)
                qClient.Close();
        }
    }
}
