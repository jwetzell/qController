using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

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
            client = new TcpClient();

            if (!client.ConnectAsync(Address, Port).Wait(TimeSpan.FromSeconds(1)))
            {
                return false;
            }

            Thread receivingThread = new Thread(ReceiveLoop);
            receivingThread.Start();
            return true;
            
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
                return client.Connected;
            }
        }

        public void ReceiveLoop()
        {
            while (client.Connected)
            {
                Receive();
            }
            Console.WriteLine("TCPClient - Receive Loop has exited for some reason");
        }

        public void Receive()
        {
            Random random = new Random();
            int num = random.Next(1000);
            NetworkStream netStream = client.GetStream();
            try
            {
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
                    OscMessage response = (OscMessage)OscPacket.GetPacket(responseData.Skip(1).ToArray());
                    //watch.Stop();
                    //Console.WriteLine($"TCPCLient - message receive took {watch.ElapsedMilliseconds}ms and {reads} reads");
                    OnMessageReceived(response);
                }
            } catch(Exception e)
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
            if (client.Connected)
            {
                client.GetStream().Close();
                client.Close();
            }
            
        }

        protected virtual void OnMessageReceived(OscMessage msg)
        {
            MessageReceived?.Invoke(this, new MessageEventArgs() { Message = msg });
        }
    }
}
