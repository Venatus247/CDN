using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Commons;

namespace Core.Communication.Tcp.Server
{
    public class BasicTcpServer<TClient> where TClient : TcpConnectedClient, new()
    {
        private IPAddress Address { get; }
        private int Port { get; }
        
        private TcpListener Listener { get; set; }

        private readonly Dictionary<long, TClient> _clients = new Dictionary<long, TClient>();
        private int _nextFreeClientId = 0;
        
        public BasicTcpServer(IPAddress address, int port)
        {
            Address = address;
            Port = port;
        }

        public void Start()
        {
            Listener = new TcpListener(Address, Port);
            Listener.Start();
            Listener.BeginAcceptTcpClient(ClientConnectCallback, null);
            
            Logger.Info($"TCP Server started on {Port}.");
        }

        private void ClientConnectCallback(IAsyncResult result)
        {
            var client = Listener.EndAcceptTcpClient(result);
            
            Logger.Info($"New connection from {client.Client.RemoteEndPoint}");
            
            Listener.BeginAcceptTcpClient(ClientConnectCallback, null);
            
            var basicTcpClient = new TClient();
            basicTcpClient.Prepare(_nextFreeClientId++, client);
            
            _clients.Add(basicTcpClient.Id, new TClient());
        }
        
    }
}