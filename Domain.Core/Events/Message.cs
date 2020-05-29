using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Core.Events
{
    /// <summary>
    /// 请求消息-抽象类
    /// </summary>
    public abstract class Message : IRequest<bool>
    {
        public string MessageType { get; protected set; }
        public Guid AggregateId { get; protected set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}
