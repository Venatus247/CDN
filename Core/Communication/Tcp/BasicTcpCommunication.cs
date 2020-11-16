using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading;
using Commons;
using Core.Communication.Packets;
using Core.Communication.States;

namespace Core.Communication.Tcp
{
    public abstract class BasicTcpCommunication
    {
        public const int DataBufferSize = 4096;
        
        protected TcpClient Client;

        protected NetworkStream Stream;
        protected byte[] ReceivedBuffer = new byte[DataBufferSize];
        
        protected readonly Packet ReceivedPacket = new Packet();
        
        protected readonly List<Packet> WaitingQueue = new List<Packet>();

        public delegate void PacketHandler(Packet packet);
        protected readonly Dictionary<int, PacketHandler> PacketHandlers = new Dictionary<int, PacketHandler>();

        public void Send(Packet packet)
        {
            try
            {
                if (Client == null || !Client.Connected)
                {
                    WaitingQueue.Add(packet);
                    return;
                }
                
                packet.WriteLength();
                
                Logger.Debug("Sending packet...");
                
                Stream.BeginWrite(packet.ToArray(), 0, packet.Length(), SendCallback, null);

            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            
        }

        public void SendFile([NotNull]ITcpFileState fileState)
        {
            try
            {
                if (Client == null || !Client.Connected)
                {
                    Logger.Debug($"Client: {Client != null}");
                    //todo handle file later
                    return;
                }

                var packet = fileState.NextPacket();
                packet.WriteLength();
                
                Logger.Debug("Sending file...");
                
                Stream.BeginWrite(packet.ToArray(), 0, packet.Length(), SendFileCallback, fileState);
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        private void SendFileCallback(IAsyncResult result)
        {
            try
            {
                var fileState = (ITcpFileState)result.AsyncState;
                if (fileState == null)
                {
                    throw new NullReferenceException("File state is null");
                }

                if (fileState.Finished())
                {
                    Logger.Debug("Finished sending file");
                    return;
                }
                    
                
                var packet = fileState.NextPacket();
                packet.WriteLength();
                
                Logger.Debug("Sending next file packet...");
                
                Thread.Sleep(10);
                
                Stream.BeginWrite(packet.ToArray(), 0, packet.Length(), SendFileCallback, fileState);

            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }
        
        protected void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var bytesLength = Stream.EndRead(result);
                if(bytesLength <= 0)
                    return;

                var data = new byte[bytesLength];
                Array.Copy(ReceivedBuffer, data, bytesLength);
                
                ReceivedPacket.Reset(HandleByteData(data));
                
                Stream.BeginRead(ReceivedBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Logger.Debug($"Error while receiving data from {Client.Client.RemoteEndPoint}");
                Logger.Exception(e);
            }
        }
        
        private bool HandleByteData(byte[] data)
        {
            try
            {
                ReceivedPacket.SetBytes(data);

                if (ReceivedPacket.UnreadLength() < 4)
                    return false;

                var packetLength = ReceivedPacket.ReadInt();

                Logger.Debug($"Packet Length: {packetLength}");
            
                if (packetLength <= 0)
                    return true;

                while (packetLength > 0 && packetLength <= ReceivedPacket.UnreadLength())
                {
                    var packetBytes = ReceivedPacket.ReadBytes(packetLength);
                    using (var packet = new Packet(packetBytes))
                    {
                        var packetId = packet.ReadInt();
                        Logger.Debug($"Packet ID: {packetId}");
                        PacketHandlers[packetId](packet);
                    }

                    packetLength = 0;
                    if (ReceivedPacket.UnreadLength() < 4) continue;
                
                    packetLength = ReceivedPacket.ReadInt();
                    if (packetLength <= 0)
                        return true;

                }

                return packetLength <= 1;
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                //TODO handle exception
            }

            return false;
        }
        
        public void Invoke(PacketCodes identifier, PacketHandler handler)
        {
            PacketHandlers.Add((int)identifier, handler);
        }
        
    }
}