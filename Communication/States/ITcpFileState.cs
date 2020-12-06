using System;
using System.IO;
using Communication.Packets;

namespace Communication.States
{
    public interface ITcpFileState
    {
        public const int DefaultPartLength = 3000;
        public int PartLength { get; set; }

        public long FileLength { get; set; }
        public int BytesSent { get; set; }
        
        public string FileId { get; set; }
        public FileStream FileStream { get; set; }
        
        public virtual Packet NextPacket()
        {
            throw new NotImplementedException();
        }

        public virtual bool Finished()
        {
            return Convert.ToInt32(BytesSent) == FileLength;
        }
        
    }
}