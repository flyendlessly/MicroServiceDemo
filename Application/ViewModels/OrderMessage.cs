using EasyNetQ;
using System;

namespace Application.ViewModels
{
    [Queue("Qka.Client", ExchangeName = "Qka.Client")]
    public class OrderMessage
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal total { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 下单用户Id
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// 订单生成时间
        /// </summary>
        public DateTime createOrderTime { get; set; }
    }
}
