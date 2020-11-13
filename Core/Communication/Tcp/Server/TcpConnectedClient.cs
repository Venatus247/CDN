using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Commons;
using Core.Communication.Packets;
using Core.Controller;

namespace Core.Communication.Tcp.Server
{
    public class TcpConnectedClient : IIdentified
    {
        public const int DataBufferSize = 4096;
        
        public long Id { get; set; }

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private byte[] _receivedBuffer;

        private Packet _receivedPacket = new Packet();

        public delegate void PacketHandler(Packet packet);
        private Dictionary<int, PacketHandler> _packetHandlers = new Dictionary<int, PacketHandler>();
        
        public void Prepare(long id, TcpClient tcpClient)
        {
            Id = id;
            _tcpClient = tcpClient;
            
            _tcpClient.ReceiveBufferSize = DataBufferSize;
            _tcpClient.SendBufferSize = DataBufferSize;

            _stream = _tcpClient.GetStream();
            
            _receivedBuffer = new byte[DataBufferSize];

            _stream.BeginRead(_receivedBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }
        
        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var bytesLength = _stream.EndRead(result);
                if(bytesLength <= 0)
                    return;

                var data = new byte[bytesLength];
                Array.Copy(_receivedBuffer, data, bytesLength);
                
                _receivedPacket.Reset(HandleByteData(data));
                
                _stream.BeginRead(_receivedBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Logger.Debug($"Error while receiving data from {_tcpClient.Client.RemoteEndPoint}");
                //Console.WriteLine(e);
                //throw;
            }
        }

        private bool HandleByteData(byte[] data)
        {
            _receivedPacket.SetBytes(data);

            if (_receivedPacket.UnreadLength() < 4)
                return false;

            var packetLength = _receivedPacket.ReadInt();

            if (packetLength <= 0)
                return true;

            while (packetLength > 0 && packetLength <= _receivedPacket.UnreadLength())
            {
                var packetBytes = _receivedPacket.ReadBytes(packetLength);
                using (var packet = new Packet(packetBytes))
                {
                    var packetId = packet.ReadInt();
                    _packetHandlers[packetId](packet);
                }

                packetLength = 0;
                if (_receivedPacket.UnreadLength() < 4) continue;
                
                packetLength = _receivedPacket.ReadInt();
                if (packetLength <= 0)
                    return true;

            }

            return packetLength <= 1;
        }

        public void Invoke(ServerPackets identifier, PacketHandler handler)
        {
            _packetHandlers.Add((int)identifier, handler);
        }
        
    }
}