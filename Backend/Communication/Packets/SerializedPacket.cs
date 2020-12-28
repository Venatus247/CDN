using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Communication.Serialization;

namespace Communication.Packets
{
    [Serializable]
    public class SerializedPacket<T> : ISerializable<T> where T : SerializedPacket<T>, new()
    {

        public int PacketId { get; protected set; } = -1;
        
        public SerializedPacket()
        {
        }
        
        public SerializedPacket(int packetId)
        {
            PacketId = packetId;
        }
        
        public Packet ToPacket()
        {
            var packet = new Packet(PacketId, Serialize());
            return packet;
        }
     
        public byte[] Serialize()
        {
            using var memoryStream = new MemoryStream();
            var formatter =  new BinaryFormatter();
            formatter.Serialize(memoryStream, this);
            return memoryStream.ToArray();
        }

        public static T Deserialize(byte[] data)
        {
            using var memoryStream = new MemoryStream(data);
            return (new BinaryFormatter()).Deserialize(memoryStream) as T;
        }

        public static bool TryDeserialize(byte[] data, out T obj)
        {
            try
            {
                obj = Deserialize(data);
                return true;
            }
            catch (Exception e)
            {
                obj = null;
                return false;
            }
        }

        public static T TryDeserializeOrDefault(byte[] data)
        {
            TryDeserialize(data, out var obj);
            return obj;
        }
        
    }
}