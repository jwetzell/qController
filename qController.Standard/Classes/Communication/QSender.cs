using System;
using System.Net;
using SharpOSC;

namespace qController
{
    public class QSender
    {
        TCPSender tcpSender;
        UDPSender udpSender;

        public QSender(string address, int port)
        {
            tcpSender = new TCPSender(address, port);
            udpSender = new UDPSender(address, port);
        }

        public void sendString(string address) {
            tcpSender.Send(new OscMessage(address));
        }

        public void sendStringUDP(string address)
        {
            udpSender.Send(new OscMessage(address));
        }
        public void sendArgs(string address, object args){
            tcpSender.Send(new OscMessage(address, args));
        }
        public void sendArgsUDP(string address, object args)
        {
            udpSender.Send(new OscMessage(address, args));
        }

    }
}
