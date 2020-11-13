using System.Net;
using Core.Communication.Tcp.Server;

namespace Core.Communication.Synchronization
{
    public class CdnProxyServer : BasicTcpServer<TcpConnectedClient>
    {
        public CdnProxyServer(string address, int port) : base(IPAddress.Parse(address), port)
        {
        }
    }
}