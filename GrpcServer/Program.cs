using Grpc.Core;
using System;
using GrpcProtocol;
namespace GrpcServer
{
    class Program
    {
        private static Server _server;
        static void Main(string[] args)
        {
            _server = new Server
            {
                Services = { Health.BindService(new HealthCheckService()) }, // 绑定我们的实现
                Ports = { new ServerPort("localhost", 23333, ServerCredentials.Insecure) }
            };
            _server.Start();
            Console.WriteLine("grpc ServerListening On Port 23333");
            Console.WriteLine("任意键退出...");
            Console.ReadKey();
            _server?.ShutdownAsync().Wait();
        }
    }
}
