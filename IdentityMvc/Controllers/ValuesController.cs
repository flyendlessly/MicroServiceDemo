using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace IdentityMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
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

        [HttpGet("GetData")]
        public async Task<IActionResult> GetData()
        {
            var client = new HttpClient();
            //ids4服务端口
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:10000");
            if (disco.IsError)
                return new JsonResult(new { err = disco.Error });
            var token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                //获取Token的地址
                Address = disco.TokenEndpoint,
                //客户端Id
                ClientId = "apiClientCd",
                //客户端密码
                ClientSecret = "apiSecret",
                //要访问的api资源
                Scope = "secretapi"
            });
            if (token.IsError)
                return new JsonResult(new { err = token.Error });
            client.SetBearerToken(token.AccessToken);
            //资源api服务端口
            string data = await client.GetStringAsync("https://localhost:10001/api/identity");
            JArray json = JArray.Parse(data);
            return new JsonResult(json);
        }
    }
}
