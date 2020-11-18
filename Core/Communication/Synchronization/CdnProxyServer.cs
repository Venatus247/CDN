using System;
using System.Net;
using Commons;
using Core.Communication.Messages;
using Core.Communication.Messages.Authentication;
using Core.Communication.Packets;
using Core.Communication.Tcp.Server;
using Core.Data.Cdn;
using Core.Data.File;
using MongoDB.Driver;

namespace Core.Communication.Synchronization
{
    public class CdnProxyServer : BasicTcpServer<CdnClientConnection>
    {
        public CdnProxyServer(string address, int port) : base(IPAddress.Parse(address), port)
        {
            
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
                    
                    //CdnController.Instance.Collection.ReplaceOne(x => x.CdnId.Equals(message.CdnReference.CdnId),
                    //    message.CdnReference);

                    tClient.CdnReference = message.CdnReference;
                    
                    CdnClientReference[message.CdnReference.CdnId] = tClient.Id;
                    Clients.Add(tClient.Id, tClient);
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