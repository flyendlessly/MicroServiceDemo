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

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        ILogger<LoginController> logger;

        public LoginController(ILogger<LoginController> logger)
        {
            this.logger = logger;
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

        // POST: api/Login
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([FromBody] UserViewModel model)
        {
            try
            {
                //模型验证后
                if (ModelState.IsValid)
                {
                    //验证用户密码...(省略)
                    model.Id = new Guid();
                    var admin = model;
                    
                    if(admin!=null)
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
                }
                else
                {
                    //握手
                    await HttpContext.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    ModelState.AddModelError("", "用户名或密码错误！");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest();
            }
            
        }

        /// <summary>
        /// 退出登录
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
        /// 获取令牌
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
                    model.Id = new Guid();
                    var admin = model;

                    if (admin != null)
                    {
                        var claims = new Claim[]
                        {
                            new Claim(ClaimTypes.Name, admin.Account),
                            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                        };
                        var JwtSecurityKey = "this is my first jwt token demo";
                        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JwtSecurityKey));

                        var authTime = DateTime.Now;
                        var expires = DateTime.Now.AddDays(28);
                        //token信息
                        var token = new JwtSecurityToken(
                           issuer: @"https://localhost:44359",
                           audience: "api", 
                           claims: claims,
                           notBefore: authTime,
                           expires: expires,
                           signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
                        //生成Token
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
