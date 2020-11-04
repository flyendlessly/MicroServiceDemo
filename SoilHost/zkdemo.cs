using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using org.apache.zookeeper;
using org.apache.zookeeper.data;
using Newtonsoft.Json;
namespace SoilHost
{



    public class zkdemo
    {
        private class Singleton
        {
            private static zkdemo instance;
            static Singleton()
            {
                instance = new zkdemo();
            }

            public static zkdemo getInstance()
            {
                return instance;
            }
        }
        public static zkdemo getInstance()
        {
            return Singleton.getInstance();
        }

        #region 变量
        private CountdownEvent countdownEvent { get; set; } = new CountdownEvent(1);
        private CountdownEvent latch;
        private ZooKeeper zooKeeper { get; set; }
        #endregion


        /// <summary>
        /// 初始化客户端连接
        /// </summary>
        public void InitConnect()
        {
            try
            {
                //Zookeeper连接字符串，采用host:port格式，多个地址之间使用逗号（,）隔开
                //string connectionString = "192.168.209.133:2181,192.168.209.133:2181,192.168.209.133:2181";
                string connectionString = "192.168.10.53:2181";
                //会话超时时间,单位毫秒
                int sessionTimeOut = 10000;
                //异步监听
                var watcher = new MyWatcher("ConnectWatcher");
                //连接
                this.zooKeeper = new ZooKeeper(connectionString, sessionTimeOut, watcher);
                Thread.Sleep(1000);//停一秒，等待连接完成
                while (zooKeeper.getState() == ZooKeeper.States.CONNECTING)
                {
                    Console.WriteLine("等待连接完成...");
                    Thread.Sleep(1000);
                }

                var state = zooKeeper.getState();
                if (state != ZooKeeper.States.CONNECTED && state != ZooKeeper.States.CONNECTEDREADONLY)
                {
                    Console.WriteLine("连接失败：" + state);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接失败：" + ex.Message);
            }
           
        }


        /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public bool acquireDistributedLock(long productid)
        {
            string path = "/product-lock-" + productid;
            try
            {
                CreateZnode(path,"");
                return true;
            }
            catch (Exception e)
            {
                while (true)
                {
                    try
                    {
                        Stat stat = GetZnodeStat(path);
                        if(stat!=null)
                        {
                            this.latch.Reset(1);
                            this.latch.Wait(TimeSpan.FromSeconds(10));
                            this.latch = null;
                        }
                        CreateZnode(path,"");
                        return true;
                    }
                    catch (Exception ee)
                    {
                        continue;
                    }
                }
            }
        }


        public void releaseDistributedLock(long productid)
        {
            string path = "/product-lock-" + productid;
            try
            {
                zooKeeper.deleteAsync(path);
                Console.WriteLine("");
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 创建Znode节点
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        private void CreateZnode(string path,string content)
        {
            var data = Encoding.UTF8.GetBytes(content);
            List<ACL> acl = ZooDefs.Ids.OPEN_ACL_UNSAFE;//创建节点时的acl权限，也可以使用下面的自定义权限
            //List<ACL> acl = new List<ACL>() {
            //    new ACL((int)ZooDefs.Perms.READ, new Id("ip", "127.0.0.1")),
            //    new ACL((int)(ZooDefs.Perms.READ | ZooDefs.Perms.WRITE), new Id("auth", "id:pass"))
            //};
            
            CreateMode createMode = CreateMode.PERSISTENT;

            try
            {
                zooKeeper.createAsync(path, data, acl, createMode).Wait();
                //zooKeeper.createAsync("/mynode/test", data, acl, createMode).Wait();
                Console.WriteLine($"完成创建节点:{path}");

                //var exists = zooKeeper.existsAsync(path, new MyWatcher("ExistsWatcher")).GetAwaiter().GetResult();
                //if (exists == null)
                //{
                //    zooKeeper.createAsync(path, data, acl, createMode).Wait();
                //    //zooKeeper.createAsync("/mynode/test", data, acl, createMode).Wait();
                //    Console.WriteLine("完成创建节点");
                //}
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        /// <summary>
        /// 获取Znode的Stat
        /// </summary>
        /// <param name="path"></param>
        private Stat GetZnodeStat(string path)
        {
            Stat exists = zooKeeper.existsAsync("/mynode", new MyWatcher("ExistsWatcher")).GetAwaiter().GetResult();
            Console.WriteLine("节点是否存在：" + exists);
            return exists;
        }

        //移除节点
        private void DeleteZnode(string path)
        {
            zooKeeper.deleteAsync("/mynode").Wait();
            Console.WriteLine("移除节点");
        }



        public void Operation()
        {
            //获取节点数据
            {
                var dataResult = zooKeeper.getDataAsync("/mynode", new MyWatcher("GetWatcher")).GetAwaiter().GetResult();
                var value = Encoding.UTF8.GetString(dataResult.Data);
                Console.WriteLine("完成读取节点：" + value);
            }

            //设置节点数据
            {
                var data = Encoding.UTF8.GetBytes("hello world again");
                zooKeeper.setDataAsync("/mynode", data);
                Console.WriteLine("设置节点数据");
            }

            //重新获取节点数据
            {
                var dataResult = zooKeeper.getDataAsync("/mynode", new MyWatcher("GetWatcher")).GetAwaiter().GetResult();
                var value = Encoding.UTF8.GetString(dataResult.Data);
                Console.WriteLine("重新获取节点数据：" + value);
            }


            Console.WriteLine("完成");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// 监听器
    /// </summary>
    class MyWatcher : Watcher
    {
        public string Name { get; private set; }

        public MyWatcher(string name)
        {
            this.Name = name;
        }

        public override Task process(WatchedEvent @event)
        {
            var path = @event.getPath();
            var state = @event.getState();
            Console.WriteLine($"{Name} recieve: Path-{path}     State-{@event.getState()}    Type-{@event.get_Type()}");
            return Task.FromResult(0);
        }
    }
}
