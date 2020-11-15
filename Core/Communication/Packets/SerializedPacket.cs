using System;
using Core.Communication.Serialization;

namespace Core.Communication.Packets
{
    [Serializable]
    public class SerializedPacket : ISerializable<SerializedPacket>
    {

        private ISerializable<SerializedPacket> _serializable;
        
        public SerializedPacket()
        {
            _serializable = this;
        }
        
        public Packet ToPacket()
        {
            return new Packet(_serializable.Serialize());
        }
        
    }
}