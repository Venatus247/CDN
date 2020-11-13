using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Commons;
using Core.Communication.Packets;

namespace Core.Communication.Tcp.Client
{
    public class BasicTcpClient
    {
        public const int DataBufferSize = 4096;

        private readonly IPAddress _address;
        private readonly int _port;

        private TcpClient _client;

        private NetworkStream _stream;
        private byte[] _receivedBuffer = new byte[DataBufferSize];
        
        private List<Packet> _waitingQueue = new List<Packet>();
        
        public BasicTcpClient(IPAddress address, int port)
        {
            _address = address;
            _port = port;
        }

        public void Start()
        {
            Connect();
        }
        
        private void Connect()
        {
            _client = new TcpClient()
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };
            _client.BeginConnect(_address, _port, ConnectCallback, _client);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                _client.EndConnect(result);
                if (!_client.Connected)
                {
                    Logger.Error("Failed to connect to server");
                    return;
                }

                _stream = _client.GetStream();

                foreach (var packet in _waitingQueue)
                    Send(packet);
                
                _stream.BeginRead(_receivedBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Logger.Error("Error connecting to server");
                Console.WriteLine(e);
                //throw;
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var bytesLength = _stream.EndRead(result);
                if(bytesLength <= 0)
                    return;

                var data = new byte[bytesLength];
                Array.Copy(_receivedBuffer, data, bytesLength);
                
                _stream.BeginRead(_receivedBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
            }
        }

        public void Send(Packet packet)
        {
            try
            {
                if (_client == null || !_client.Connected)
                {
                    _waitingQueue.Add(packet);
                    return;
                }
                
                packet.WriteLength();
                _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), SendCallback, null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            
        }
        
    }
}