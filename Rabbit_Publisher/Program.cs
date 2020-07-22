using EasyNetQ;
using PublicClass.RabbitModel;
using System;

namespace Rabbit_Publisher
{
    class Program
    {
        public static void Main(string[] args)
        {
            var connStr = "host=127.0.0.1;virtualHost=wangyunfei;username=admin;password=admin";

            using (var bus = RabbitHutch.CreateBus(connStr))
            {
                var input = "";
                Console.WriteLine("Please enter a message. 'Quit' to quit.");
                while ((input = Console.ReadLine()) != "Quit")
                {
                    bus.Publish(new TextMessage
                    {
                        Text = input
                    });
                }
            }
        }
    }
}
