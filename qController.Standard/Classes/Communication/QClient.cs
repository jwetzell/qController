﻿//Class used to facilitate communication with a QInstance
//Listens for incoming messages via qReceiver (UDP) and tcpSender (TCP)
//WEIRD MESSAGE PARSING RULES NEED FIXED
using System;
using SharpOSC;

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
            //udpSender = new UDPSender(Address, Port);

            tcpSender.MessageReceived += OnMessageReceived;
            qReceiver.UpdateMessageReceived += OnMessageReceived;

        }

        //one method for messages received whether from TCP or UDP (SAME MSG FORMAT)
        private void OnMessageReceived(object source, MessageEventArgs args)
        {
            Console.WriteLine("QCLIENT - Message Received: " + args.Message.Address);
            //try to find a better filtering process
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
                Console.WriteLine("QCLIENT - Send Exception: " + ex.Message);
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
                Console.WriteLine("QCLIENT - Send w/Args Exception: " + ex.Message);
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
                Console.WriteLine("QCLIENT - Send and Receive Exception: " + ex.Message);
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
                Console.WriteLine("QCLIENT - Send and Receive w/Args Exception: " + ex.Message);
            }
        }


        public void ProcessUpdate(OscMessage msg)
        {
            Console.WriteLine("QCLIENT - Process Update: " + msg.Address);
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
            Console.WriteLine("QCLIENT - Workspace needs to be updated: " + ws_id);
        }

        public void Close()
        {
            udpSender.Close();
            qReceiver.Close();
        }
    }
}
