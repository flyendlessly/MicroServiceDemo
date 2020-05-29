using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Core.Events
{
    /// <summary>
    /// 请求事件-抽象类
    /// </summary>
    public abstract class Event : Message, INotification
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; private set; }

        protected Event()
        {
            Timestamp = DateTime.Now;
        }
    }
}
