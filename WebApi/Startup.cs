using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using DapperExtensions.Mapper;
using DapperExtensions;
using MySql.Data.MySqlClient;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Linq;
using OrderApi.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using EasyNetQ;
using Application.IServices;
using Application.Services;

namespace OrderApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory logger)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加验证器
            //services.AddSingleton<IValidator<Orders>, OrderValidation>();

            //RabbitMQ:EventBus (通过主题发布)
            services.AddSingleton(RabbitHutch.CreateBus(Configuration["RabbitMQ:Dev"]));
            // Ioc&Services
            services.AddScoped<IOrderService, OrderService>();

            //jwt 令牌认证
            services.AddAuthentication("Bearer")
                //.AddJwtBearer[//options.Audience = "secretapi";//权限标识  ]
                .AddIdentityServerAuthentication( options =>
            {
                options.Authority = "http://localhost:10000"; //ids4认证地址[LoginAPI:ids4示例]
                options.RequireHttpsMetadata = false;//是否必需HTTPS
                options.ApiName = "secretapi";
                //options.SupportedTokens = SupportedTokens.Both;
            });


            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            //mvc + 验证
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation();

            // override modelstate
            services.Configure<ApiBehaviorOptions>(
                options => {
                    options.InvalidModelStateResponseFactory = (context) => {
                        var errors = context.ModelState 
                        .Values 
                        .SelectMany(x => x.Errors .Select(p => p.ErrorMessage)) .ToList();
                        var result = new { Code = "00009", Message = "验证错误（Even）", Errors = errors };
                        return new BadRequestObjectResult(result);
                    };
                }
            );

            //services.AddDapperDataBase(ESqlDialect.MySQL, () => new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")), true,
            // typeof(PluralizedAutoClassMapper<>), new[] { typeof(Services.DASCustomClassMapper.BaseUserClassMapper).Assembly });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();//鉴权[UseMvc放在前面就不起作用]

            //app.UsePlatformAuthorication();

            app.UseMvc();
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
