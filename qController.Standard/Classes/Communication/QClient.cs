using System;
using SharpOSC;
using Newtonsoft.Json;
namespace qController
{
    public class QClient
    {
        TCPSender tcpSender;
        public QParser qParser;

        public QClient(string address, int port)
        {
            qParser = new QParser();
            tcpSender = new TCPSender(address, port);
            tcpSender.MessageReceived += OnMessageReceived;
            
        }

        private void OnMessageReceived(object source, MessageEventArgs args)
        {
            qParser.ParseMessage(args.Message);
        }

        public void sendString(string address)
        {
            tcpSender.Send(new OscMessage(address));
        }

        public void sendArgs(string address, object args)
        {
            tcpSender.Send(new OscMessage(address, args));
        }

        public OscMessage sendAndReceiveString(string address)
        {
            return tcpSender.SendAndReceive(new OscMessage(address));
        }

        public OscMessage sendAndReceiveStringArgs(string address, params object[] args)
        {
            return tcpSender.SendAndReceive(new OscMessage(address, args));
        }



    }
}
