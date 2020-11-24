using System;
using Communication.Packets;

namespace Communication.Messages
{
    [Serializable]
    public class CdnSavedFileMessage : SerializedPacket<CdnSavedFileMessage>
    {
        public string FileId { get; set; }
        public string CdnId { get; set; }
        public CdnSavedFileMessage() : base((int) PacketCodes.CdnSavedFile)
        {
        }
    }
}