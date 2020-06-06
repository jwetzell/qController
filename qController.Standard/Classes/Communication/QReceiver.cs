//Class used for receiving workspace update messages (UDP)
using SharpOSC;
using System.Threading;
using Serilog;

namespace qController
{

    public class QReceiver
    {
        public delegate void UpdateMessageReceivedHandler(object source, MessageEventArgs args);
        public event UpdateMessageReceivedHandler UpdateMessageReceived;

        static Thread thread;
        static UDPListener udpListener;
        static int Port;
        public QReceiver(int port)
        {
            Port = port;
            udpListener = new UDPListener(port);
            thread = new Thread(new ThreadStart(ListenLoop));
            thread.Start();

        }

		public void ListenLoop()
		{
            /*
            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageReceived = (OscMessage)packet;
                if (messageReceived != null)
                    OnUpdateMessageReceived(messageReceived);
                else
                    Log.Debug("Message came back null");
            };

            udpListener = new UDPListener(Port, callback);
            */
            while (true)
            {
                OscMessage messageReceived = null;
                while(messageReceived == null)
                {
                    messageReceived = (OscMessage)udpListener.Receive();
                    Thread.Sleep(1);
                }
                OnUpdateMessageReceived(messageReceived);
            }           

        }

        public void Close(){

            thread.Abort();
            udpListener.Close();
        }

        protected virtual void OnUpdateMessageReceived(OscMessage msg)
        {
            if (UpdateMessageReceived != null)
                UpdateMessageReceived(this, new MessageEventArgs() { Message = msg });
        }
    }
}
