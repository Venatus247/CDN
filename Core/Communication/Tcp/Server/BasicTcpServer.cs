using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Commons;

namespace Core.Communication.Tcp.Server
{
    public class BasicTcpServer<TClient> where TClient : TcpConnectedClient, new()
    {
        private readonly IPAddress _address;
        private readonly int _port;
        
        private TcpListener Listener { get; set; }

        private readonly Dictionary<long, TClient> _clients = new Dictionary<long, TClient>();
        private int _nextFreeClientId = 1;
        
        public BasicTcpServer(IPAddress address, int port)
        {
            _address = address;
            _port = port;
        }
        
        public void Start()
        {
            Listener = new TcpListener(_address, _port);
            Listener.Start();
            Listener.BeginAcceptTcpClient(ClientConnectCallback, null);
            
            Logger.Info($"TCP Server started on {_port}.");
        }

        private void ClientConnectCallback(IAsyncResult result)
        {
            try
            {
                var client = Listener.EndAcceptTcpClient(result);
            
                Listener.BeginAcceptTcpClient(ClientConnectCallback, null);
                
                Logger.Info($"New connection from {client.Client.RemoteEndPoint} as client {_nextFreeClientId}");
                
                var tClient = new TClient();
                tClient.Prepare(_nextFreeClientId++, client);
                
                _clients.Add(tClient.Id, tClient);
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        public TcpConnectedClient GetClientConnection(int id)
        {
            return _clients[id];
        }
        
    }
}