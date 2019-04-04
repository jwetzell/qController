using System;
using SharpOSC;
using System.Net;
using System.Threading;

namespace qController
{
    public class MessageEventArgs : EventArgs{
        public OscMessage Message {
            get;
            set;
        }
    }

    public class QReceiver
    {
        static Thread thread;
        private QController owner;
        static UDPListener udpListener;
        public delegate void MessageReceivedHandler(object source, MessageEventArgs args);

        public event MessageReceivedHandler MessageReceived;

        public QReceiver(int port, QController ctrl)
        {
            owner = ctrl;

            udpListener = new UDPListener(port);

            thread = new Thread(new ThreadStart(ListenLoop));
            thread.Start();

        }

		public void ListenLoop()
		{
            Console.WriteLine("Loop Started");

            while (true)
            {
                OscMessage messageReceived = null;
                while(messageReceived == null)
                {
                    messageReceived = (OscMessage)udpListener.Receive();
                    Thread.Sleep(1);
                }
                Console.WriteLine("Received a message!");
                Console.WriteLine(messageReceived.Address);
                OnMessageReceived(messageReceived);
            }

            Console.WriteLine("Loop Ended");

        }

        public void Close(){
            thread.Abort();
            udpListener.Close();
        }

        protected virtual void OnMessageReceived(OscMessage msg){
            if (MessageReceived != null)
                MessageReceived(this, new MessageEventArgs(){Message = msg});
        }
    }
}
