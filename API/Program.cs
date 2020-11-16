using Core;
using Core.Communication.Synchronization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace API
{
    public class Program
    {
        public static CdnProxyServer CdnProxyServer;
        public static void Main(string[] args)
        {
            
            CdnProxyServer = new CdnProxyServer("127.0.0.1",4500);
            CdnProxyServer.Start();
            
            BackendServer.Instance.OnStartup();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}