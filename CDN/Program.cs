using System.Net;
using Core;
using Core.Communication.Packets;
using Core.Communication.Tcp.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CDN
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cdnClient = new BasicTcpClient(IPAddress.Parse("127.0.0.1"), 4500);
            cdnClient.Start();
            
            var packet = new Packet((int)ClientPackets.Ping);
            packet.Write("Hello there!");
            cdnClient.Send(packet);
            
            BackendServer.Instance.OnStartup();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}