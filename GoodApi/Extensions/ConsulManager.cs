using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodApi.Extensions
{
    public static class ConsulManager
    {

        public static void UseConsul(this IApplicationBuilder app, IConfiguration configuration, IConsulClient consul)
        {
            RegServer(configuration,consul);
        }
            

        private static void RegServer(IConfiguration configuration,IConsulClient consul)
        {
            //该服务注册Consul的组名
            string consulGroup = configuration["ConsulGroup"];
            string ip = configuration["ip"];
            int port = Convert.ToInt32(configuration["port"]);
            string serverId = $"{consulGroup}_{ip}_{port}";

            var regist = new AgentServiceRegistration
            {
                Address = ip,
                Port = port,
                Name = consulGroup,
                ID = serverId,
                Check = new AgentServiceCheck()//健康检查
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = $"http://{ip}:{port}/health",
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };
            consul.Agent.ServiceRegister(regist);
        }

    }


}
