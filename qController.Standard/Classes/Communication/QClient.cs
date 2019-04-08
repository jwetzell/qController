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
            if (msg.Address.Contains("cue_id"))
            {
                var updateID = msg.Address.Split('/').Last();
                Console.WriteLine("Cue needs to be updated: " + updateID);
            }else if (msg.Address.Contains("playbackPosition"))
            {
                if (msg.Arguments.Count > 0)
                {
                    Console.WriteLine("Playback position to be updated: " + msg.Arguments[0]);
                    UpdateSelectedCue();
                }

            }
        }

        public void UpdateSpecificCue(string cue_id)
        {
            Console.WriteLine(cue_id);
        }

        public void UpdateSelectedCue()
        {
            string valuesForKeys = "[\"displayName\",\"number\",\"type\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"uniqueID\",\"flagged\",\"listName\",\"colorName\",\"name\",\"armed\"]";
            sendAndReceiveStringArgs("/cue/selected/valuesForKeys", valuesForKeys);
        }

        public void Close()
        {
            udpSender.Close();
            qReceiver.Close();
        }
    }
}
