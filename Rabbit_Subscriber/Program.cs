using EasyNetQ;
using PublicClass.RabbitModel;
using System;

namespace Rabbit_Subscriber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connStr = "host=127.0.0.1;virtualHost=wangyunfei;username=admin;password=admin";

            /* 
             * 过IBus实例去订阅消息（这里是除非用户关闭程序否则一直处于监听状态），
             * 当发布者发布了指定类型的消息之后，这里就把它打印出来（红色字体显示）。
             */
            using (var bus = RabbitHutch.CreateBus(connStr))
            {

                bus.Subscribe<TextMessage>("my_test_subscriptionid", HandleTextMessage);

                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();
            }
        }

        public static void HandleTextMessage(TextMessage textMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got message: {0}", textMessage.Text);
            Console.ResetColor();
        }
    }
}
