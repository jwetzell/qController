using System;
using System.Net;
using Rug.Osc;

namespace qController
{
    public class QSender
    {
        OscSender qSender;

        public QSender(string address, int port)
        {
			IPAddress ipAddress = IPAddress.Parse(address);

            qSender = new OscSender(ipAddress, port);
        }

        public void sendString(string address) {
            qSender.Connect();
            qSender.Send(new OscMessage(address,new object[]{}));
            qSender.Close();
        }

        public void sendArgs(string msg, object args){
            qSender.Connect();
            qSender.Send(new OscMessage(msg,args));
            qSender.Close();
        }

    }
}
