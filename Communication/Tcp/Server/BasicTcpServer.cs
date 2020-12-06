using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Commons;

namespace Communication.Tcp.Server
{
    public class BasicTcpServer<TClient> where TClient : TcpConnectedClient, new()
    {
        protected readonly IPAddress Address;
        protected readonly int Port;
        
        private TcpListener Listener { get; set; }

        protected readonly Dictionary<long, TClient> Clients = new Dictionary<long, TClient>();
        protected readonly Dictionary<string, long> CdnClientReference = new Dictionary<string, long>();
        private int _nextFreeClientId = 1;
        
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
            try
            {
                var client = Listener.EndAcceptTcpClient(result);
            
                Listener.BeginAcceptTcpClient(ClientConnectCallback, null);
                
                Logger.Info($"New connection from {client.Client.RemoteEndPoint} as client {_nextFreeClientId}");
                
                var tClient = new TClient();
                tClient.Prepare(_nextFreeClientId++, client);

                InvokeClient(tClient);
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        protected virtual void NewCdnRegisteredCallback(TClient client) {}
        
        protected virtual void InvokeClient(TClient tClient)
        {
        }

        public TcpConnectedClient GetClientConnection(int id)
        {
            return Clients[id];
        }

        public TcpConnectedClient GetClientConnection(string cdnId)
        {
            return Clients[CdnClientReference[cdnId]];
        }
        
    }
}