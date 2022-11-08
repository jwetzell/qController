using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Serilog;
using SuperSimpleTcp;

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

        SimpleTcpClient tcpClient;



        public TCPClient(string address, int port)
        {
            _port = port;
            _address = address;
        }

        public bool Connect()
        {

            try
            {
                tcpClient = new SimpleTcpClient(Address, Port);
                tcpClient.Events.Connected += ClientConnected;
                tcpClient.Events.DataReceived += DataReceived;
                tcpClient.Events.Disconnected += ClientDisconneted;
                tcpClient.Logger += TCPLog;
                tcpClient.Connect();
                return true;
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return false;
            }

        }

        private void TCPLog(string obj)
        {
            _log.Verbose($"{obj}");
        }

        private void ClientDisconneted(object sender, ConnectionEventArgs e)
        {
            _log.Verbose($"{e.IpPort} client disconnected: {e.Reason}");
            Close();
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {

            _log.Verbose($"Raw Data Received contents: {Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count)}");
            _log.Verbose($"Raw Data Received size: {e.Data.Count}");
            List<byte[]> messages = SlipFrame.Decode(e.Data.Array);
            _log.Verbose($"Slip decoded {messages.Count} osc messages");
            foreach (var message in messages)
            {
                _log.Verbose($"Raw message contents: {Encoding.UTF8.GetString(message, 0, message.Length)}");
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

                    _log.Debug($"OSC Message Received: {responseMessage.Address}");
                    OnMessageReceived(responseMessage);
                    _log.Debug($"After OnMessageReceived Event");

                }
                catch (Exception ex)
                {
                    _log.Error($"Exception parsing OSC message: {ex.ToString()}");
                }
            }
        }

        private void ClientConnected(object sender, ConnectionEventArgs e)
        {
            _log.Debug($"connected to <{Address}:{Port}>");
        }

        public void Send(byte[] message)
        {
            byte[] slipData = SlipFrame.Encode(message);
            tcpClient.Send(slipData.ToArray());
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
                    return tcpClient.IsConnected;
            }
        }

        public void Close()
        {
            if (tcpClient != null)
            {
                tcpClient.Events.Connected -= ClientConnected;
                tcpClient.Events.DataReceived -= DataReceived;
                tcpClient.Events.Disconnected -= ClientDisconneted;
                if (tcpClient.IsConnected)
                {
                    _log.Debug($"closing connection to {Address}");
                    tcpClient.Disconnect();
                    tcpClient.Dispose();
                }
            }
        }

        protected virtual void OnMessageReceived(OscMessage msg)
        {
            MessageReceived?.Invoke(this, new MessageEventArgs() { Message = msg });
        }
    }
}