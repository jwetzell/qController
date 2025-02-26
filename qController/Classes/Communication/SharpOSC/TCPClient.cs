﻿using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using TcpSharp;

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
        public event MessageReceivedHandler? MessageReceived;

        string _address;

        TcpSharpSocketClient? tcpClient;

        public TCPClient(string address, int port)
        {
            _port = port;
            _address = address;
        }

        public bool Connect()
        {

            try
            {
                tcpClient = new TcpSharpSocketClient(_address, _port);
                tcpClient.OnConnected += ClientConnected;
                tcpClient.OnDataReceived += DataReceived;
                tcpClient.OnDisconnected += ClientDisconneted;
                tcpClient.OnError += ClientError;
                tcpClient.Connect();
                return true;
            }
            catch (Exception e)
            {
                _log.Error(e.StackTrace);
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
            List<byte[]> messages = SlipFrame.Decode(e.Data);
            _log.Debug("tcpclient: " +  messages.Count + " messages pulled from slip");
            foreach (var message in messages)
            {
                try
                {

                    OscMessage responseMessage = OscPacket.GetMessage(message);
                    if (responseMessage == null)
                    {
                        _log.Error("responseMessage is null");
                    } else
                    {
                        OnMessageReceived(responseMessage);
                    }

                }
                catch (Exception ex)
                {
                    _log.Error($"Exception parsing OSC message: {ex.ToString()}");
                }
            }
        }

        private void ClientConnected(object sender, OnClientConnectedEventArgs e)
        {
            _log.Debug($"connected to <{_address}:{_port}>");
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
                    _log.Debug($"closing connection to {_address}");
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