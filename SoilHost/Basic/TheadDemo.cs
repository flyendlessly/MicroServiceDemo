using System;
using System.Collections.Generic;
using System.Text;

namespace SoilHost.Basic
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    namespace ConsoleApp1
    {
        class Node
        {
            public Node Left { get; set; }
            public Node Right { get; set; }
            public string Text { get; set; }
        }



        public class TheadDemo
        {
            #region 动态并行(TaskCreationOptions.AttachedToParent) 父任务等待所有子任务完成后 整个任务才算完成
            static Node GetNode()
            {
                Node root = new Node
                {
                    Left = new Node
                    {
                        Left = new Node
                        {
                            Text = "L-L"
                        },
                        Right = new Node
                        {
                            Text = "L-R"
                        },
                        Text = "L"
                    },
                    Right = new Node
                    {
                        Left = new Node
                        {
                            Text = "R-L"
                        },
                        Right = new Node
                        {
                            Text = "R-R"
                        },
                        Text = "R"
                    },
                    Text = "Root"
                };
                return root;
            }

            public static void main()
            {
                Node root = GetNode();
                DisplayTree(root);
            }

            static void DisplayTree(Node root)
            {
                var task = Task.Factory.StartNew(() => DisplayNode(root),
                                                CancellationToken.None,
                                                TaskCreationOptions.None,
                                                TaskScheduler.Default);
                task.Wait();
            }

            static void DisplayNode(Node current)
            {

                if (current.Left != null)
                    Task.Factory.StartNew(() => DisplayNode(current.Left),
                                                CancellationToken.None,
                                                TaskCreationOptions.AttachedToParent,
                                                TaskScheduler.Default);
                if (current.Right != null)
                    Task.Factory.StartNew(() => DisplayNode(current.Right),
                                                CancellationToken.None,
                                                TaskCreationOptions.AttachedToParent,
                                                TaskScheduler.Default);
                Console.WriteLine("当前节点的值为{0};处理的ThreadId={1}", current.Text, Thread.CurrentThread.ManagedThreadId);
            }
            #endregion

            #region 使用IProgress实现异步编程的进程通知
            static void DoProcessing(IProgress<int> progress)
            {
                for (int i = 0; i <= 100; ++i)
                {
                    Thread.Sleep(100);
                    if (progress != null)
                    {
                        progress.Report(i);
                    }
                }
            }

            static async Task Display()
            {
                //当前线程
                var progress = new Progress<int>(percent =>
                {
                    Console.Clear();
                    Console.Write("{0}%", percent);
                });
                //线程池线程
                await Task.Run(() => DoProcessing(progress));
                Console.WriteLine("");
                Console.WriteLine("结束");
            }

            public static void main2()
            {
                Task task = Display();
                task.Wait();
            }
            #endregion

            #region MyRegion
            public static void main3()
            {
                TaskCompletionSource<int> tcs1 = new TaskCompletionSource<int>();
                Task<int> t1 = tcs1.Task;

                // Start a background task that will complete tcs1.Task
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    tcs1.SetResult(15);
                    Console.WriteLine("第一个Factory" + Thread.CurrentThread.ManagedThreadId);
                });

                // The attempt to get the result of t1 blocks the current thread until the completion source gets signaled.
                // It should be a wait of ~1000 ms.
                Stopwatch sw = Stopwatch.StartNew();
                int result = t1.Result;
                Console.WriteLine("主1:" + Thread.CurrentThread.ManagedThreadId);
                sw.Stop();

                Console.WriteLine("(ElapsedTime={0}): t1.Result={1} (expected 15) ", sw.ElapsedMilliseconds, result);

                // ------------------------------------------------------------------

                // Alternatively, an exception can be manually set on a TaskCompletionSource.Task
                TaskCompletionSource<int> tcs2 = new TaskCompletionSource<int>();
                Task<int> t2 = tcs2.Task;

                // Start a background Task that will complete tcs2.Task with an exception
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    tcs2.SetException(new InvalidOperationException("SIMULATED EXCEPTION"));
                    Console.WriteLine("第二个Factory" + Thread.CurrentThread.ManagedThreadId);
                });

                // The attempt to get the result of t2 blocks the current thread until the completion source gets signaled with either a result or an exception.
                // In either case it should be a wait of ~1000 ms.
                sw = Stopwatch.StartNew();
                try
                {
                    result = t2.Result;

                    Console.WriteLine("t2.Result succeeded. THIS WAS NOT EXPECTED.");
                }
                catch (AggregateException e)
                {
                    Console.WriteLine("AggregateException:" + Thread.CurrentThread.ManagedThreadId);
                    Console.Write("(ElapsedTime={0}): ", sw.ElapsedMilliseconds);
                    Console.WriteLine("The following exceptions have been thrown by t2.Result: (THIS WAS EXPECTED)");
                    for (int j = 0; j < e.InnerExceptions.Count; j++)
                    {
                        Console.WriteLine("\n-------------------------------------------------\n{0}", e.InnerExceptions[j].ToString());
                    }

                }
                Console.WriteLine("主2:" + Thread.CurrentThread.ManagedThreadId);
                Console.ReadKey();
            }
            #endregion

        }


    }

    
}
