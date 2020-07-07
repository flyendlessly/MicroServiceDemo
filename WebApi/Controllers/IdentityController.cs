using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class IdentityController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        /// <summary>
        /// 受保护的Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("demo")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public object GetUserClaims()
        {
            return User.Claims.Select(r => new { r.Type, r.Value });
        }

        /// <summary>
        /// 受保护的Api(Claim为role==admin)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("demo_admin")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles ="admin")]
        public object GetUserClaims2()
        {
            return User.Claims.Select(r => new { r.Type, r.Value });
        }
    }
}