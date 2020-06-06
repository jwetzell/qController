//Class used to facilitate communication with a QInstance
//Listens for incoming messages via qReceiver (UDP) and tcpSender (TCP)
//TODO: WEIRD MESSAGE PARSING RULES NEED FIXED
using System;
using SharpOSC;
using Serilog;

namespace qController
{
    public class QClient
    {
        TCPSender tcpSender;
        UDPSender udpSender;
        public QParser qParser;
        public QReceiver qReceiver;
        string Address;
        int Port;

        public QClient(string address, int port)
        {
            Address = address;
            Port = port;
            qParser = new QParser();
            qReceiver = new QReceiver(Port + 1);
            tcpSender = new TCPSender(Address, Port);
            tcpSender.Connect();
            udpSender = new UDPSender(Address, Port);

            tcpSender.MessageReceived += OnMessageReceived;
            qReceiver.UpdateMessageReceived += OnMessageReceived;

        }

        //one method for messages received whether from TCP or UDP (SAME MSG FORMAT)
        private void OnMessageReceived(object source, MessageEventArgs args)
        {
            Log.Debug("QCLIENT - Message Received: " + args.Message.Address);


            //TODO: find a better filtering process
            if (!args.Message.Address.Contains("update"))
                qParser.ParseMessage(args.Message);
            else if (args.Message.Address.Contains("playbackPosition") || args.Message.Address.Contains("cueList") || args.Message.Address.Contains("dashboard"))
                qParser.ParseMessage(args.Message);
            else if (args.Message.Address.Contains("cue_id"))
            {
                var parts = args.Message.Address.Split('/');
                UpdateSpecificCue(parts[3], parts[5]);
            }
            else
                ProcessUpdate(args.Message);
        }

        public void sendString(string address)
        {
            try
            {
                tcpSender.Send(new OscMessage(address));
            }
            catch (Exception ex)
            {
                Log.Debug("QCLIENT - Send Exception: " + ex.Message);
            }
        }

        public void sendUDP(string address)
        {
            Log.Debug($"QCLIENT - UDP Sent with address: {address}");
            udpSender = new UDPSender(Address,Port);
            udpSender.Send(new OscMessage(address));
            udpSender.Close();
        }
        public void sendUDP(string address, params object[] args)
        {
            Log.Debug($"QCLIENT - UDP Sent with address: {address}");
            udpSender = new UDPSender(Address, Port);
            udpSender.Send(new OscMessage(address, args));
            udpSender.Close();
        }
        public void sendTCP(string address)
        {
            Log.Debug($"QCLIENT - TCP Sent with address: {address}");
            try
            {
                tcpSender.Send(new OscMessage(address));
            }
            catch (Exception ex)
            {
                Log.Debug("QCLIENT - Send and Receive Exception: " + ex.Message);
            }
        }

        public void sendTCP(string address, params object[] args)
        {
            Log.Debug($"QCLIENT - TCP Sent with address: {address}");
            try
            {
                tcpSender.Send(new OscMessage(address, args));
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
            udpSender.Close();
            qReceiver.Close();
        }
    }
}
