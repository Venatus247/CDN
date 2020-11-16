using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Commons;
using Core.Communication.Messages;
using Core.Communication.Packets;

namespace Core.Communication.Tcp.Client
{
    public class BasicTcpClient : BasicTcpCommunication
    {
        
        private readonly IPAddress _address;
        private readonly int _port;
        
        private readonly Dictionary<string, FileStream> _tempFiles = new Dictionary<string, FileStream>();
        private readonly Dictionary<string, int> _fileParts = new Dictionary<string, int>();
        
        public BasicTcpClient(IPAddress address, int port)
        {
            _address = address;
            _port = port;
            
            PacketHandlers.Add((int)PacketCodes.Ping, packet =>
            {
                var message = SerializedPacket<PingMessage>.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                Logger.Debug($"Received message: {message.Message}");
            });
            
            PacketHandlers.Add((int) PacketCodes.FileHeader, packet =>
            {
                Logger.Debug("Received file header");
                var fileHeader = FileHeaderMessage.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                Logger.Debug($"File Header for {fileHeader.FileId}");

                var tempFile = Path.GetTempFileName();
                var fileStream = File.Create(tempFile);
                
                _tempFiles.Add(fileHeader.FileId, fileStream);
                _fileParts.Add(fileHeader.FileId, fileHeader.FileParts);
                
                Logger.Debug($"saving received file at: {tempFile}");
            });
            
            PacketHandlers.Add((int) PacketCodes.FilePart, packet =>
            {
                Logger.Debug("Received file part");
                var filePart = FilePartMessage.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                var tempFile = _tempFiles[filePart.FileId];
                var fileStream = _tempFiles[filePart.FileId];
                fileStream.Write(filePart.ByteData);

                if (_fileParts[filePart.FileId] == filePart.PartIndex)
                {
                    fileStream.Close();
                    Logger.Debug("Finished receiving file.");
                }
                else
                {
                    Logger.Debug($"Received {filePart.PartIndex}/{_fileParts[filePart.FileId]}");
                }
                
            });
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