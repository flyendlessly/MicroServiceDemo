using Domain.Core.Events;
using System;
using FluentValidation.Results;

namespace Domain.Core.Commands
{
    /// <summary>
    /// 请求指令-抽象类
    /// </summary>
    public abstract class Command : Message
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// 验证结果
        /// </summary>
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }

        public abstract bool IsValid();
    }
}
