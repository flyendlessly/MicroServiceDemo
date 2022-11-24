using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MediatR;
using Domain.Core.Notifications;
using Domain.Core.Bus;
using Microsoft.AspNetCore.Authorization;
using Class1.Model;

namespace LoginApi.Controllers
{
    /// <summary>
    /// 登录模块
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ApiController
    {
        ILogger<LoginController> logger;
        IConfiguration Configuration;

        public LoginController(ILogger<LoginController> logger, IConfiguration Configuration,
                 INotificationHandler<DomainNotification> notifications, IMediatorHandler mediator
                 ) :base(notifications,mediator)
        {
            this.logger = logger;
            this.Configuration = Configuration;
            var aa = this.Configuration["demo"].ToString();
            var aa1 = this.Configuration.GetSection("demo").ToString();
        }

        // GET: api/Login
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //var properties = new AuthenticationProperties();
            //var ticket = new AuthenticationTicket(principal, properties, "myScheme");
            // 加密 序列化var token = Protect(ticket);
            logger.LogWarning("get方法调用");
            return new string[] { "value1", "value2" };
        }

        // GET: api/Login/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 用户认证登录 Cookie
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: api/Login
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] UserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyModelStateErrors();
                    //握手
                    //await HttpContext.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    //ModelState.AddModelError("", "用户名或密码错误！");
                    return Response(model);
                }

                //验证用户密码...(省略)
                model.Id = new Guid();
                var admin = model;

                if (admin != null)
                {
                    //注意此方法的第一个参数，必需与StartUp.cs中services.AddAuthentication的参数相同
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);//一定要声明AuthenticationScheme
                    identity.AddClaim(new Claim(ClaimTypes.Name, admin.Account));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()));
                    //identity.AddClaim(new Claim(ClaimTypes.Role, admin.Role));
                    //登入
                    await HttpContext.SignInAsync(identity.AuthenticationType,
                              new ClaimsPrincipal(identity),
                              new AuthenticationProperties
                              {
                                    IsPersistent = false,
                                    //RedirectUri = "/Home/Index",
                                    ExpiresUtc = new System.DateTimeOffset(dateTime: DateTime.Now.AddHours(6)),
                              });
                }
                //更新登陆时间 ...

                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest();
            }
            
        }

        /// <summary>
        /// 退出登录 Cookie
        /// </summary>
        /// <returns></returns>
        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                //删除Cookie
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 获取令牌(JWT)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Token")]
        public IActionResult Token([FromBody] UserViewModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    //验证用户密码...(省略)
                    var admin = model;
                    if (admin != null)
                    {
                        var claims = new Claim[]
                        {
                            //new Claim(ClaimTypes.Name, admin.Account),
                            //new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Sub,admin.Role),//Subject, subject签发给的受众，在Issuer范围内是唯一的
                            new Claim(JwtRegisteredClaimNames.Jti,model.Id.ToString()),//JWT ID,JWT的唯一标识
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64),//Issued At，JWT颁发的时间，采用标准unix时间，用于验证过期
                        };
                        var JwtSecurityKey = "this is my first jwt token demo";
                        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JwtSecurityKey));

                        var authTime = DateTime.Now;
                        var expires = DateTime.Now.AddDays(28);
                        //token信息
                        var token = new JwtSecurityToken(
                           issuer: @"jwt token demo",//jwt签发者,非必须
                           audience: "api", //jwt的接收该方，非必须
                           claims: claims,//声明集合
                           notBefore: authTime,
                           expires: expires,//指定token的生命周期，unix时间戳格式,非必须
                           signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));//使用私钥进行签名加密
                        //生成Token,生成最后的JWT字符串
                        string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                        return Ok(new
                        {
                            access_token = jwtToken,
                            token_type = "jwtBreaer",
                            profile = new
                            {
                                sid = admin.Id,
                                name = admin.Account,
                                auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                                expires_at = new DateTimeOffset(expires).ToUnixTimeSeconds()
                            }
                        });

                        //可以将token信息存入缓存
                    }
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Unauthorized();
            }
        }

        /// <summary>
        /// 验证领牌
        /// </summary>
        /// <returns></returns>
        [HttpGet("TokenValidation")]
        [Authorize]
        //[Authorize(Policy = "Admin")]
        public ActionResult<IEnumerable<string>> TokenValidation()
        {
            return new string[] { "Token Validation Success" };
        }

        /// <summary>
        /// 解析JWT Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("TokenInfo")]
        public ActionResult<JwtInfo> TokenInfo(string token)
        {
            if (token is null)
                return null;

            string tokenStr = token.Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();

            var payload = handler.ReadJwtToken(tokenStr).Payload;

            var claims = payload.Claims;

            JwtInfo info = new JwtInfo()
            {
                Jti = claims.First(claim => claim.Type == "jti")?.Value,
                Sub = claims.First(claim => claim.Type == "sub")?.Value,
                Iat = claims.First(claim => claim.Type == "iat")?.Value
            };

            return info;
        }


        // PUT: api/Login/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
