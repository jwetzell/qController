//Class used to facilitate communication with a QInstance
//Listens for incoming messages via qReceiver (UDP) and tcpSender (TCP)
//WEIRD MESSAGE PARSING RULES NEED FIXED

using System;
using System.Linq;
using System.Threading;
using SharpOSC;
using Newtonsoft.Json;
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
            udpSender = new UDPSender(Address, Port);

            tcpSender.MessageReceived += OnMessageReceived;
            qReceiver.UpdateMessageReceived += OnMessageReceived;

        }

        //one method for messages received whether from TCP or UDP (SAME MSG FORMAT)
        private void OnMessageReceived(object source, MessageEventArgs args)
        {
            Console.WriteLine("QCLIENT MESSAGE RECEIVED: "+args.Message.Address);
            //try to find a better filtering process
            if (!args.Message.Address.Contains("update"))
                qParser.ParseMessage(args.Message);
            else if (args.Message.Address.Contains("playbackPosition") || args.Message.Address.Contains("cue_id") || args.Message.Address.Contains("cueList"))
                qParser.ParseMessage(args.Message);
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
                Console.WriteLine("SEND EXCEPTION: " + ex.Message);
            }
        }

        public void sendArgs(string address, object args)
        {
            try
            {
                tcpSender.Send(new OscMessage(address, args));
            }
            catch (Exception ex)
            {
                Console.WriteLine("SEND ARGS EXCEPTION: " + ex.Message);
            }
        }
        public void sendStringUDP(string address)
        {
            udpSender = new UDPSender(Address,Port);
            udpSender.Send(new OscMessage(address));
            udpSender.Close();
        }
        public void sendArgsUDP(string address, params object[] args)
        {
            udpSender = new UDPSender(Address, Port);
            udpSender.Send(new OscMessage(address, args));
            udpSender.Close();
        }
        public void sendAndReceiveString(string address)
        {
            try
            {
                tcpSender.SendAndReceive(new OscMessage(address));
            }
            catch (Exception ex)
            {
                Console.WriteLine("SEND AND RECIEVE EXCEPTION: " + ex.Message);
            }
        }

        public void sendAndReceiveStringArgs(string address, params object[] args)
        {
            try
            {
                tcpSender.SendAndReceive(new OscMessage(address, args));
            }
            catch (Exception ex)
            {
                Console.WriteLine("SEND AND RECEIVE ARGS EXCEPTION: " + ex.Message);
            }
        }


        public void ProcessUpdate(OscMessage msg)
        {
            Console.WriteLine("PROCESS UPDATE : " + msg.Address);
            if (msg.Address.Contains("workspace"))
            { 
                UpdateWorkspace("not yet implemented");
            }
        }

        public void UpdateSpecificCue(string workspace_id,string cue_id)
        {
            string valuesForKeys = "[\"number\",\"uniqueID\",\"flagged\",\"listName\",\"type\",\"colorName\",\"name\",\"armed\",\"displayName\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"notes\",\"levels\"]";
            string address = "/workspace/" + workspace_id + "/cue_id/" + cue_id + "/valuesForKeys";
            sendAndReceiveStringArgs(address, valuesForKeys);
        }

        public void UpdateSelectedCue(string workspace_id)
        {
            string valuesForKeys = "[\"number\",\"uniqueID\",\"flagged\",\"listName\",\"type\",\"colorName\",\"name\",\"armed\",\"displayName\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"notes\",\"levels\"]";
            string address = "/workspace/" + workspace_id + "/cue/selected/valuesForKeys"; 
            sendAndReceiveStringArgs(address, valuesForKeys);
        }

        public void UpdateWorkspace(string ws_id)
        {
            Console.WriteLine("Workspace needs to be updated: " + ws_id);
        }

        public void Close()
        {
            udpSender.Close();
            qReceiver.Close();
        }
    }
}
