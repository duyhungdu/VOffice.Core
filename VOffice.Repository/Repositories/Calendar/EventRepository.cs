using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VOffice.Model;
using VOffice.Repository.Infrastructure;

namespace VOffice.Repository
{
    public partial class EventRepository : BaseRepository<Event>
    {
        public EventRepository()
        {
        }

        public List<SPGetEventOfDepartment_Result> GetEventOfDepartment(int type, int departmentId, string userId, bool morning, DateTime date)
        {
            List<SPGetEventOfDepartment_Result> result = new List<SPGetEventOfDepartment_Result>();
            result = _entities.SPGetEventOfDepartment(type, departmentId, userId, morning, date).ToList();
            return result;
        }
        public List<CustomDayEventOfWeek> GetDeparmentLeaderEventOfWeek(int departmentId, DateTime date)
        {
            CultureInfo calendarInfo = new CultureInfo("vi-VN");
            Calendar calendar = calendarInfo.Calendar;
            CalendarWeekRule calWeekRule = calendarInfo.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dayOfweek = calendarInfo.DateTimeFormat.FirstDayOfWeek;
            StaffRepository staffRepostiory = new StaffRepository();
            DateTime fromDate = date.GetFirstDayOfWeek();
            DateTime toDate = fromDate.AddDays(6);
            int week = calendar.GetWeekOfYear(date, calWeekRule, dayOfweek) - 1;

            if (week == 0) week = calendar.GetWeekOfYear(new DateTime(fromDate.Year, 12, 31), calWeekRule, dayOfweek) - 1;
            List<SPGetDepartmentLeaderEventOfWeek_Result> result = new List<SPGetDepartmentLeaderEventOfWeek_Result>();
            result = _entities.SPGetDepartmentLeaderEventOfWeek(departmentId, fromDate, toDate).ToList();
            List<CustomDayEventOfWeek> ListCustomEventOfWeek = new List<CustomDayEventOfWeek>();

            for (int i = 0; i <= 6; i++)
            {
                CustomDayEventOfWeek customDayEventOfWeek = new CustomDayEventOfWeek();
                customDayEventOfWeek.Title = string.Format("{0}, {1:dd/MM/yyyy}", Util.GetDayOfWeek(i), fromDate.AddDays(i).Date);
                customDayEventOfWeek.Week = week;
                customDayEventOfWeek.FromDate = fromDate;
                customDayEventOfWeek.ToDate = toDate;
                customDayEventOfWeek.EventLeaders = new List<CustomEventLeaderOfWeek>();

                var oevents = result.ToList()
                                .GroupBy(x => new { x.FullName, x.ShortPosition, x.LeaderId })
                                .Select(y => new
                                {
                                    FullName = y.Key.FullName,
                                    SortPosition = y.Key.ShortPosition,
                                    LeaderId = y.Key.LeaderId,
                                }).ToList();
                foreach (var item in oevents)
                {
                    var leader = new CustomEventLeaderOfWeek();
                    leader.LeaderFullName = item.FullName;
                    leader.Position = item.SortPosition;
                    leader.LeaderId = item.LeaderId;
                    leader.ContentAfternoons = new List<EventOfDay>();
                    leader.ContentMornings = new List<EventOfDay>();
                    var listExistEvent = result.Where(x => x.OccurDate.HasValue);
                    var eventleaders = listExistEvent.Where(x => x.LeaderId == item.LeaderId &&
                                                            x.OccurDate.Value.Date == fromDate.AddDays(i).Date).ToList();
                    if (eventleaders.Count > 0)
                    {
                        foreach (var e in eventleaders)
                        {
                            var objevent = new EventOfDay();
                            if (e.Morning.Value)
                            {
                                objevent.Id = e.Id.Value;
                                objevent.StartTime = e.StartTime;
                                objevent.Morning = e.Morning.Value;
                                objevent.Content = e.Content;
                                objevent.NotificationTimeSpan = e.NotificationTimeSpan;
                                objevent.Title = e.StartTime != null ? string.Format("{0:HH}h{1:mm}: ", e.StartTime, e.StartTime) : "";
                                objevent.Participant = e.Participant;
                                objevent.Place = e.Place;
                                objevent.Public = e.Public.Value;
                                objevent.Accepted = e.Accepted.Value;
                                objevent.AcceptedBy = e.AcceptedBy;
                                objevent.AcceptedOn = e.AcceptedOn;
                                objevent.Canceled = e.Canceled.Value;
                                objevent.CanceledBy = e.CanceledBy;
                                objevent.CanceledOn = e.CanceledOn;
                                objevent.CreatedOn = e.CreatedOn.Value;
                                objevent.CreatedBy = e.CanceledBy;
                                objevent.Important = e.Important.Value;
                                objevent.FullName = e.FullName;
                                objevent.Position = e.ShortPosition;
                                leader.ContentMornings.Add(objevent);
                            }
                            else if (!e.Morning.Value)
                            {
                                objevent.Id = e.Id.Value;
                                objevent.Morning = e.Morning.Value;
                                objevent.StartTime = e.StartTime;
                                objevent.Content = e.Content;
                                objevent.Title = e.StartTime != null ? string.Format("{0:HH}h{1:mm}: ", e.StartTime, e.StartTime) : "";
                                objevent.NotificationTimeSpan = e.NotificationTimeSpan;
                                objevent.Participant = e.Participant;
                                objevent.Place = e.Place;
                                objevent.Public = e.Public.Value;
                                objevent.Accepted = e.Accepted.Value;
                                objevent.AcceptedBy = e.AcceptedBy;
                                objevent.AcceptedOn = e.AcceptedOn;
                                objevent.Canceled = e.Canceled.Value;
                                objevent.CanceledBy = e.CanceledBy;
                                objevent.CanceledOn = e.CanceledOn;
                                objevent.CreatedOn = e.CreatedOn.Value;
                                objevent.CreatedBy = e.CanceledBy;
                                objevent.Important = e.Important.Value;
                                objevent.FullName = e.FullName;
                                objevent.Position = e.ShortPosition;
                                leader.ContentAfternoons.Add(objevent);
                            }
                            else
                            {
                                objevent.Content = "";
                                leader.ContentAfternoons.Add(objevent);
                                leader.ContentMornings.Add(objevent);
                            }

                        }
                        if (leader.ContentAfternoons.Count == 0)
                        {
                            var obj = new EventOfDay();
                            obj.Content = "";
                            leader.ContentAfternoons.Add(obj);
                        }
                        if (leader.ContentMornings.Count == 0)
                        {
                            var obj = new EventOfDay();
                            obj.Content = "";
                            leader.ContentMornings.Add(obj);
                        }
                    }
                    else
                    {
                        var objevent = new EventOfDay();
                        objevent.Content = "";
                        leader.ContentAfternoons.Add(objevent);
                        leader.ContentMornings.Add(objevent);
                    }
                    leader.ContentAfternoons = leader.ContentAfternoons.OrderBy(x => x.StartTime).ToList();
                    leader.ContentMornings = leader.ContentMornings.OrderBy(x => x.StartTime).ToList();
                    customDayEventOfWeek.EventLeaders.Add(leader);
                }

                ListCustomEventOfWeek.Add(customDayEventOfWeek);
            }
            return ListCustomEventOfWeek;
            //return;
        }

