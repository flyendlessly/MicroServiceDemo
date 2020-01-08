using Grpc.Core;
using GrpcProtocol;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static void Main(string[] args)
        {
            HealthCheckRequest healthCheckRequest = new HealthCheckRequest()
            {
                Service = "1"
            };
            var result = HealthCheckServiceClient.HealthCheck(healthCheckRequest).Result;

            Console.WriteLine($"grpc Client Call");

            Console.WriteLine("任意键退出...");
            Console.ReadKey();
        }
    }

    public static class HealthCheckServiceClient
    {
        private static Channel _channel;
        private static Health.HealthClient _client;

        static HealthCheckServiceClient()
        {
            _channel = new Channel("127.0.0.1:23333", ChannelCredentials.Insecure);
            _client = new Health.HealthClient(_channel);
        }

        public static Task<HealthCheckResponse> HealthCheck(HealthCheckRequest request)
        {
            return _client.CheckAsync(request).ResponseAsync;
        }
    }
}
