using System.Net.Sockets;
using Commons;
using Core.Communication.Messages;
using Core.Communication.Packets;
using Core.Controller;

namespace Core.Communication.Tcp.Server
{
    public class TcpConnectedClient : BasicTcpCommunication, IIdentified
    {
        public long Id { get; set; }

        public TcpConnectedClient()
        {
            PacketHandlers.Add((int)PacketCodes.Ping, packet =>
            {
                var message = SerializedPacket<PingMessage>.Deserialize(packet.ReadBytes(packet.UnreadLength()));
                Logger.Debug($"Received message: {message.Message}");
            });
            PacketHandlers.Add((int) PacketCodes.FileHeader, packet =>
            {
                Logger.Debug("Received file header");
            });
            PacketHandlers.Add((int) PacketCodes.FilePart, packet =>
            {
                Logger.Debug("Received file part");
            });
        }
        
        public void Prepare(long id, TcpClient client)
        {
            Id = id;
            Client = client;
            
            Client.ReceiveBufferSize = DataBufferSize;
            Client.SendBufferSize = DataBufferSize;

            Stream = Client.GetStream();
            
            ReceivedBuffer = new byte[DataBufferSize];

            Stream.BeginRead(ReceivedBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }
        
    }
}