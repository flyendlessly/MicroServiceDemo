using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.MiddleWare;
using Class1.Model;
using IoC;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            //添加基于内存的缓存支持
            services.AddMemoryCache();

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

            #region 认证
            #region 添加认证Cookie信息
            //1.Cookie认证属于Form认证，并不属于HTTP标准验证。
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            //     {
            //         o.Cookie.Name = "_AdminTicketCookie";
            //         o.LoginPath = new PathString("/api/Login");
            //     });

            //2.Cookie认证,自定义事件 CustomCookieAuthenticationEvents
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,o =>
            //    {
            //        o.Cookie.Name = "_AdminTicketCookie";
            //        o.LoginPath = new PathString("/api/Login");
            //        o.EventsType = typeof(CustomCookieAuthenticationEvents);
            //    });
            //services.AddScoped<CustomCookieAuthenticationEvents>();

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
            //1.JwtBearer JWT Token
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer"; //JwtBearerDefaults.AuthenticationScheme
                options.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer",
                (jwtBearerOptions) =>
                {
                    //jwtBearerOptions.RequireHttpsMetadata = false; //获取或设置元数据地址或权限是否需要HTTPS，默认为true
                    //jwtBearerOptions.SaveToken = false; //是否将信息存储在token中
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("this is my first jwt token demo")),//秘钥
                        ValidateIssuer = true, //如果设置为True,JWT Token生成的时候，ValidIssuer必须和这里设置的一致。
                        ValidIssuer = @"jwt token demo",
                        ValidateAudience = true, //Audience 接收方
                        ValidAudience = "api",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5),
                        
                    };
                    //jwtBearerOptions.Events = new JwtBearerEvents()
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        context.Token = context.Request.Query["access_token"];
                    //        return Task.CompletedTask;
                    //    }
                    //};
                }
            );
            //services.AddAuthentication("Bearer").AddIdentityServerAuthentication();
            #endregion

            #region OAuth2
            //services.AddAuthentication(options =>
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
            //    options.AuthorizationEndpoint = "https://localhost:44359/api/Login/Token";
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
            //    //options.Events.OnRedirectToAuthorizationEndpoint = context => context.Response.Redirect(context.RedirectUri);
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

            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //                    // register your IdentityServer with Google at https://console.developers.google.com
            //                    // enable the Google+ API
            //                    // set the redirect URI to http://localhost:5000/signin-google
            //                    options.ClientId = "copy client ID from Google here";
            //        options.ClientSecret = "copy client secret from Google here";
            //    });
            #endregion

            //IdentityServer4配置
            //InMemoryConfiguration.Configuration = this.Configuration;
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential() //开发时使用的签名 filename: "tmpKey.rsa"
            //    //.AddTestUsers(InMemoryConfiguration.GetUsers().ToList())
            //    //添加客户端：用于访问被保护的Api客户端
            //    .AddInMemoryClients(InMemoryConfiguration.GetClients())
            //    //API访问授权资源：受保护的Api资源
            //    .AddInMemoryApiResources(InMemoryConfiguration.GetApiResources())
            //    //添加用户
            //    .AddTestUsers(InMemoryConfiguration.GetUsers())
            //    //身份信息授权资源：允许哪些用户访问
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

            // Adding MediatR for Domain Events and Notifications
            services.AddMediatR(typeof(Startup));

            // .NET Native DI Abstraction
            RegisterServices(services);
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

            //IdentityServer4
            //app.UseIdentityServer();

            app.UseHttpsRedirection();
            
            //app.UseCookiePolicy(new CookiePolicyOptions() { }); //用于设置应用的 cookie的兼容性。
            app.UseAuthentication(); //添加认证中间件 鉴权，检测有没有登录，登录的是谁，赋值给User

            //在3.0之后微软明确的把授权功能提取到了Authorization中间件里，所以我们需要在UseAuthentication之后再次UseAuthorization。
            //否则，当你使用授权功能比如使用[Authorize]属性的时候系统就会报错。
            //app.UseAuthorization(); //就是授权，检测权限，在.net 2.1中是没有UseAuthorization方法的
            //app.UseAuthorize();

            

            //app.UseMiddleware<AuthMiddleware>();//自定义Auth验证缓存中间件，Login控制器暂时没将token存入缓存
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "EvenMC:登录模块服务"); });

            app.UseMvc();
            app.UseStaticFiles();//用于访问wwwroot下的文件 
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Adding dependencies from another layers (isolated from Presentation)
            NativeInjectorBootStrapper.RegisterServices(services);
        }
    }
}
