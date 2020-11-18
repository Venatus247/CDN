using System;
using Core.Communication.Packets;
using Core.Data.Cdn;

namespace Core.Communication.Messages.Authentication
{
    [Serializable]
    public class CdnAuthMessage : SerializedPacket<CdnAuthMessage>
    {
        
        public CdnReference CdnReference { get; set; }
        
        public CdnAuthMessage() : base((int) PacketCodes.CdnAuth)
        {
        }
        
    }
}