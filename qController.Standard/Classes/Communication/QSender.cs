using System;
using System.Net;
using SharpOSC;

namespace qController
{
    public class QSender
    {
        TCPSender tcpSender;

        public QSender(string address, int port)
        {
            tcpSender = new TCPSender(address, port);
            tcpSender.MessageReceived += OnMessageReceived;
        }

        void OnMessageReceived(object source, MessageEventArgs args)
        {
            Console.WriteLine("New Message Received");
        }


        public void sendString(string address) {
            tcpSender.Send(new OscMessage(address));
        }

        public void sendArgs(string address, object args){
            tcpSender.Send(new OscMessage(address, args));
        }

    }
}
