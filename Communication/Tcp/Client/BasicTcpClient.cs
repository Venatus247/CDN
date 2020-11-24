using System;
using System.Net;
using System.Net.Sockets;
using Commons;

namespace Communication.Tcp.Client
{
    public class BasicTcpClient : BasicTcpCommunication
    {
        
        private readonly IPAddress _address;
        private readonly int _port;
        
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
            Client = new TcpClient()
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };
            Client.BeginConnect(_address, _port, ConnectCallback, Client);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                Client.EndConnect(result);
                if (!Client.Connected)
                {
                    Logger.Error("Failed to connect to server");
                    return;
                }

                Stream = Client.GetStream();

                foreach (var packet in WaitingQueue)
                    Send(packet);
                
                Stream.BeginRead(ReceivedBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Logger.Error("Error connecting to server");
                Logger.Exception(e);
            }
        }
        
    }
}