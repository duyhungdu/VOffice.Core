using System.Collections.Generic;
using System.Linq;
using VOffice.Model;
using VOffice.Repository.Infrastructure;

namespace VOffice.Repository
{
    public partial class EventLeaderRepository : BaseRepository<LeaderEvent>
    {
        public EventLeaderRepository()
        {
        }

        public List<LeaderEvent> GetEventUserNotifyByEventId(int eventId)
        {
            var listLeaderEvent = _entities.SPGetLeaderEventsByEventId(eventId).ToList();
            var result = new List<LeaderEvent>();
            foreach (var item in listLeaderEvent)
            {
                result.Add(new LeaderEvent
                {
                    CreatedBy = item.CreatedBy,
                    CreatedOn = item.CreatedOn,
                    EventId = item.EventId,
                    LeaderId = item.LeaderId                
                });
            }
            return result;
        }
    }
}