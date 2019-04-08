//this class is used for receiving workspace update messages

using System;
using SharpOSC;
using System.Net;
using System.Threading;

namespace qController
{

    public class QReceiver
    {
        public delegate void UpdateMessageReceivedHandler(object source, MessageEventArgs args);
        public event UpdateMessageReceivedHandler UpdateMessageReceived;

        static Thread thread;
        static UDPListener udpListener;

        public QReceiver(int port)
        {
            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageReceived = (OscMessage)packet;
                if(messageReceived != null)
                    OnUpdateMessageReceived(messageReceived);
                else
                    Console.WriteLine("Message came back null");
            };

            udpListener = new UDPListener(port, callback);

        }

		public void ListenLoop()
		{
            /*while (true)
            {
                OscMessage messageReceived = null;
                while(messageReceived == null)
                {
                    messageReceived = (OscMessage)udpListener.Receive();
                    Thread.Sleep(1);
                }
                OnUpdateMessageReceived(messageReceived);

            }*/

        }

        public void Close(){
            udpListener.Close();
        }

        protected virtual void OnUpdateMessageReceived(OscMessage msg)
        {
            if (UpdateMessageReceived != null)
                UpdateMessageReceived(this, new MessageEventArgs() { Message = msg });
        }
    }
}
