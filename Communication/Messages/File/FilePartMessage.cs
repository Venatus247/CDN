using System;
using Communication.Packets;

namespace Communication.Messages.File
{
    [Serializable]
    public class FilePartMessage : SerializedPacket<FilePartMessage>
    {
        public const long DefaultContentLength = 32768;

        public long ContentLength = DefaultContentLength;
        public string FileId { get; set; }
        public int PartIndex { get; set; }
        public byte[] ByteData;

        public FilePartMessage()
        {
            PacketId = (int) PacketCodes.FilePart;
            ByteData = new byte[ContentLength];
        }
        
        public FilePartMessage(long contentLength) : base((int) PacketCodes.FilePart)
        {
            ContentLength = contentLength;
            ByteData = new byte[ContentLength];
        }
        
    }
}