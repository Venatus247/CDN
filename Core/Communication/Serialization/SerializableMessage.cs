using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core.Communication.Serialization
{
    public abstract class SerializableMessage<T> where T : SerializableMessage<T>
    {
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