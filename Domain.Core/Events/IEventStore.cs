using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Core.Events
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventStore
    {
        void Save<T>(T theEvent) where T : Event;
    }
}
