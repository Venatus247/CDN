using System;
using Communication.Packets;

namespace Communication.Messages
{
    [Serializable]
    public class CdnConnectedMessage : SerializedPacket<CdnConnectedMessage>
    {

        public string CdnId { get; set; }
        
        public CdnConnectedMessage() : base((int) PacketCodes.CdnConnected)
        {
        }

    }
}