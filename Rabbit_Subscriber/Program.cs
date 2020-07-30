using Application.ViewModels;
using EasyNetQ;
using PublicClass.RabbitModel;
using System;
using Newtonsoft.Json;
using EasyNetQ.AutoSubscribe;
using System.Threading.Tasks;
using System.Reflection;

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
                //This is something you only should do ONCE, preferably on application start up.
                AutoSubscriber autoSubscriber = new AutoSubscriber(bus, "WYF");
                //autoSubscriber.Subscribe(Assembly.GetExecutingAssembly());

                //简单订阅方式
                //bus.Subscribe<OrderMessage>("my_test_subscriptionid", HandleTextMessage);

                //自定义消费者 ps:为什么用Assembly.GetExecutingAssembly()创建不到队列？
                autoSubscriber.SubscribeAsync(typeof(SendOrderEmailToUserConsumer).Assembly);

                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();
            }
        }

        public static void HandleTextMessage(OrderMessage textMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got message: {0}",JsonConvert.SerializeObject(textMessage));
            Console.ResetColor();
        }

    }

    /// <summary>
    /// 自定义邮件发送消费者
    /// 可以继承IConsume<T> or IConsumeAsync<T>接口，来生成消费者对象
    /// .net core web api 中可以使用app.UseSubscribe("服务名", Assembly.GetExecutingAssembly());引用
    /// 默认的AutoSubscriber将不带主题的绑定。
    /// </summary>
    public class SendOrderEmailToUserConsumer : IConsumeAsync<OrderMessage>
    {
        [ForTopic("Topic.EmailServer")] //没有加上ForTopic属性标签，它将有一个"#"路由Key
        [AutoSubscriberConsumer(SubscriptionId = "EmailService.SendOrderEmailToUser")]
        public Task ConsumeAsync(OrderMessage message)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine("Consume one message from RabbitMQ : {0}, I will send Email to User",
                JsonConvert.SerializeObject(message));
            System.Console.ResetColor();
            return Task.CompletedTask;
        }
    }

    public class SendOrderEmailToServerConsumer : IConsumeAsync<OrderMessage>
    {
        [AutoSubscriberConsumer(SubscriptionId = "EmailService.SendOrderEmailToServer")]
        public Task ConsumeAsync(OrderMessage message)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine("Consume one message from RabbitMQ : {0}, I will send Email to Server",
                JsonConvert.SerializeObject(message));
            System.Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
