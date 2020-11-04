using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.IServices;
using Application.ViewModels;
using EasyNetQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderApi.Interface;
using Refit;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly IProductAPI productAPI;
        private readonly IBus bus;
        public OrderController(IOrderService _orderService,IBus _bus,IProductAPI _productAPI)
        {
            orderService = _orderService;
            bus = _bus;
            productAPI = _productAPI;
        }

        // GET: api/Order
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var result = productAPI.GetProduct().Result;
            return result;
        }

        // GET: api/Order/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var proInfo = productAPI.GetProduct(id).Result;
            var item = new OrderMessage
            {
                id = 1,
                orderId = "alibaba_20200723161759000_0001",
                createOrderTime = DateTime.Now,
                state = "未支付",
                total = 123.59M,
                productInfos = proInfo,
                userId = 10001
            };
            return Ok(item);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]OrderDto dto)
        {
            var message = new OrderMessage
            {
                id = 1,
                orderId = dto.orderId,
                createOrderTime = DateTime.Now,
                state = "未支付",
                total = 123.59M,
                userId = 10001
            };

            //发订单信息到消息队列
            //bus.Publish(message);
            await bus.PublishAsync(message, "Topic.EmailServer");
            return Ok("order insert success");
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] OrderMessage value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
