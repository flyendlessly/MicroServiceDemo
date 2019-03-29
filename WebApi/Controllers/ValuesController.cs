using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Class1.Model;
using Domain.Validations;
using FluentValidation;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "这是一个订单value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var item = new Orders
            {
                Id = id,
                Content = $"{id}的订单明细",
            };
            return JsonConvert.SerializeObject(item);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] Orders value)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AddOrderValidation validationRules = new AddOrderValidation();
                    var result = validationRules.Validate(value);
                    validationRules.ValidateAndThrow(value);
                    if (!result.IsValid)
                    {
                        foreach (var failure in result.Errors)
                        {
                        }
                    }
                }

                return Ok(value);
            }
            catch
            {
                return BadRequest();
            }
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

        /// <summary>
        /// 心跳监测
        /// </summary>
        /// <returns></returns>
        [HttpGet("/health")]
        public IActionResult Heathle()
        {
            return Ok();
        }

        /// <summary>
        /// 熔断报警
        /// </summary>
        /// <returns></returns>
        [HttpPost("/api/notice")]
        public IActionResult Notice()
        {
            var bytes = new byte[10240];
            var i = Request.Body.ReadAsync(bytes, 0, bytes.Length);
            var content = System.Text.Encoding.UTF8.GetString(bytes).Trim('\0');
            SendEmail(content);
            return Ok();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="content"></param>
        void SendEmail(string content)
        {
            try
            {
                dynamic list = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                if (list != null && list.Count > 0)
                {
                    var emailBody = new StringBuilder("健康检查故障:\r\n");
                    foreach (var noticy in list)
                    {
                        emailBody.AppendLine($"--------------------------------------");
                        emailBody.AppendLine($"Node:{noticy.Node}");
                        emailBody.AppendLine($"Service ID:{noticy.ServiceID}");
                        emailBody.AppendLine($"Service Name:{noticy.ServiceName}");
                        emailBody.AppendLine($"Check ID:{noticy.CheckID}");
                        emailBody.AppendLine($"Check Name:{noticy.Name}");
                        emailBody.AppendLine($"Check Status:{noticy.Status}");
                        emailBody.AppendLine($"Check Output:{noticy.Output}");
                        emailBody.AppendLine($"--------------------------------------");
                    }
                    var message = new MimeMessage();
                    //这里只是是测试邮箱，请不要发垃圾邮件，谢谢
                    message.From.Add(new MailboxAddress("yunfeizhishang2", "yunfeizhishang2@163.com"));
                    message.To.Add(new MailboxAddress("904044929", "904044929@qq.com"));

                    message.Subject = string.Format("C#自动发送邮件测试 From" +
                        " geffzhang TO {0}", "904044929");
                    message.Body = new TextPart("plain") { Text = "不好意思，我在测试程序，刚才把QQ号写错了，Sorry！" };
                    using (var client = new SmtpClient())
                    {

                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect("smtp.163.com", 25, false);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate("yunfeizhishang2@163.com", "asd123");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
            }
            catch
            {

            }

        }
    }
}
