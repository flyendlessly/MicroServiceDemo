using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Class1.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "这是一个商品value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var item = new Goods
            {
                Id = id,
                Content = $"{id}的关联的商品明细",
            };
            return JsonConvert.SerializeObject(item);
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

        [HttpGet("/health")]
        public IActionResult HeathCheck()
        {
            return Ok();
        }
    }
}
