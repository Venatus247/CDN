using System;
using Communication.Packets;

namespace Communication.Messages
{
    [Serializable]
    public class PingMessage : SerializedPacket<PingMessage>
    {
        
        public string Message { get; set; }

        public PingMessage() : base((int)PacketCodes.Ping)
        {
        }

        public PingMessage(string message) : base((int)PacketCodes.Ping)
        {
            Message = message;
        }
        
    }
}