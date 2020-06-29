using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodApi.Filter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using GoodApi.Extensions;
using Consul;

namespace GoodApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ModelValidationAttribute>();

            // Adding MediatR for Domain Events and Notifications
            services.AddMediatR(typeof(Startup));

            // Adding ConsulClient Object
            services.AddSingleton<IConsulClient>(c=>new ConsulClient(cc=> {
                cc.Address = new Uri("http://localhost:8500");
            }));

            services.AddAuthentication("Bearer").AddJwtBearer(r => {
                //认证地址
                r.Authority = "http://localhost:5000";
                //权限标识
                r.Audience = "secretapi";
                //是否必需HTTPS
                r.RequireHttpsMetadata = false;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConsulClient consul)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseConsul(Configuration,consul);
            app.UseMvc();
        }
    }
}
