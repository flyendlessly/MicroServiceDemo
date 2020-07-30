using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Dto
{
    public class OrderDto
    {
        [Required(ErrorMessage = "用户账户名不能为空")]
        public string orderId { get; set; }


    }
}
