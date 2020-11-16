using System;
using System.IO;
using Commons;
using Core.Communication.Messages;
using Core.Communication.Packets;

namespace Core.Communication.States
{
    public class CdnFileState : ITcpFileState
    {
        public long DefaultPartLength { get; set; } = 3000;
        public int PartLength { get; set; } = 3000;
        public long FileLength { get; set; }
        public int CurrentPartIndex { get; set; } = 1;
        public int BytesSent { get; set; } = -1;
        public string FileId { get; set; }
        public FileStream FileStream { get; set; }

        private readonly FileHeaderMessage _fileHeaderMessage;
        
        public CdnFileState(FileHeaderMessage headerMessage, FileStream fileStream)
        {
            FileStream = fileStream;
            FileLength = FileStream.Length;
            _fileHeaderMessage = headerMessage;
            headerMessage.FileParts = Convert.ToInt32(Math.Ceiling( ((double)FileLength) / ((double)PartLength) ));
        }

        public Packet NextPacket()
        {
            if (BytesSent == -1)
            {
                BytesSent = 0;
                return _fileHeaderMessage.ToPacket();
            }
            
            var message = new FilePartMessage(PartLength)
            {
                FileId = _fileHeaderMessage.FileId,
                PartIndex = CurrentPartIndex++
            };

            var toSend = (BytesSent + PartLength) > FileLength ? (FileLength - BytesSent) : PartLength;
            var toSend2 = Convert.ToInt32(toSend);
            
            Logger.Info($"FileLength: {FileLength}");
            Logger.Info($"Offset: {BytesSent}");
            Logger.Info($"ToSend: {toSend2}");
            
            FileStream.Read(message.ByteData, 0, toSend2);
            BytesSent += toSend2;

            return message.ToPacket();
        }
    }
}