using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Model
{
    public class ComplexEvent : Event
    {
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public virtual ICollection<EventUserNotify> EventUserNotifys { get; set; }
        public virtual ICollection<LeaderEvent> LeaderEvents { get; set; }

        public EventGoogleEvent GoogleEvent { get; set; }
    }
    public class CustomEvent
    {
        public int EventId { get; set; }
        public string CreatedBy { get; set; }
        public CustomEvent(int _eventId, string _createdBy)
        {
            this.EventId = _eventId;
            this.CreatedBy = _createdBy;
        }
    }
    public class CustomEventDeferred
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
        public bool Canceled { get; set; }
    }
    public class CustomEventAccepted
    {
        public bool Accepted { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
    public class CustomEventLeaderOfWeek
    {
        public string LeaderFullName { get; set; }
        public string Position { get; set; }
        public string LeaderId { get; set; }
        public virtual ICollection<EventOfDay> ContentMornings { get; set; }
        public virtual ICollection<EventOfDay> ContentAfternoons { get; set; }
    }
    public class CustomDayEventOfWeek
    {
        public string Title { get; set; }
        public int Week { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public virtual ICollection<CustomEventLeaderOfWeek> EventLeaders { get; set; }
    }
    public class CustomEventOfWeek : Event
    {
        public string DayOfWeek { get; set; }        
        public string Title { get; set; }
        public string Week { get; set; }
        public string FullName { get; set; }
        public string ImportantJobString { get; set; }
        public string NoteJobString { get; set; }
        public int ImportantJobId { get; set; }
        public int ImportantJobIdNormal { get; set; }        
        public string Position { get; set; }
        public bool Blue { get; set; }
        public virtual ICollection<EventOfDay> ContentMornings { get; set; }
        public virtual ICollection<EventOfDay> ContentAfternoons { get; set; }
        public EventGoogleEvent GoogleEvent { get; set; }
        public int STT { get; set; }
    }
    public class EventOfDay
    {
        public int Id { get; set; }
        public DateTime OccurDate { get; set; }
        public DateTime? StartTime { get; set; }
        public bool Morning { get; set; }
        public string Content { get; set; }
        public int? NotificationTimeSpan { get; set; }
        public string Title { get; set; }
        public string Participant { get; set; }
        public string Place { get; set; }
        public bool Public { get; set; }
        public bool Accepted { get; set; }
        public string AcceptedBy { get; set; }
        public DateTime? AcceptedOn { get; set; }
        public bool Canceled { get; set; }
        public string CanceledBy { get; set; }
        public DateTime? CanceledOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool Important { get; set; }
        public bool Blue { get; set; }
        public string FullName { get; set; }
        public EventGoogleEvent GoogleEvent { get; set; }
        public string Position { get; set; }
        public string ShortDepartment { get; set; }
    }
}
