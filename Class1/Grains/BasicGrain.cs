using Class1.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orleans;

namespace Class1.Grains
{
    public class HelloGrain : Grain, IHello
    {

        private readonly ILogger logger;

        public HelloGrain(ILogger<HelloGrain> logger)
        {
            this.logger = logger;
        }

        public Task<string> DelayedMsg(string hellomsg)
        {
            Thread.Sleep(3000);
            Console.WriteLine("{0}:延迟的消息---{1}", DateTime.Now.ToString("HH:mm:ss.fff"), hellomsg);
            return Task.FromResult<string>("延迟的done");
        }

        public Task<string> SayHello(string hellostr)
        {
            Console.WriteLine("{0}:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), hellostr);
            return Task.FromResult<string>("done");
        }
    }
}
