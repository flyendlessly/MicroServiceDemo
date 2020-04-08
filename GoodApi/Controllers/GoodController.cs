using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Class1.Model;
using GoodApi.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GoodApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodController : ControllerBase
    {
        public GoodController()
        {

        }
        // GET: api/Good
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Good/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult<string> Get(int id)
        {
            var item = new Goods
            {
                Id = id,
                Content = $"{id}的关联的商品明细",
            };
            return JsonConvert.SerializeObject(item);
        }


        // POST: api/Good
        [HttpPost]
        //[ModelValidation]
        public IActionResult Post([FromBody] Goods good)
        {
            try
            {
                //if (good.IsObjectNull()) { return BadRequest("good object is null"); }
                //if (!ModelState.IsValid) { return BadRequest("Invalid model object"); }
                //_repository.Owner.CreateOwner(owner);
                //return CreatedAtRoute("OwnerById", new { id = owner.Id }, owner)
                return Ok("as");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Good/5
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
