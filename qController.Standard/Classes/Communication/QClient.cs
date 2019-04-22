﻿using System;
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


        public delegate void WorkspaceDisconnectHandler(object source, EventArgs args);
        public event WorkspaceDisconnectHandler WorkspaceDisconnect;

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
            tcpSender.SendAndReceive(new OscMessage(address));
        }

        public void sendAndReceiveStringArgs(string address, params object[] args)
        {
            tcpSender.SendAndReceive(new OscMessage(address,args));
        }


        public void ProcessUpdate(OscMessage msg)
        {
            if (msg.Address.Contains("disconnect"))
            {
                OnWorkspaceDisconnect();
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

            string valuesForKeys = "[\"number\",\"uniqueID\",\"flagged\",\"listName\",\"type\",\"colorName\",\"name\",\"armed\",\"displayName\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"notes\",\"levels\"]";

            string address = "/cue_id/" + cue_id + "/valuesForKeys";
            sendAndReceiveStringArgs(address, valuesForKeys);
        }

        public void UpdateSelectedCue()
        {
            string valuesForKeys = "[\"number\",\"uniqueID\",\"flagged\",\"listName\",\"type\",\"colorName\",\"name\",\"armed\",\"displayName\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"notes\",\"levels\"]"; 
            sendAndReceiveStringArgs("/cue/selected/valuesForKeys", valuesForKeys);
        }

        public void UpdateWorkspace(string ws_id)
        {
            Console.WriteLine("Workspace needs to be updated: " + ws_id);
        }

        protected virtual void OnWorkspaceDisconnect()
        {
            if (WorkspaceDisconnect != null)
                WorkspaceDisconnect(this, new EventArgs());
        }

        public void Close()
        {
            udpSender.Close();
            qReceiver.Close();
        }
    }
}
