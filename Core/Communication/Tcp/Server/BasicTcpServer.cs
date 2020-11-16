using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Commons;
using Core.Communication.Messages;
using Core.Communication.Packets;
using Core.Data.Cdn;
using Core.Data.File;
using MongoDB.Driver;

namespace Core.Communication.Tcp.Server
{
    public class BasicTcpServer<TClient> where TClient : TcpConnectedClient, new()
    {
        private readonly IPAddress _address;
        private readonly int _port;
        
        private TcpListener Listener { get; set; }

        private readonly Dictionary<long, TClient> _clients = new Dictionary<long, TClient>();
        private readonly Dictionary<string, long> _cdnClientReference = new Dictionary<string, long>();
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
                
                tClient.Invoke(PacketCodes.CdnConnected, packet =>
                {
                    try
                    {
                        var message = CdnConnectedMessage.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                        Logger.Debug($"Cdn connected with id '{message.CdnId}'");
                        _cdnClientReference[message.CdnId] = tClient.Id;

                        var cdnReference = new CdnReference()
                        {
                            CdnId = message.CdnId,
                            CdnUrlAddress = "https://localhost:4001"
                        };

                        var found = CdnController.Instance.Collection.FindSync(x => x.CdnId.Equals(cdnReference.CdnId)).FirstOrDefault();
                        if (found == null)
                        {
                            CdnController.Instance.Collection.InsertOne(cdnReference);
                        }
                        else
                        {
                            cdnReference.Id = found.Id;
                            CdnController.Instance.Collection.ReplaceOne(x => x.CdnId.Equals(cdnReference.CdnId),
                                cdnReference);
                        }
                        
                    }
                    catch (Exception e)
                    {
                        Logger.Exception(e);
                    }
                });
                
                tClient.Invoke(PacketCodes.CdnSavedFile, packet =>
                {
                    try
                    {
                        var message = CdnSavedFileMessage.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                        Logger.Debug($"cdn with id '{message.CdnId}' successfully saved file with id '{message.FileId}'");

                        var savedFile = FileController.Instance.Collection
                            .FindSync(x => x.FileId.Equals(message.FileId)).FirstOrDefault();
                        if (savedFile == null)
                        {
                            Logger.Error($"Could not find file with id '{message.FileId}' in database!");
                            return;
                        }
                        
                        savedFile.FileStoredAt.Add(new CdnFileReference()
                        {
                            CdnId = message.CdnId
                        });

                        FileController.Instance.Collection.ReplaceOne(x => x.Id.Equals(savedFile.Id), savedFile);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });
                
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

        public TcpConnectedClient GetClientConnection(string cdnId)
        {
            return _clients[_cdnClientReference[cdnId]];
        }
        
    }
}