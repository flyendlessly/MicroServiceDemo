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
            //TheadDemo.main3();

            //反射.
            //Emit.EmitDemo.Demo();

            string aa = "";
            RunCmd(@"start Bypass.exe", out aa);
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

        public static void RunCmd(string cmd, out string output)
        {
            cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (Process p = new Process())
            {
                p.StartInfo.FileName = @"E:\EvenWang\工具\Bypass\Bypass.exe";
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                p.Start();//启动程序

                //向cmd窗口写入命令
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                //获取cmd窗口的输出信息
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
            }
        }
    }
}
