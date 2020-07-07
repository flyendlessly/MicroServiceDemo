using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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


        [HttpGet("GetData")]
        public async Task<IActionResult> GetData(string type, string userName="", string password="")
        {
            type = type ?? "client";
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:10000");
            if (disco.IsError)
                return new JsonResult(new { err = disco.Error });
            TokenResponse token = null;
            switch (type)
            {
                case "client":
                    token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
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
                    break;
                case "password":
                    token = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
                    {
                        //获取Token的地址
                        Address = disco.TokenEndpoint,
                        //客户端Id
                        ClientId = "apiClientPassword",
                        //客户端密码
                        ClientSecret = "apiSecret",
                        //要访问的api资源
                        Scope = "secretapi",
                        UserName = userName,
                        Password = password
                    });
                    break;
            }
            if (token.IsError)
                return new JsonResult(new { err = token.Error });
            client.SetBearerToken(token.AccessToken);
            string data = await client.GetStringAsync("http://localhost:10001/api/Identity/demo");
            JArray json = JArray.Parse(data);
            return new JsonResult(json);
        }
    }
}
