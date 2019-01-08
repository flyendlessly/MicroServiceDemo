using Class1.Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SoilHost
{
    public class RunHost
    {
        public static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await Start();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        public static async Task<ISiloHost> Start()
        {
            var builder = new Orleans.Hosting.SiloHostBuilder().UseLocalhostClustering()
                //.UseDevelopmentClustering(options=>options.PrimarySiloEndpoint= new IPEndPoint())
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloWorldApp";
                })
                //.Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());
            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
