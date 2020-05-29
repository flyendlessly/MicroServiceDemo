using Domain.Core.Commands;
using Domain.Core.Events;
using System.Threading.Tasks;

namespace Domain.Core.Bus
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMediatorHandler
    {
        /// <summary>
        /// 发送指令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        Task SendCommand<T>(T command) where T : Command;

        /// <summary>
        /// 实现事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        Task RaiseEvent<T>(T @event) where T : Event;
    }
}
