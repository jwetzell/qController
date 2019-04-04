using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

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
            Console.WriteLine("Available: " + client.Available);
            Console.WriteLine("Connected: " + client.Connected);
            List<byte> slipData= new List<byte>();

            byte[] esc_end = { ESC, ESC_END };
            byte[] esc_esc = { ESC, ESC_ESC };
            byte[] end = { END };

            int length = message.Length;
            for (int i = 0; i < length; i++)
            {
                if (message[i] == END)
                {
                    slipData.AddRange(esc_end);
                } else if (message[i] == ESC)
                {
                    slipData.AddRange(esc_esc);
                }
                else
                {
                    slipData.Add(message[i]);
                }
            }
            slipData.AddRange(end);
            NetworkStream netStream = client.GetStream();
            netStream.Write(slipData.ToArray(), 0, slipData.ToArray().Length);
            netStream.Close();
            client.Close();
        }

        public void Receive()
        {
            NetworkStream netStream = client.GetStream();
            List<byte> responseData = new List<byte>();
            if (netStream.CanRead)
            {
                byte[] buffer = new byte[512];

                int bytesRead = 0;
                do
                {
                    bytesRead = netStream.Read(buffer, 0, buffer.Length);
                    responseData.AddRange(buffer);

                } while (netStream.DataAvailable);
                OscMessage response = (OscMessage)OscPacket.GetPacket(responseData.Skip(1).ToArray());
            }
            netStream.Close();
        }
        public void Send(OscPacket packet)
        {
            byte[] data = packet.GetBytes();
            Send(data);
        }

        public void Close()
        {
            client.GetStream().Close();
            client.Close();
        }
    }
}
