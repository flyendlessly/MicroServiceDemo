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
    }
}
