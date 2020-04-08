using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections;
using SoilHost.Algorithm;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SoilHost
{
    class Program
    {
        static void Main(string[] args)
        {

            //本服务开放端口
            //int silePort = 11113;
            //主简仓网关端口
            //int gatewayPort = 30000;
            //主简仓开放端口    
            //int mainSiloPort = 11111;
            //IServiceCollection

            //RunHost.RunMainAsync().Wait();

            //dapper demo
            //var aa = demo();

            //Union_Find.start();

            demo2();
        }



        //private static List<record> demo()
        //{

        //    var constr="Server=192.168.100.26;Database=hexun_sync_v2; User Id=mysql; Password=123456;Convert Zero Datetime=True;Allow Zero Datetime=True;default command timeout=20000;allow zero datetime=no";

        //    using (IDbConnection connection = new MySqlConnection(constr))
        //    {
        //        return connection.Query<record>("select * from sync_record").ToList();
        //    }

        //}

        public class record
        {
            public int id { get; set; }
            public string table_name { get; set; }
            public DateTime sync_time { get; set; }
            public string operation { get; set; }
            public int count { get; set; }
        }

        class Foo
        {
            int _answer;
            bool _complete;
            void A()
            {
                _answer = 123;
                Thread.MemoryBarrier();    // Barrier 1
                _complete = true;
                Thread.MemoryBarrier();    // Barrier 2
            }
            void B()
            {
                Thread.MemoryBarrier();    // Barrier 3
                if (_complete)
                {
                    Thread.MemoryBarrier();       // Barrier 4
                    Console.WriteLine(_answer);
                }
            }
        }

        private static readonly Stopwatch Watch = new Stopwatch();
        public static void demo2()
        {
            Watch.Start();
            const string url1 = "https://www.csdn.net/";
            const string url2 = "https://blog.csdn.net/wyf1554624584";
            //两次调用 CountCharacters 方法（下载某网站内容，并统计字符的个数）
            Task<int> result1 = CountCharactersAsync(1, url1);
            Task<int> result2 = CountCharactersAsync(2, url2);
            //三次调用 ExtraOperation 方法（主要是通过拼接字符串达到耗时操作）
            for (var i = 0; i < 3; i++)
            {
                ExtraOperation(i + 1);
            }
            //控制台输出
            Console.WriteLine($"{url1} 的字符个数：{result1.Result}");
            Console.WriteLine($"{url2} 的字符个数：{result2.Result}");
            Console.Read();
        }

        /// <summary>
        /// 统计字符个数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private static async Task<int> CountCharactersAsync(int id, string address)
        {
            var wc = new WebClient();
            Console.WriteLine($"开始调用 id = {id}：{Watch.ElapsedMilliseconds} ms");
            var result = await wc.DownloadStringTaskAsync(address);
            Console.WriteLine($"调用完成 id = {id}：{Watch.ElapsedMilliseconds} ms");
            return result.Length;
        }

        /// <summary>
        /// 额外操作
        /// </summary>
        /// <param name="id"></param>
        private static void ExtraOperation(int id)
        {
            //这里是通过拼接字符串进行一些相对耗时的操作
            var s = "";
            for (var i = 0; i < 6000; i++)
            {
                s += i;
            }
            Console.WriteLine($"id = {id} 的 ExtraOperation 方法完成：{Watch.ElapsedMilliseconds} ms");
        }
    }
}
