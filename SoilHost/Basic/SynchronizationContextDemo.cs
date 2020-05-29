using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SoilHost.Basic
{
    class SynchronizationContextDemo
    {

        static SynchronizationContext context;

        // 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
        //private  ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();
        /// <summary>
        /// 测试上下文同步
        /// SynchronizationContext
        /// </summary>
        /// <param name="args"></param>
         public static void main()
        {
            context = new SynchronizationContext();
            Console.WriteLine("主线程id：" + Thread.CurrentThread.ManagedThreadId);
            TestThread();
            Thread.Sleep(6000);
            Console.WriteLine("主线程执行");
            context.Send(EventMethod, "Send");
            context.Post(EventMethod, "Post");
            Console.WriteLine("主线程结束");
        }


        static void TestThread()
        {
            var thrd = new Thread(Start);
            thrd.Start();
        }

        static void Start()
        {
            Console.WriteLine("子线程id：" + Thread.CurrentThread.ManagedThreadId);
            context.Send(EventMethod, "子线程Send");
            context.Post(EventMethod, "子线程Post");
            Console.WriteLine("子线程结束");
        }

        static void EventMethod(object arg)
        {
            Console.WriteLine("CallBack::当前线程id：" + Thread.CurrentThread.ManagedThreadId + "     arg:" + (string)arg);
        }
    }
}
