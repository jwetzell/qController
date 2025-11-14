//Class used for overall Workspace control (ONLY REPRESENTS ONE WORKSPACE)
//Contains all necessary items to facilitate communication (sending, receiving) to a QLab workspace 
//Also contains the local QWorkspace object which stores all the information that is used in displaying
//TODO: STILL NEED TO WORK ON PASSWORD PROTECTED WORKSPACES
using Serilog;
using qController.QItems;
using System;

namespace qController.Communication
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

        public void Connect(QWorkspaceInfo workspaceInfo)
        {
            Log.Debug($"QCONTROLLER - Connect Called: {workspaceInfo.uniqueID}");
            Connect(new QWorkspace(workspaceInfo));
        }

        public void Connect(QWorkspace workspace)
        {
            Log.Debug($"QCONTROLLER - Connect Called: {workspace.workspace_id}");
            qWorkspace = workspace;
            if (qWorkspace.passcode != null)
            {
                Log.Debug($"QCONTROLLER - Connect with Passcode Called: {qWorkspace.workspace_id}:{qWorkspace.passcode}");
                qClient.sendTCP("/workspace/" + qWorkspace.workspace_id + "/connect", qWorkspace.passcode);
            }
            else
            {
                Log.Debug($"QCONTROLLER - Connect Called: {qWorkspace.workspace_id}");
                qClient.sendTCP("/workspace/" + qWorkspace.workspace_id + "/connect");
            }
        }

        public void KickOff()
        {
            Log.Debug($"QCONTROLLER - Searching for workspaces on: {ipAddress}");
            qClient.sendTCP("/workspaces");
        }

        public void Disconnect()
        {
            Log.Debug($"QCONTROLLER - Disconnecting from: {qWorkspace.workspace_id}");
            qClient.sendTCP("/workspace/"+qWorkspace.workspace_id+"/disconnect");
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
            Log.Debug("QCONTROLLER - Killing");
            if (qClient != null)
                qClient.Close();
        }
    }
}
