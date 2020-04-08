using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.MiddleWare;
using Class1.Model;
using IoC;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LoginApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory logger)
        {
            //依赖注入
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();//添加基于内存的缓存支持

            //防止CSRF
            services.AddAntiforgery(options => {
                options.Cookie.Name = "AntiForgery";
                options.Cookie.Domain = "localhost";
                options.Cookie.Path = "/";
                options.FormFieldName = "AntiForgery";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
            //CORS
            //services.AddCors(options => {
            //    options.AddPolicy("CorsPolicy", builder => 
            //    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            //});

            // .NET Native DI Abstraction
            RegisterServices(services);

            #region 认证
            #region 添加认证Cookie信息
            //Cookie认证属于Form认证，并不属于HTTP标准验证。
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            //     {
            //         o.Cookie.Name = "_AdminTicketCookie";
            //         o.LoginPath = new PathString("/api/Login");
            //     });

            //openid
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //})
            //.AddCookie()
            //.AddOpenIdConnect(o =>
            //{
            //    o.ClientId = "server.hybrid";
            //    o.ClientSecret = "secret";
            //    o.Authority = "https://demo.identityserver.io/";
            //    o.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            //});
            #endregion

            #region JwtBearer方式身份认证
            //services.AddAuthentication(options=> {
            //    options.DefaultAuthenticateScheme = "JwtBearer";
            //    options.DefaultChallengeScheme = "JwtBearer";
            //}).AddJwtBearer("JwtBearer",
            //(jwtBearerOptions) =>
            //{
            //    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("this is my first jwt token demo")),//秘钥
            //        ValidateIssuer = true,
            //        ValidIssuer = @"https://localhost:44359",
            //        ValidateAudience = true,
            //        ValidAudience = "api",
            //        ValidateLifetime = true,
            //        ClockSkew = TimeSpan.FromMinutes(5)
            //    };
            //});
            //services.AddAuthentication("Bearer").AddIdentityServerAuthentication()
            #endregion

            #region OAuth2
            //services.AddAuthentication(options =>k
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = OAuthDefaults.DisplayName;
            //})
            //.AddCookie()
            //.AddOAuth(OAuthDefaults.DisplayName, options =>
            //{
            //    options.ClientId = "oauth.code";
            //    options.ClientSecret = "secret";
            //    options.AuthorizationEndpoint = "https://localhost:44359/connect/authorize";
            //    options.TokenEndpoint = "https://localhost:44359/connect/token";
            //    options.CallbackPath = "/signin-oauth";
            //    options.Scope.Add("openid");
            //    options.Scope.Add("profile");
            //    options.Scope.Add("email");
            //    options.SaveTokens = true;
            //    // 事件执行顺序 ：
            //    // 1.创建Ticket之前触发
            //    options.Events.OnCreatingTicket = context => Task.CompletedTask;
            //    // 2.创建Ticket失败时触发
            //    options.Events.OnRemoteFailure = context => Task.CompletedTask;
            //    // 3.Ticket接收完成之后触发
            //    options.Events.OnTicketReceived = context => Task.CompletedTask;
            //    // 4.Challenge时触发，默认跳转到OAuth服务器
            //    // options.Events.OnRedirectToAuthorizationEndpoint = context => context.Response.Redirect(context.RedirectUri);
            //});
            #endregion
            #endregion

            #region 授权
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("System", policy => policy.RequireClaim("SystemType").Build());
            //    options.AddPolicy("Client", policy => policy.RequireClaim("ClientType").Build());
            //    options.AddPolicy("Admin", policy => policy.RequireClaim("AdminType").Build());
            //});
            #endregion


            //IdentityServer4
            //InMemoryConfiguration.Configuration = this.Configuration;
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential() //开发时使用的签名 filename: "tmpKey.rsa"
            //    .AddTestUsers(InMemoryConfiguration.GetUsers().ToList())
            //    .AddInMemoryClients(InMemoryConfiguration.GetClients())
            //    .AddInMemoryApiResources(InMemoryConfiguration.GetApiResources())
            //    .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentity());

            //swagger
            services.AddSwaggerGen(options=>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "EvenMC:登录模块服务",
                    Version="v1",
                    Description="用户身份验证、授权、token获取",
                    Contact=new OpenApiContact()
                    {
                        Name="Even Wang", Email="904044929@qq.com",Url= new Uri("https://blog.csdn.net/wyf1554624584")
                    },
                });
                options.DocInclusionPredicate((docName, description) => true);
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                //添加注释功能
                options.IncludeXmlComments(Path.Combine(basePath, "LoginApi.xml"),true);
                options.IncludeXmlComments(Path.Combine(basePath, "Application.xml"),true);
                //手动高亮
                //添加header验证信息
                //options.OperationFilter<SwaggerHeader>();
                var bearerScheme = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                };
                //添加一个必须的全局安全信息，[待证实]和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    { bearerScheme, new List<string>()}
                });
                options.AddSecurityDefinition("Bearer", bearerScheme);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//设置时间格式
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
        {
            var log = logger.CreateLogger("LoginMain");
            //日志测试
            //app.Run(context => {
            //    log.LogWarning("this is a test log");
            //    return context.Response.WriteAsync("Hello Login. Take a look at your terminal to see the logging messages.");
            //});

            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //app.UseAuthentication(); //添加认证中间件
            //app.UseAuthorize();
            //处理HTTP:401异常
            app.UseStatusCodePages(new StatusCodePagesOptions()
            {
                HandleAsync = (context) =>
                {
                    if(context.HttpContext.Response.StatusCode == 401)
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(context.HttpContext.Response.Body))
                        {
                            sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
                            {
                                status = 401,
                                message = "access denied",
                            }));
                        }
                    }
                    return System.Threading.Tasks.Task.Delay(0);
                }
            });

            //处理全局异常 
            app.UseCustomExceptionMiddleware();

            //app.UseIdentityServer(); //IdentityServer4
            app.UseHttpsRedirection();
            //app.UseMiddleware<AuthMiddleware>();//自定义Auth验证缓存中间件，Login控制器暂时没将token存入缓存
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "EvenMC:登录模块服务"); });
            app.UseMvc();
            //app.UseStaticFiles();//用于访问wwwroot下的文件 
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Adding dependencies from another layers (isolated from Presentation)
            NativeInjectorBootStrapper.RegisterServices(services);
        }
    }
}
