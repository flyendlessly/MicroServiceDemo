using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Class1.Model
{
    public class Goods
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }

    public class Orders
    {
        [Display(Name = "订单编号")]
        public int Id { get; set; }

        [Display(Name ="订单内容")]
        [Required(ErrorMessage = "订单内容= =不能为空")]
        public string Content { get; set; }
    }
}
