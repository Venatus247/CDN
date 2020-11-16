using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CDN.Utils.Service;
using Commons;
using Core;
using Core.Communication.Messages;
using Core.Communication.Packets;
using Core.Communication.Tcp.Client;
using Core.Data.File;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CDN
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cdnClient = new BasicTcpClient(IPAddress.Parse("127.0.0.1"), 4500);
            
            var tempFiles = new Dictionary<string, FileStream>();
            var fileParts = new Dictionary<string, int>();
            var fileHeaders = new Dictionary<string, FileHeaderMessage>();

            cdnClient.Invoke(PacketCodes.Ping, packet =>
            {
                var message = SerializedPacket<PingMessage>.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                Logger.Debug($"Received message: {message.Message}");
            });
            
            cdnClient.Invoke(PacketCodes.FileHeader, packet =>
            {
                Logger.Debug("Received file header");
                var fileHeader = FileHeaderMessage.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                Logger.Debug($"File Header for {fileHeader.FileId}");

                var tempFile = Path.GetTempFileName();
                var fileStream = File.Create(tempFile);
                
                tempFiles.Add(fileHeader.FileId, fileStream);
                fileParts.Add(fileHeader.FileId, fileHeader.FileParts);
                fileHeaders.Add(fileHeader.FileId, fileHeader);
                
                Logger.Debug($"saving received file at: {tempFile}");
            });
            
            cdnClient.Invoke(PacketCodes.FilePart, packet =>
            {
                Logger.Debug("Received file part");
                var filePart = FilePartMessage.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                var tempFile = tempFiles[filePart.FileId];
                var fileStream = tempFiles[filePart.FileId];
                fileStream.Write(filePart.ByteData);

                if (fileParts[filePart.FileId] == filePart.PartIndex)
                {
                    fileStream.Close();
                    Logger.Debug("Finished receiving file.");
                    tempFiles.Remove(filePart.FileId);

                    var saveFileTask = Task.Run(async () =>
                    {
                        var fileHeader = fileHeaders[filePart.FileId];
                        await FilesService.SaveFile(new SavedFile()
                        {
                            FileId = filePart.FileId,
                            FileName = fileHeader.FileName,
                            ContentType = fileHeader.ContentType,
                            Description = fileHeader.Description,
                            Created = fileHeader.Created,
                            LastModified = fileHeader.LastModified,
                            Version = fileHeader.Version,
                            AccessLevel = fileHeader.AccessLevel,
                            FileOwner = fileHeader.FileOwner,
                            GrantedAccounts = fileHeader.GrantedAccounts
                        }, fileStream.Name);
                        cdnClient.Send(new CdnSavedFileMessage()
                        {
                            CdnId = FileController.Instance.GetCdnId(),
                            FileId = filePart.FileId
                        }.ToPacket());
                    });
                    
                }
                else
                {
                    Logger.Debug($"Received {filePart.PartIndex}/{fileParts[filePart.FileId]}");
                }
                
            });

            cdnClient.Start();
            
            cdnClient.Send(new CdnConnectedMessage()
            {
                CdnId = FileController.Instance.GetCdnId()
            }.ToPacket());
            
            BackendServer.Instance.OnStartup();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}