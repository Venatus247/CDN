using System;
using Core.Communication.Packets;

namespace Core.Communication.Messages
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