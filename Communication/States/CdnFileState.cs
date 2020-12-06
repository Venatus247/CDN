using System;
using System.IO;
using Commons;
using Communication.Messages.File;
using Communication.Packets;

namespace Communication.States
{
    public class CdnFileState : ITcpFileState
    {
        public const int DefaultPartLength = 1048576; //1mb
        public int PartLength { get; set; }
        public long FileLength { get; set; }
        public int CurrentPartIndex { get; set; } = 1;
        public int BytesSent { get; set; } = -1;
        public string FileId { get; set; }
        public FileStream FileStream { get; set; }

        private readonly FileHeaderMessage _fileHeaderMessage;
        
        public CdnFileState(FileHeaderMessage headerMessage, FileStream fileStream, int partLength = DefaultPartLength)
        {
            PartLength = partLength;
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