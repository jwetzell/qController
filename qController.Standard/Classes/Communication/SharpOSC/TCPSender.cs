using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace SharpOSC
{
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
        string _address;
        TcpClient client;
        Thread receivingThread;

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
            netStream.Close();
            client.Close();
        }

        public OscMessage Receive()
        {
            OscMessage response = new OscMessage("/null");
            try
            {
                NetworkStream netStream = client.GetStream();
                netStream.ReadTimeout = 250;
                List<byte> responseData = new List<byte>();
                if (netStream.CanRead)
                {
                    byte[] buffer = new byte[1024];

                    int bytesRead = 0;
                    do
                    {
                        bytesRead = netStream.Read(buffer, 0, buffer.Length);
                        responseData.AddRange(buffer);
                    } while (netStream.DataAvailable);
                    response = (OscMessage)OscPacket.GetPacket(responseData.Skip(1).ToArray());
                    netStream.Close();
                    client.Close();
                }
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return response;
        }

        public OscMessage SendAndReceive(byte[] message)
        {
            client = new TcpClient(Address, Port);
            byte[] slipData = SlipEncode(message);
            NetworkStream netStream = client.GetStream();
            netStream.Write(slipData.ToArray(), 0, slipData.ToArray().Length);
            OscMessage response = Receive();
            return response;
        }

        public OscMessage SendAndReceive(OscPacket packet)
        {
            byte[] data = packet.GetBytes();
            OscMessage response = SendAndReceive(data);
            return response;
        }
        public void StartReceiving()
        {
            receivingThread.Start();
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
            receivingThread.Abort();
            client.Close();
        }
    }
}
