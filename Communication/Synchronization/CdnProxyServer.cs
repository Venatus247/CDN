using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Commons;
using Communication.Data.File;
using Communication.Messages;
using Communication.Messages.Authentication;
using Communication.Messages.File;
using Communication.Packets;
using Communication.States;
using Communication.Tcp.Server;
using Core.Data.Cdn;
using Core.Data.File;
using MongoDB.Driver;

namespace Communication.Synchronization
{
    public class CdnProxyServer : BasicTcpServer<CdnClientConnection>
    {
        private readonly List<UploadedFileInfo> _waitingQueue = new List<UploadedFileInfo>();
        
        public CdnProxyServer(string address, int port) : base(IPAddress.Parse(address), port)
        {
        }

        public async void SaveFile(UploadedFileInfo fileInfo)
        {
            if (CdnClientReference.Count == 0)
            {
                _waitingQueue.Add(fileInfo);
                return;
            }

            var tcpClient = GetBestCdn();
            
            tcpClient.SendFile(new CdnFileState(fileInfo.CreateFileHeader(), fileInfo.FileStream));
        }

        protected override void NewCdnRegisteredCallback(CdnClientConnection client)
        {
            base.NewCdnRegisteredCallback(client);

            for (var i = _waitingQueue.Count - 1; i >= 0; i--)
            {
                SaveFile(_waitingQueue[i]);
                _waitingQueue.RemoveAt(i);
            }
        }

        private TcpConnectedClient GetBestCdn()
        {
            if (CdnClientReference.Count == 0)
            {
                return null;
            }
            
            //TODO improve cdn selection
            var random = new Random();
            return Clients[CdnClientReference.Select(x => x.Value).ToList()[random.Next(0, CdnClientReference.Count - 1)]];
        }
        
        protected override void InvokeClient(CdnClientConnection tClient)
        {
            tClient.Invoke(PacketCodes.CdnAuth, packet =>
            {
                try
                {
                    var message = CdnAuthMessage.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                    
                    var found = CdnController.Instance.Collection.FindSync(x => 
                        x.CdnId.Equals(message.CdnReference.CdnId) && 
                        x.CdnAuthToken.Equals(message.CdnReference.CdnAuthToken) && 
                        x.IpAddress.Equals(tClient.IpAddress())).FirstOrDefault();
                    
                    if (found == null)
                    {
                        //Debug purposes only (easy way to add new cdn)
                        //dnController.Instance.Collection.InsertOne(message.CdnReference);
                        Logger.Warn($"{tClient.IpAddress()} tried to authenticate as cdn! Client was disconnected.");
                        tClient.Kick();
                        return;
                    }
                    
                    Logger.Info($"Cdn with id '{message.CdnReference.CdnId}' connected.");
                    
                    message.CdnReference.Id = found.Id;

                    tClient.CdnReference = message.CdnReference;
                    
                    CdnClientReference[message.CdnReference.CdnId] = tClient.Id;
                    Clients.Add(tClient.Id, tClient);
                    
                    NewCdnRegisteredCallback(tClient);
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
                    if (!tClient.IsAuthenticated())
                    {
                        tClient.Kick();
                        return;
                    }

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
            
            base.InvokeClient(tClient);
        }
    }
}