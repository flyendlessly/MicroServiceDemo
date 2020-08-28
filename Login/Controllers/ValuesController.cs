using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.MiddleWare;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {

        }

        /// <summary>
        /// 身份认证
        /// </summary>
        /// <returns></returns>
        // GET api/values
        //[Authorize]
        [HttpGet]
        //[Authorize(Policy = "Admin")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "login", "login2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            if(id==99)
            {
                throw new ArgumentNullException();
            }
            LogDiagnostic.SendLog();

            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
