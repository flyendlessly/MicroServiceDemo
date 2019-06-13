using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
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

namespace LoginApi
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
            services.AddMemoryCache();//添加基于内存的缓存支持

            //防止CSRF
            services.AddAntiforgery(options => {
                options.Cookie.Name = "AntiForgery";
                options.Cookie.Domain = "localhost";
                options.Cookie.Path = "/";
                options.FormFieldName = "AntiForgery";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            #region 添加认证Cookie信息
            //Cookie认证属于Form认证，并不属于HTTP标准验证。
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,o=>
            //    {
            //        o.Cookie.Name = "_AdminTicketCookie";
            //        o.LoginPath = new PathString("/api/Login");
            //    })

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

            //IdentityServer4
            InMemoryConfiguration.Configuration = this.Configuration;
            services.AddIdentityServer()
                .AddDeveloperSigningCredential() //开发时使用的签名
                .AddTestUsers(InMemoryConfiguration.GetUsers().ToList())
                .AddInMemoryClients(InMemoryConfiguration.GetClients())
                .AddInMemoryApiResources(InMemoryConfiguration.GetApiResources())
                .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentity());

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
        {
            var log = logger.CreateLogger("LoginMain");
            //日志测试
            app.Run(context => {
                log.LogWarning("this is a test log");
                return context.Response.WriteAsync("Hello Login. Take a look at your terminal to see the logging messages.");
            });

            
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
            //处理401异常
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

            app.UseIdentityServer(); //IdentityServer4

            app.UseHttpsRedirection();
            app.UseMvc();


        }
    }
}
