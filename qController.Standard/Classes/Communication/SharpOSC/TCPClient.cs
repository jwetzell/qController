using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;
using TcpSharp.Events;

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
        private ILogger _log = Log.Logger.ForContext<TCPClient>();

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

        TcpSharp.TcpSharpSocketClient tcpClient;

        private List<byte> frameStream = new List<byte>();

        public TCPClient(string address, int port)
        {
            _port = port;
            _address = address;
        }

        public bool Connect()
        {

            try
            {
                tcpClient = new TcpSharp.TcpSharpSocketClient(Address, Port);
                tcpClient.OnConnected += ClientConnected;
                tcpClient.OnDataReceived += DataReceived;
                tcpClient.OnDisconnected += ClientDisconneted;
                tcpClient.OnError += ClientError;
                tcpClient.Connect();
                return true;
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return false;
            }

        }
        private void ClientError(object sender, OnClientErrorEventArgs e)
        {
            _log.Error("client error");
            _log.Error(e.Exception.ToString());
        }

        private void ClientDisconneted(object sender, OnClientDisconnectedEventArgs e)
        {
            _log.Information($"client disconnected: {e.Reason}");
            Close();
        }

        private void DataReceived(object sender, OnClientDataReceivedEventArgs e)
        {
            frameStream.AddRange(e.Data);

            int i = 0;
            int frameEnd = frameStream.FindIndex(1, frameByte => frameByte.Equals(SlipFrame.END));
            while (frameEnd > 0)
            {
                List<byte> frame = frameStream.GetRange(0, frameEnd + 1);
                frameStream.RemoveRange(0, frameEnd + 1);
                List<byte[]> messages = SlipFrame.Decode(frame.ToArray());
                foreach (var message in messages)
                {
                    try
                    {
                        OscPacket packet = OscPacket.GetPacket(message);
                        OscMessage responseMessage = (OscMessage)packet;
                        if (packet == null)
                        {
                            _log.Error("packet is null");
                        }

                        if (responseMessage == null)
                        {
                            _log.Error("responeMessage is null");
                        }
                        OnMessageReceived(responseMessage);
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Exception parsing OSC message: {ex.ToString()}");
                    }
                }
                frameEnd = frameStream.FindIndex(0, frameByte => frameByte.Equals(SlipFrame.END));
            }
        }

        private void ClientConnected(object sender, OnClientConnectedEventArgs e)
        {
            _log.Debug($"connected to <{Address}:{Port}>");
        }

        public void Send(byte[] message)
        {
            byte[] slipData = SlipFrame.Encode(message);
            tcpClient.SendBytes(slipData.ToArray());
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
                if (tcpClient == null)
                    return false;
                else
                    return tcpClient.Connected;
            }
        }

        public void Close()
        {
            if (tcpClient != null)
            {
                tcpClient.OnConnected -= ClientConnected;
                tcpClient.OnDataReceived -= DataReceived;
                tcpClient.OnDisconnected -= ClientDisconneted;
                if (tcpClient.Connected)
                {
                    _log.Debug($"closing connection to {Address}");
                    tcpClient.Disconnect();
                }
            }
        }

        protected virtual void OnMessageReceived(OscMessage msg)
        {
            MessageReceived?.Invoke(this, new MessageEventArgs() { Message = msg });
        }
    }
}