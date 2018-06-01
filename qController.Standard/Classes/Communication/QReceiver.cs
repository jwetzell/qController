using System;
using Rug.Osc;
using System.Net;
using System.Threading;

namespace qController
{
    public class PacketEventArgs : EventArgs{
        public OscPacket Packet {
            get;
            set;
        }
    }

    public class QReceiver
    {
        static OscReceiver qReceiver;
        static Thread thread;
        private QController owner;

        public delegate void PacketReceivedHandler(object source, PacketEventArgs args);

        public event PacketReceivedHandler PacketReceived;

        public QReceiver(int port, QController ctrl)
        {
            qReceiver = new OscReceiver(port);

            thread = new Thread(new ThreadStart(ListenLoop));
            owner = ctrl;
            qReceiver.Connect();
            thread.Start();

        }

		public void ListenLoop()
		{
            Console.WriteLine("Loop Started");
			try
			{
                while (qReceiver.State != OscSocketState.Closed)
				{
					// if we are in a state to recieve
					if (qReceiver.State == OscSocketState.Connected)
					{
						// get the next message 
						// this will block until one arrives or the socket is closed
                        OscPacket packet = qReceiver.Receive();

                        OnPacketReceived(packet);
                        // Write the packet to the console 

						// DO SOMETHING HERE!
					}
				}
			}
			catch (Exception ex)
			{
				// if the socket was connected when this happens
				// then tell the user
				if (qReceiver.State == OscSocketState.Connected)
				{
					Console.WriteLine("Exception in listen loop");
					Console.WriteLine(ex.Message);
				}
			}
            Console.WriteLine("Loop Ended");
		}

        public void Close(){
            qReceiver.Close();
            thread.Abort();
        }

        protected virtual void OnPacketReceived(OscPacket pkt){
            if (PacketReceived != null)
                PacketReceived(this, new PacketEventArgs(){Packet = pkt});
        }
    }
}
