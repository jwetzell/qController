using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Serilog;

namespace SharpOSC
{
    public class MessageEventArgs : EventArgs
    {
        public OscMessage Message
        {
            get;
            set;
        }
    }

    public class TCPClient
    {
        public int Port
        {
            get { return _port; }
        }
        int _port;

        public string Address
        {
            get { return _address; }
        }
        public delegate void MessageReceivedHandler(object source, MessageEventArgs args);
        public event MessageReceivedHandler MessageReceived;

        private Queue<OscPacket> SendQueue = new Queue<OscPacket>();

        private Thread receivingThread;
        private Thread sendThread;

        string _address;
        TcpClient client;

        byte END = 0xc0;
        byte ESC = 0xdb;
        byte ESC_END = 0xDC;
        byte ESC_ESC = 0xDD;


        public TCPClient(string address, int port)
        {
            _port = port;
            _address = address;
        }

        public bool Connect()
        {
            try
            {
                client = new TcpClient(Address, Port);
                receivingThread = new Thread(ReceiveLoop);
                receivingThread.Start();

                sendThread = new Thread(SendLoop);
                sendThread.Start();

                Log.Debug($"[tcpclient] connected to <{Address}:{Port}>");
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }

        }

        public void QueueForSending(OscPacket packet)
        {
            SendQueue.Enqueue(packet);
        }

        private void SendLoop()
        {
            while (client != null && client.Connected)
            {
                if (SendQueue.Count > 0)
                {
                    OscPacket packet = SendQueue.Dequeue();
                    Send(packet);
                }
            }
        }

        public void Send(byte[] message)
        {
            byte[] slipData = SlipEncode(message);
            NetworkStream netStream = client.GetStream();
            netStream.Write(slipData.ToArray(), 0, slipData.ToArray().Length);
        }

        public void Send(OscPacket packet)
        {
            byte[] data = packet.GetBytes();
            Send(data);
        }

        public bool IsConnected
        {
            get
            {
                if (client == null)
                    return false;
                else
                    return client.Connected;
            }
        }

        public void ReceiveLoop()
        {
            while (client != null && client.Connected)
            {
                Receive();
            }
            //Log.Debug("[tcpclient] - ReceiveLoop has exited");
        }

        public void Receive()
        {
            Random random = new Random();
            int num = random.Next(1000);
            try
            {
                NetworkStream netStream = client.GetStream();
                netStream.ReadTimeout = 250;
                List<byte> responseData = new List<byte>();
                if (netStream.CanRead)
                {
                    //var watch = System.Diagnostics.Stopwatch.StartNew();
                    byte[] buffer = new byte[256];

                    int bytesRead = 0;
                    int reads = 0;
                    do
                    {
                        bytesRead = netStream.Read(buffer, 0, buffer.Length);
                        responseData.AddRange(buffer);
                        reads += 1;
                        Thread.Sleep(1);
                        //Log.Debug("Thread " + num + ":  Bytes read: " + bytesRead + " - " + Encoding.UTF8.GetString(buffer));
                    } while (netStream.DataAvailable);

                    //Console.WriteLine("Raw TCP In: " + System.Text.Encoding.UTF8.GetString(responseData.ToArray()));
                    OscPacket packet = OscPacket.GetPacket(responseData.Skip(1).ToArray());
                    OscMessage responseMessage = (OscMessage)packet;
                    //watch.Stop();
                    //Console.WriteLine($"TCPCLient - message receive took {watch.ElapsedMilliseconds}ms and {reads} reads");
                    OnMessageReceived(responseMessage);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("TCPSENDER - Receive Exception: " + e.ToString());
            }
        }

        public byte[] SlipEncode(byte[] data)
        {
            List<byte> slipData = new List<byte>();

            byte[] esc_end = { ESC, ESC_END };
            byte[] esc_esc = { ESC, ESC_ESC };
            byte[] end = { END };

            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                if (data[i] == END)
                {
                    slipData.AddRange(esc_end);
                }
                else if (data[i] == ESC)
                {
                    slipData.AddRange(esc_esc);
                }
                else
                {
                    slipData.Add(data[i]);
                }
            }
            slipData.AddRange(end);
            return slipData.ToArray();
        }

        public void Close()
        {
            if (client != null)
            {
                if (client.Connected)
                {
                    Log.Debug($"[tcpClient] closing connection to {Address}");
                    client.GetStream().Close();
                    client.Close();
                }
            }
        }

        protected virtual void OnMessageReceived(OscMessage msg)
        {
            MessageReceived?.Invoke(this, new MessageEventArgs() { Message = msg });
        }
    }
}