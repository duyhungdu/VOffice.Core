using System.Linq;
using VOffice.Model;
using VOffice.Repository.Infrastructure;

namespace VOffice.Repository
{
    public class EventGoogleEventRepository : BaseRepository<EventGoogleEvent>
    {
        public EventGoogleEventRepository()
        {
        }
        public EventGoogleEvent GetByEventId(int id)
        {
            return this.FindBy(x => x.EventId == id).FirstOrDefault();
        }
    }
}