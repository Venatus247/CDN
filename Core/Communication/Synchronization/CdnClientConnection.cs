using Core.Communication.Tcp.Server;
using Core.Data.Cdn;

namespace Core.Communication.Synchronization
{
    public class CdnClientConnection : TcpConnectedClient
    {
        public CdnReference CdnReference { get; set; }

        public bool IsAuthenticated()
        {
            //TODO think if token should be checked
            return CdnReference != null;
        }
        
    }
}