//Class used to facilitate communication with a QInstance
//Listens for incoming messages via qReceiver (UDP) and tcpClient (TCP)
//TODO: WEIRD MESSAGE PARSING RULES NEED FIXED
using System;
using SharpOSC;
using Serilog;

namespace qController.Communication
{
    public class QClient
    {
        private TCPClient tcpClient;
        public QParser qParser;
        string Address;
        int Port;
        public bool connected;

        public QClient(string address, int port)
        {
            Address = address;
            Port = port;
            qParser = new QParser();
            tcpClient = new TCPClient(Address, Port);
            connected = tcpClient.Connect();
            tcpClient.MessageReceived += OnMessageReceived;

        }

        //one method for messages received whether from TCP or UDP (SAME MSG FORMAT)
        private void OnMessageReceived(object source, MessageEventArgs args)
        {
            Log.Debug("QCLIENT - Message Received: " + args.Message.Address);


            //TODO: find a better filtering process
            if (!args.Message.Address.Contains("update")) 
            {
                qParser.ParseMessage(args.Message);
            }
            else if (args.Message.Address.Contains("playbackPosition") || args.Message.Address.Contains("cueList") || args.Message.Address.Contains("dashboard"))
            {
                qParser.ParseMessage(args.Message);
            }
            else if (args.Message.Address.Contains("cue_id")) 
            {
                var parts = args.Message.Address.Split('/');
                UpdateSpecificCue(parts[3], parts[5]);
            } 
            else if (args.Message.Address.EndsWith("update"))
            {
                ProcessUpdate(args.Message);
            } 
            else
            {
                Log.Warning("QCLIENT - Unhandled address: " + args.Message.Address);
            }
        }

        public void sendTCP(string address)
        {
            //Log.Debug($"QCLIENT - TCP Sent with address: {address}");
            try
            {
                tcpClient.Send(new OscMessage(address));
            }
            catch (Exception ex)
            {
                Log.Debug("QCLIENT - Send and Receive Exception: " + ex.Message);
            }
        }

        public void sendTCP(string address, params object[] args)
        {
            //Log.Debug($"QCLIENT - TCP Sent with address: {address}");
            try
            {
                tcpClient.Send(new OscMessage(address, args));
            }
            catch (Exception ex)
            {
                Log.Debug("QCLIENT - Send and Receive w/Args Exception: " + ex.Message);
            }
        }


        public void ProcessUpdate(OscMessage msg)
        {
            Log.Debug("QCLIENT - Process Update: " + msg.Address);
            if (msg.Address.Contains("workspace"))
            { 
                UpdateWorkspace("not yet implemented");
            }
        }

        public void UpdateSpecificCue(string workspace_id,string cue_id)
        {
            string valuesForKeys = "[\"number\",\"uniqueID\",\"flagged\",\"listName\",\"type\",\"colorName\",\"name\",\"armed\",\"displayName\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"notes\",\"levels\"]";
            string address = "/workspace/" + workspace_id + "/cue_id/" + cue_id + "/valuesForKeys";
            sendTCP(address, valuesForKeys);
        }

        public void UpdateSelectedCue(string workspace_id)
        {
            string valuesForKeys = "[\"number\",\"uniqueID\",\"flagged\",\"listName\",\"type\",\"colorName\",\"name\",\"armed\",\"displayName\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"notes\",\"levels\"]";
            string address = "/workspace/" + workspace_id + "/cue/selected/valuesForKeys"; 
            sendTCP(address, valuesForKeys);
        }

        public void UpdateWorkspace(string ws_id)
        {
            Log.Debug("QCLIENT - Workspace needs to be updated: " + ws_id);
        }

        public void Close()
        {
            tcpClient.Close();
        }
    }
}
