using Domain.Core.Events;
using System;

namespace Infra.CrossCutting.Data.EventSourcing
{
    public class SqlEventStore : IEventStore
    {
        //private readonly IEventStoreRepository _eventStoreRepository;
        //private readonly IUser _user;

        public SqlEventStore()
        {
            //_eventStoreRepository = eventStoreRepository;
            //_user = user;
        }

        public void Save<T>(T theEvent) where T : Event
        {
            //var serializedData = JsonConvert.SerializeObject(theEvent);

            //var storedEvent = new StoredEvent(
            //    theEvent,
            //    serializedData,
            //    _user.Name);

            //_eventStoreRepository.Store(storedEvent);
        }
    }
}
