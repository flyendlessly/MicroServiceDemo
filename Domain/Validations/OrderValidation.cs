using Class1.Model;
using FluentValidation;
using System;

namespace Domain
{
    /// <summary>
    /// Order验证
    /// </summary>
    public class OrderValidation : AbstractValidator<Orders> 
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderValidation()
        {
            RuleFor(c => c.Id).NotEmpty().WithMessage("订单Id不能为空哟");
        }

        protected void ValidateContent()
        {
            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("订单内容不能为空")
                .Length(2, 150).WithMessage("内容范围在（2~150）之间");
        }
    }
}
