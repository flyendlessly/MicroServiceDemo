using Grpc.Core;
using System;

namespace GrpcServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Server
            //{
            //    Services = { RpcStreamingService.BindService(new RpcStreamingServiceImpl()) }, // 绑定我们的实现
            //    Ports = { new ServerPort("localhost", 23333, ServerCredentials.Insecure) }
            //}.Start();

            Console.ReadKey();
        }
    }
}
