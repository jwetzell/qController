using System;
using System.Linq;
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

        public QClient(string address, int port)
        {
            qParser = new QParser();
            qReceiver = new QReceiver(port + 1);
            tcpSender = new TCPSender(address, port);
            udpSender = new UDPSender(address, port);

            tcpSender.MessageReceived += OnMessageReceived;
            qReceiver.UpdateMessageReceived += OnMessageReceived;

            
        }

        private void OnMessageReceived(object source, MessageEventArgs args)
        {
            if (!args.Message.Address.Contains("update"))
                qParser.ParseMessage(args.Message);
            else
                ProcessUpdate(args.Message);
        }

        public void sendString(string address)
        {
            tcpSender.Send(new OscMessage(address));
        }

        public void sendArgs(string address, object args)
        {
            tcpSender.Send(new OscMessage(address, args));
        }
        public void sendStringUDP(string address)
        {
            udpSender.Send(new OscMessage(address));
        }
        public void sendArgsUDP(string address, params object[] args)
        {
            udpSender.Send(new OscMessage(address, args));
        }
        public OscMessage sendAndReceiveString(string address)
        {
            return tcpSender.SendAndReceive(new OscMessage(address));
        }

        public OscMessage sendAndReceiveStringArgs(string address, params object[] args)
        {
            return tcpSender.SendAndReceive(new OscMessage(address, args));
        }


        public void ProcessUpdate(OscMessage msg)
        {
            if (msg.Address.Contains("disconnect"))
            {
                Console.WriteLine("Workspace is disonnecting");
            }
            else if (msg.Address.Contains("cue_id"))
            {
                var updateID = msg.Address.Split('/').Last();
                UpdateSpecificCue(updateID);
            }
            else if (msg.Address.Contains("playbackPosition"))
            {
                if (msg.Arguments.Count > 0)
                {
                    qParser.ParseMessage(msg);
                    UpdateSelectedCue();
                }

            }
            else if (msg.Address.Contains("workspace"))
            {
                UpdateWorkspace("not yet implemented");
            }

         
        }

        public void UpdateSpecificCue(string cue_id)
        {
            Console.WriteLine("Cue needs to be updated: " + cue_id);
            string valuesForKeys = "[\"displayName\",\"number\",\"type\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"uniqueID\",\"flagged\",\"listName\",\"colorName\",\"name\",\"armed\"]";
            string address = "/cue/" + cue_id + "/valuesForKeys";
            sendAndReceiveStringArgs(address, valuesForKeys);
        }

        public void UpdateSelectedCue()
        {
            string valuesForKeys = "[\"displayName\",\"number\",\"type\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"uniqueID\",\"flagged\",\"listName\",\"colorName\",\"name\",\"armed\"]";
            sendAndReceiveStringArgs("/cue/selected/valuesForKeys", valuesForKeys);
        }

        public void UpdateWorkspace(string ws_id)
        {
            Console.WriteLine("Workspace needs to be updaeted: " + ws_id);
        }

        public void Close()
        {
            udpSender.Close();
            qReceiver.Close();
        }
    }
}