        public List<CustomEventOfWeek> GetDeparmentEventOfWeek(int departmentId, DateTime date)
        {
            CultureInfo calendarInfo = new CultureInfo("vi-VN");
            Calendar calendar = calendarInfo.Calendar;
            CalendarWeekRule calWeekRule = calendarInfo.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dayOfweek = calendarInfo.DateTimeFormat.FirstDayOfWeek;
            StaffRepository staffRepostiory = new StaffRepository();
            EventGoogleEventRepository eventGoogleEventRepository = new EventGoogleEventRepository();
            DateTime fromDate = date.GetFirstDayOfWeek();
            DateTime toDate = fromDate.AddDays(6);
            int week = calendar.GetWeekOfYear(date, calWeekRule, dayOfweek) - 1;

            if (week == 0) week = calendar.GetWeekOfYear(new DateTime(fromDate.Year, 12, 31), calWeekRule, dayOfweek) - 1;
            List<SPGetDepartmentEventOfWeek_Result> result = new List<SPGetDepartmentEventOfWeek_Result>();
            result = _entities.SPGetDepartmentEventOfWeek(departmentId, fromDate, toDate).ToList();
            List<CustomEventOfWeek> ListCustomEventOfWeek = new List<CustomEventOfWeek>();

            var importantJob = _entities.SPGetImportantJob(departmentId, fromDate, toDate, true).FirstOrDefault();
            var noteJob = _entities.SPGetImportantJob(departmentId, fromDate, toDate, false).FirstOrDefault();
            for (int i = 0; i <= 6; i++)
            {
                CustomEventOfWeek customEventOfWeek = new CustomEventOfWeek();
                customEventOfWeek.OccurDate = fromDate.AddDays(i);
                customEventOfWeek.STT = i;
                customEventOfWeek.Week = string.Format("Tuần {0}: từ ngày {1:dd/MM/yyyy} đến ngày {2:dd/MM/yyyy}", week, fromDate, toDate);
                customEventOfWeek.Title = string.Format("{0} <br /> {1:dd/MM}", Util.GetDayOfWeek(i), fromDate.AddDays(i).Date);
                if (importantJob != null)
                {
                    customEventOfWeek.ImportantJobString = importantJob.Content;
                    customEventOfWeek.ImportantJobId = importantJob.Id;
                }

                if (noteJob != null)
                {
                    customEventOfWeek.NoteJobString = noteJob.Content;
                    customEventOfWeek.ImportantJobIdNormal = noteJob.Id;
                }
                var oevents = result.Where(x => x.OccurDate.Date == fromDate.AddDays(i).Date).ToList();

                customEventOfWeek.ContentMornings = new List<EventOfDay>();
                customEventOfWeek.ContentAfternoons = new List<EventOfDay>();

                foreach (var item in oevents)
                {
                    var objevent = new EventOfDay();
                    if (item.Morning)
                    {

                        objevent.Id = item.Id;
                        objevent.OccurDate = item.OccurDate;
                        objevent.StartTime = item.StartTime;
                        objevent.Morning = item.Morning;
                        objevent.Content = item.Content;
                        objevent.NotificationTimeSpan = item.NotificationTimeSpan;
                        objevent.Title = item.StartTime != null ? string.Format("{0:HH}h{1:mm}: ", item.StartTime, item.StartTime) : "";
                        objevent.Participant = item.Participant;
                        objevent.Place = item.Place;
                        objevent.Public = item.Public;
                        objevent.Accepted = item.Accepted;
                        objevent.AcceptedBy = item.AcceptedBy;
                        objevent.AcceptedOn = item.AcceptedOn;
                        objevent.Canceled = item.Canceled;
                        objevent.CanceledBy = item.CanceledBy;
                        objevent.CanceledOn = item.CanceledOn;
                        objevent.CreatedOn = item.CreatedOn;
                        objevent.CreatedBy = item.CreatedBy;
                        objevent.Important = item.Important;
                        objevent.Blue = item.CreatedOn != item.EditedOn ? true : false;
                        objevent.FullName = staffRepostiory.FindBy(x => x.UserId == item.CreatedBy).FirstOrDefault().FullName;
                        objevent.GoogleEvent = eventGoogleEventRepository.GetByEventId(item.Id);

                        customEventOfWeek.ContentMornings.Add(objevent);
                    }
                    else
                    {
                        objevent.Id = item.Id;
                        objevent.OccurDate = item.OccurDate;
                        objevent.Morning = item.Morning;
                        objevent.StartTime = item.StartTime;
                        objevent.Content = item.Content;
                        objevent.Title = item.StartTime != null ? string.Format("{0:HH}h{1:mm}: ", item.StartTime, item.StartTime) : "";
                        objevent.NotificationTimeSpan = item.NotificationTimeSpan;
                        objevent.Participant = item.Participant;
                        objevent.Place = item.Place;
                        objevent.Accepted = item.Accepted;
                        objevent.Public = item.Public;
                        objevent.AcceptedBy = item.AcceptedBy;
                        objevent.AcceptedOn = item.AcceptedOn;
                        objevent.Canceled = item.Canceled;
                        objevent.CanceledBy = item.CanceledBy;
                        objevent.CanceledOn = item.CanceledOn;
                        objevent.CreatedOn = item.CreatedOn;
                        objevent.CreatedBy = item.CreatedBy;
                        objevent.Important = item.Important;
                        objevent.Blue = item.CreatedOn != item.EditedOn ? true : false;
                        objevent.FullName = staffRepostiory.FindBy(x => x.UserId == item.CreatedBy).FirstOrDefault().FullName;
                        objevent.GoogleEvent = eventGoogleEventRepository.GetByEventId(item.Id);
                        customEventOfWeek.ContentAfternoons.Add(objevent);
                    }

                }
                ListCustomEventOfWeek.Add(customEventOfWeek);
            }
            return ListCustomEventOfWeek;
            //return;
        }
        public List<CustomEventOfWeek> GetMutilDepartmentEventOfWeek(string listDepartmentId, DateTime date)
        {
            var arraryDepartmentId = listDepartmentId.Split(',').ToArray();
            var listReuslt = new List<CustomEventOfWeek>();
            foreach (var item in arraryDepartmentId)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int departmentId = Convert.ToInt32(item);
                    var deparment = _entities.Departments.First(x => x.Id == departmentId);
                    var departmentEvent = GetDeparmentEventOfWeek(departmentId, date);
                    if (listReuslt.Count == 0)
                    {
                        foreach (var e in departmentEvent)
                        {
                            e.ImportantJobString = !string.IsNullOrEmpty(e.ImportantJobString) ? "<b>(" + deparment.ShortName + ") </b>" + e.ImportantJobString : "";
                            e.NoteJobString = !string.IsNullOrEmpty(e.NoteJobString) ? "<b>(" + deparment.ShortName + ") </b>" + e.NoteJobString : "";
                            foreach (var a in e.ContentMornings)
                            {
                                a.ShortDepartment = deparment.ShortName;
                            }

                            foreach (var b in e.ContentAfternoons)
                            {
                                b.ShortDepartment = deparment.ShortName;
                            }
                        }
                        listReuslt.AddRange(departmentEvent);
                    }
                    else
                    {
                        for (int i = 0; i < listReuslt.Count; i++)
                        {
                            if (i == 0)
                            {
                                if (string.IsNullOrEmpty(listReuslt[i].ImportantJobString) &&!string.IsNullOrEmpty(departmentEvent[i].ImportantJobString))
                                {
                                    listReuslt[i].ImportantJobString = "<b>("  + deparment.ShortName + ") </b>"+ departmentEvent[i].ImportantJobString;
                                }
                                else
                                {
                                    string job = string.IsNullOrEmpty(departmentEvent[i].ImportantJobString) ? "" : "<br /> <b>(" + deparment.ShortName + ") </b>" + departmentEvent[i].ImportantJobString;
                                    listReuslt[i].ImportantJobString = listReuslt[i].ImportantJobString + job;
                                }
                                if (string.IsNullOrEmpty(listReuslt[i].NoteJobString) && !string.IsNullOrEmpty(departmentEvent[i].NoteJobString))
                                {
                                    listReuslt[i].NoteJobString = "<b>(" + deparment.ShortName + ")</b> " + departmentEvent[i].NoteJobString;
                                }
                                else
                                {
                                    string ob = string.IsNullOrEmpty(departmentEvent[i].NoteJobString) ? "" : "<br /> <b>(" + deparment.ShortName + ") </b>" + departmentEvent[i].NoteJobString;
                                    listReuslt[i].NoteJobString = listReuslt[i].NoteJobString + ob;
                                }
                            }
                            foreach (var s in departmentEvent[i].ContentMornings)
                            {
                                s.ShortDepartment = deparment.ShortName;
                                listReuslt[i].ContentMornings.Add(s);
                            }
                            foreach (var s in departmentEvent[i].ContentAfternoons)
                            {
                                s.ShortDepartment = deparment.ShortName;
                                listReuslt[i].ContentAfternoons.Add(s);
                            }
                        }
                    }
                }

            }
            return listReuslt;
        }
    }
}
