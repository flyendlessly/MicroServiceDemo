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
using SoilHost.Basic;
using SoilHost.Basic.ConsoleApp1;
using Application.MiddleWare;
using System.Text;

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

            //SynchronizationContextDemo.main();
            var aa = Enum.IsDefined(typeof(TlevaltorCommandEnum), "0201");
            var aa2  = Convert.ToInt32("0201", 16)==0x0201;
            //TheadDemo.main3();
        }


        /// <summary>
        /// 梯控协议命令地址=基地址+命令
        /// </summary>
        public enum TlevaltorCommandEnum : int
        {
            /// <summary>
            /// 开机上报
            /// </summary>
            OpenUpload = 0x0100,

            /// <summary>
            /// 写控制楼层编号
            /// </summary>
            WriteFloorId = 0x0201,

            /// <summary>
            /// 读蓝牙名称
            /// </summary>
            ReadBluetoothName = 0x0202,

            /// <summary>
            /// 写蓝牙名称
            /// </summary>
            WriteBluetoothName = 0x0203,

            /// <summary>
            /// 读软硬件版本号
            /// </summary>
            ReadHardWareAndSoftWareVersion = 0x0204,

            /// <summary>
            /// 写服务器IP和端口号和登录方式
            /// </summary>
            WriteServerIPPortAndLoginMethod = 0x0205,

            /// <summary>
            /// 读服务器IP和端口号和登录方式
            /// </summary>
            ReadServerIPPortAndLoginMethod = 0x0206,

            /// <summary>
            /// 写服务器域名
            /// </summary>
            WriteServerDomain = 0x0207,

            /// <summary>
            /// 读服务器域名
            /// </summary>
            ReadServerDomain = 0x0208,

            /// <summary>
            /// 软重启
            /// </summary>
            SoftRestart = 0x0209,

            /// <summary>
            /// 读CCID号
            /// </summary>
            ReadCCIDNumber = 0x020A,

            /// <summary>
            /// 恢复出厂配置
            /// </summary>
            RecoveryFactorySetting = 0x020B,

            /// <summary>
            /// 心跳
            /// </summary>
            HearBeat = 0x0501,

            /// <summary>
            /// 触发上报
            /// </summary>
            TriggerUpload = 0x0600,

        }
    }
}
