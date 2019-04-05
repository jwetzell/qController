using System;
using SharpOSC;
using Newtonsoft.Json;
namespace qController
{
    public class QClient
    {
        TCPSender tcpSender;
        UDPSender udpSender;

        public QClient(string address, int port)
        {
            tcpSender = new TCPSender(address, port);
            udpSender = new UDPSender(address, port);
        }

        public void sendString(string address)
        {
            tcpSender.Send(new OscMessage(address));
        }

        public OscMessage sendAndReceiveString(string address)
        {
            return tcpSender.SendAndReceive(new OscMessage(address));
        }

        public OscMessage sendAndReceiveStringArgs(string address, params object[] args)
        {
            return tcpSender.SendAndReceive(new OscMessage(address, args));
        }

        public void sendStringUDP(string address)
        {
            udpSender.Send(new OscMessage(address));
        }
        public void sendArgs(string address, object args)
        {
            tcpSender.Send(new OscMessage(address, args));
        }
        public void sendArgsUDP(string address, object args)
        {
            udpSender.Send(new OscMessage(address, args));
        }
    }
}
