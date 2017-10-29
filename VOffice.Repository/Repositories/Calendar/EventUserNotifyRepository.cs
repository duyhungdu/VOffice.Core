using System.Collections.Generic;
using System.Linq;
using VOffice.Model;
using VOffice.Repository.Infrastructure;

namespace VOffice.Repository
{
    public partial class EventUserNotifyRepository : BaseRepository<EventUserNotify>
    {
        public EventUserNotifyRepository()
        {
        }

        public List<EventUserNotify> GetEventUserNotifyByEventId(int eventId)
        {
            var listLeaderEvent = _entities.SPGetEventUserNotifyByEventId(eventId).ToList();
            var result = new List<EventUserNotify>();
            foreach (var item in listLeaderEvent)
            {
                result.Add(new EventUserNotify
                {
                    CreatedBy = item.CreatedBy,
                    CreatedOn = item.CreatedOn,
                    EventId = item.EventId,
                    UserId = item.UserId
                });
            }
            return result;
        }
    }
}