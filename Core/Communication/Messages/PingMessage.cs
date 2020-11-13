using Core.Communication.Serialization;

namespace Core.Communication.Messages
{
    public class PingMessage : SerializableMessage<PingMessage>
    {
        
        public string Message { get; set; }
        
    }
}