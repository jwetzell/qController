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

    public class TCPSender
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


        public TCPSender(string address, int port)
        {
            _port = port;
            _address = address;
        }

        public void Send(byte[] message)
        {
            client = new TcpClient(Address, Port);
            byte[] slipData = SlipEncode(message);
            NetworkStream netStream = client.GetStream();
            netStream.Write(slipData.ToArray(), 0, slipData.ToArray().Length);
        }

        public async void Receive()
        {
            Random random = new Random();
            int num = random.Next(1000);

            OscMessage response = new OscMessage("/null");
            NetworkStream netStream = client.GetStream();
            try
            {
                netStream.ReadTimeout = 250;
                List<byte> responseData = new List<byte>();
                if (netStream.CanRead)
                {
                    byte[] buffer = new byte[1024];

                    int bytesRead = 0;
                    do
                    {
                        bytesRead = await netStream.ReadAsync(buffer, 0, buffer.Length);
                        responseData.AddRange(buffer);
                        Thread.Sleep(1);
                        //Console.WriteLine(  "Thread " + num + ":  Bytes read: " + bytesRead + " - " + Encoding.ASCII.GetString(buffer));
                    } while (netStream.DataAvailable);

                    //Console.WriteLine("Raw TCP In: " + System.Text.Encoding.ASCII.GetString(responseData.ToArray()));
                    response = (OscMessage)OscPacket.GetPacket(responseData.Skip(1).ToArray());
                }
            } catch(Exception e)
            {
                Console.WriteLine("RECEIVE EXCEPTION: " + e.ToString());
            }
            OnMessageReceived(response);
        }

        public void SendAndReceive(byte[] message)
        {

            client = new TcpClient(Address, Port);
            byte[] slipData = SlipEncode(message);
            NetworkStream netStream = client.GetStream();
            netStream.Write(slipData.ToArray(), 0, slipData.ToArray().Length);
            Receive();
        }

        public void SendAndReceive(OscPacket packet)
        {
            OscMessage msg = (OscMessage)packet;
            //Console.WriteLine("TCP Message Sent: " + msg.Address);
            byte[] data = packet.GetBytes();
            SendAndReceive(data);
        }

        public void Send(OscPacket packet)
        {
            byte[] data = packet.GetBytes();
            Send(data);
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
            client.GetStream().Close();
            client.Close();
        }

        protected virtual void OnMessageReceived(OscMessage msg)
        {
            if (MessageReceived != null)
                MessageReceived(this, new MessageEventArgs() { Message = msg });
        }
    }
}
