using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Model.Validators;
using VOffice.Repository;
using VOffice.Repository.Queries;

namespace VOffice.ApplicationService
{
    public partial class CalendarService : BaseService, ICalendarService
    {
        protected readonly ApplicationLoggingRepository _applicationLoggingRepository;
        protected readonly EventRepository _eventRepository;
        protected readonly EventUserNotifyRepository _eventUserNotifyRepository;
        protected readonly EventLeaderRepository _eventLeaderRepository;
        protected readonly ImportantJobRepository _importantJobRepository;
        protected readonly EventGoogleEventRepository _eventGoogleEventRepository;
        protected readonly UserNotificationRepository _userNotificationRepository;
        protected readonly DepartmentRepository _departmentRepository;
        protected readonly NotificationCenterRepository _notificationCenterRepository;

        public CalendarService()
        {
            _applicationLoggingRepository = new ApplicationLoggingRepository();
            _eventRepository = new EventRepository();
            _eventUserNotifyRepository = new EventUserNotifyRepository();
            _eventLeaderRepository = new EventLeaderRepository();
            _importantJobRepository = new ImportantJobRepository();
            _eventGoogleEventRepository = new EventGoogleEventRepository();
            _userNotificationRepository = new UserNotificationRepository();
            _departmentRepository = new DepartmentRepository();
            _notificationCenterRepository = new NotificationCenterRepository();
        }

        #region Event

        public BaseResponse<ComplexEvent> GetEventById(int id)
        {
            var response = new BaseResponse<ComplexEvent>();
            var complexEvent = new ComplexEvent();
            try
            {
                var currentEvent = _eventRepository.GetById(id);
                complexEvent.Accepted = currentEvent.Accepted;
                complexEvent.AcceptedBy = currentEvent.AcceptedBy;
                complexEvent.AcceptedOn = currentEvent.AcceptedOn;
                complexEvent.Active = currentEvent.Active;
                complexEvent.Canceled = currentEvent.Canceled;
                complexEvent.CanceledBy = currentEvent.CanceledBy;
                complexEvent.CanceledOn = currentEvent.CanceledOn;
                complexEvent.Content = currentEvent.Content;
                complexEvent.CreatedBy = currentEvent.CreatedBy;
                complexEvent.CreatedOn = currentEvent.CreatedOn;
                complexEvent.Deleted = currentEvent.Deleted;
                complexEvent.DepartmentId = currentEvent.DepartmentId;
                complexEvent.EditedBy = currentEvent.EditedBy;
                complexEvent.EditedOn = currentEvent.EditedOn;
                complexEvent.EndTime = currentEvent.EndTime;
                complexEvent.EndTimeUndefined = currentEvent.EndTimeUndefined;
                complexEvent.Id = currentEvent.Id;
                complexEvent.Important = currentEvent.Important;
                complexEvent.MeetingRoomId = currentEvent.MeetingRoomId;
                complexEvent.Morning = currentEvent.Morning;
                complexEvent.NotificationTimeSpan = currentEvent.NotificationTimeSpan;
                complexEvent.OccurDate = currentEvent.OccurDate;
                complexEvent.Participant = currentEvent.Participant;
                complexEvent.Personal = currentEvent.Personal;
                complexEvent.Public = currentEvent.Public;
                complexEvent.StartTime = currentEvent.StartTime;
                complexEvent.StartHour = currentEvent.StartTime.HasValue ? currentEvent.StartTime.Value.Hour : 0;
                complexEvent.EndHour = currentEvent.EndTime.HasValue ? currentEvent.EndTime.Value.Hour : 0;
                complexEvent.Place = currentEvent.Place;
                complexEvent.StartTimeUnderfined = currentEvent.StartTimeUnderfined;
                complexEvent.LeaderEvents = _eventLeaderRepository.GetEventUserNotifyByEventId(id);
                complexEvent.EventUserNotifys = _eventUserNotifyRepository.GetEventUserNotifyByEventId(id);
                complexEvent.GoogleEvent = _eventGoogleEventRepository.GetByEventId(id);

                response.Value = complexEvent;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }

        public BaseResponse<Event> AddEvent(Event model)
        {
            var response = new BaseResponse<Event>();
            var errors = Validate<Event>(model, new EventValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Event> errResponse = new BaseResponse<Event>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _eventRepository.Add(model);
                _applicationLoggingRepository.Log("EVENT", "CREATE", "Event", response.Value.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<Event> UpdateEvent(Event model)
        {
            var response = new BaseResponse<Event>();
            var errors = Validate<Event>(model, new EventValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Event> errResponse = new BaseResponse<Event>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _eventRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "EDIT", "Event", model.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse DeleteLogicalEvent(int id)
        {
            BaseResponse response = new BaseResponse();
            Event model = _eventRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _eventRepository.Edit(model);
                _applicationLoggingRepository.Log("EVENT", "DELETE", "Event", id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                // xoa chi tiet
                _eventLeaderRepository.DeleteMulti(x => x.EventId == id);
                _eventUserNotifyRepository.DeleteMulti(x => x.EventId == id);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse DeferredEvent(CustomEventDeferred input)
        {
            BaseResponse response = new BaseResponse();
            Event model = _eventRepository.GetById(input.EventId);
            try
            {
                model.Canceled = input.Canceled;
                model.CanceledBy = input.UserId;
                model.CanceledOn = DateTime.Now;
                model.EditedBy = input.UserId;
                _eventRepository.Edit(model);
                _applicationLoggingRepository.Log("EVENT", "DEFERRED", "Event", input.EventId.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public System.Threading.Tasks.Task<BaseResponse> AcceptedEvent(CustomEventAccepted input)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                BaseResponse response = new BaseResponse();
                Event model = _eventRepository.GetById(input.EventId);
                try
                {
                    model.Accepted = input.Accepted;
                    model.AcceptedBy = input.UserId;
                    model.AcceptedOn = DateTime.Now;
                    model.EditedBy = input.UserId;
                    _eventRepository.Edit(model);
                    _applicationLoggingRepository.Log("EVENT", "ACCEPTED", "Event", input.EventId.ToString(), "", "", model, "", System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Request.UserHostAddress : "", model.CreatedBy);

                    // sendmail
                    if (input.Accepted)
                    {
                        string mailTo = (new AspNetUsersRepository()).GetUserByUserId(model.CreatedBy).Email;
                        #region Send email
                        try
                        {
                            //mainAssignee: Người xử lý chính
                            //listUserRelate: Người tạo, đồng xử lý, giám sát 
                            string domain = (new SystemConfigDepartmentRepository()).GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = model.DepartmentId, Title = "APP_DOMAIN", DefaultValue = "http://congvan.veph.vn/" });
                            string subject = (new SystemConfigDepartmentRepository()).GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = model.DepartmentId, Title = "TASKNOTICEEMAILSUBJECT", DefaultValue = "[VOffice] - Thông báo" });
                            subject += " - Lịch công tác: Đăng ký sự kiện thành công";
                            string eventDetailUrl = domain + "#!/lich-cong-tac////";
                            string temp = @"
                                   <table style='width:100%'>
                                        <tr>
                                            <td><a style='text-decoration:none;' href='" + eventDetailUrl + @"'>Lịch công tác</a></td>
                                        </tr>
                                        <tr>
                                            <td>
                                            <div style='border-bottom:1px dotted #ddd; margin:5px 0; width:100%'></div>
                                            </td></tr>
                                        <tr>
                                            <td><div style='border: 1px solid #e7eaec;box-shadow: none;margin-top: 10px;margin-bottom: 5px;padding: 10px 20px;line-height:16px;border-radius: 4px;min-height:20px;background-color: #f5f5f5;'>{0}</div></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                        </tr>
                                            </table>";

                            StringBuilder contentNotice = new StringBuilder();
                            if (!string.IsNullOrEmpty(model.Content))
                            {
                                contentNotice.Append("<b>").Append(model.StartTime != null ? string.Format("{0:HH}h{1:mm}: ", model.StartTime, model.StartTime) : "").Append("</b>").Append(model.Content);

                                if (!string.IsNullOrEmpty(model.Participant))
                                {
                                    contentNotice.Append("<br />Thành phần: ").Append(model.Participant);
                                }
                                if (!string.IsNullOrEmpty(model.Place))
                                {
                                    contentNotice.Append("<br />Địa điểm: ").Append(model.Place);
                                }

                            }
                            var content = string.Format(temp, contentNotice.ToString());
                            UtilityProvider.SendEmail(model.DepartmentId, mailTo, "", "", subject, content);
                        }
                        catch
                        { }
                        #endregion Send email
                        #region Send notification
                        var listUserSendNotifi = new List<SPGetUserNotificationForDocumentAndEvent_Result>();
                        listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByDepartmentId(model.DepartmentId));
                        var userProfile = _departmentRepository.GetUserMainOrganization(model.EditedBy);
                        NotificationCenter resultNotifiCenter = null;
                        NotificationCenter notifiFirst = null;
                        foreach (var re in listUserSendNotifi)
                        {
                            FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                            fcmNotificationCenter.Avatar = userProfile.Avatar;
                            fcmNotificationCenter.FullName = userProfile.FullName;
                            fcmNotificationCenter.Content = "đã duyệt một sự kiện mới: \n" + model.Content;
                            fcmNotificationCenter.Title = "vOffice";
                            fcmNotificationCenter.RecordId = model.Id;
                            fcmNotificationCenter.Type = (int)NotificationCode.CreateEvent;
                            fcmNotificationCenter.CreatedBy = model.EditedBy;
                            fcmNotificationCenter.CreatedOn = model.CreatedOn;
                            fcmNotificationCenter.HaveSeen = false;
                            fcmNotificationCenter.ReceivedUserId = re.UserId;
                            fcmNotificationCenter.DeviceId = re.DeviceId;
                            if (notifiFirst != null && notifiFirst.ReceivedUserId == re.UserId)
                            {
                                fcmNotificationCenter.GroupId = notifiFirst.Id;
                            }
                            var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                            resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                            fcmNotificationCenter.Id = resultNotifiCenter.Id;
                            if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                            FCMPushNotification fcmPushNotification = new FCMPushNotification();
                            fcmPushNotification.SendNotification(fcmNotificationCenter, re.ClientId);
                        }
                        #endregion
                    }

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                }
                return response;
            });

        }
        public BaseResponse<Event> AddComplexEvent(ComplexEvent model)
        {
            var response = new BaseResponse<Event>();
            var newEvent = new Event
            {
                DepartmentId = model.DepartmentId,
                OccurDate = model.OccurDate,
                Content = model.Content,
                Place = model.Place,
                MeetingRoomId = model.MeetingRoomId,
                Participant = model.Participant,
                StartTime = model.StartTime,
                StartTimeUnderfined = model.StartTimeUnderfined,
                EndTime = model.EndTime,
                EndTimeUndefined = model.EndTimeUndefined,
                Morning = model.Morning,
                Public = model.Public,
                Important = model.Important,
                NotificationTimeSpan = model.NotificationTimeSpan,
                Accepted = model.Accepted,
                AcceptedBy = model.AcceptedBy,
                AcceptedOn = model.AcceptedOn,
                Canceled = model.Canceled,
                CanceledBy = model.CanceledBy,
                CanceledOn = model.CanceledOn,
                Active = model.Active,
                Deleted = model.Deleted,
                CreatedBy = model.CreatedBy,
                CreatedOn = DateTime.Now,
                EditedBy = model.EditedBy,
                EditedOn = DateTime.Now
            };
            if (model.StartTime != null)
            {
                newEvent.Morning = model.StartTime.Value.Hour <= 12 ? true : false;
            }
            var errors = Validate<Event>(newEvent, new EventValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Event> errResponse = new BaseResponse<Event>(newEvent, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                response.Value = _eventRepository.Add(newEvent);
                _applicationLoggingRepository.Log("EVENT", "CREATE", "Event", response.Value.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, newEvent.CreatedBy);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            #region send notifcation
            if (newEvent.Accepted)
            {

                var listUserSendNotifi = new List<SPGetUserNotificationForDocumentAndEvent_Result>();
                var userProfile = _departmentRepository.GetUserMainOrganization(model.CreatedBy);

                listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByDepartmentId(newEvent.DepartmentId));

                NotificationCenter resultNotifiCenter = null;
                NotificationCenter notifiFirst = null;
                foreach (var re in listUserSendNotifi)
                {
                    FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                    fcmNotificationCenter.Avatar = userProfile.Avatar;
                    fcmNotificationCenter.FullName = userProfile.FullName;
                    fcmNotificationCenter.Content = "đã tạo một sự kiện mới: \n" + newEvent.Content;
                    fcmNotificationCenter.Title = "vOffice";
                    fcmNotificationCenter.RecordId = response.Value.Id;
                    fcmNotificationCenter.Type = (int)NotificationCode.CreateEvent;
                    fcmNotificationCenter.CreatedBy = model.CreatedBy;
                    fcmNotificationCenter.CreatedOn = newEvent.CreatedOn;
                    fcmNotificationCenter.HaveSeen = false;
                    fcmNotificationCenter.ReceivedUserId = re.UserId;
                    fcmNotificationCenter.DeviceId = re.DeviceId;
                    if (notifiFirst != null && notifiFirst.ReceivedUserId == re.UserId)
                    {
                        fcmNotificationCenter.GroupId = notifiFirst.Id;
                    }
                    var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                    resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                    fcmNotificationCenter.Id = resultNotifiCenter.Id;
                    if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                    FCMPushNotification fcmPushNotification = new FCMPushNotification();
                    fcmPushNotification.SendNotification(fcmNotificationCenter, re.ClientId);
                }


            }
            #endregion
            // add event notify
            try
            {
                if (model.EventUserNotifys != null)
                {
                    if (model.EventUserNotifys.Count > 0)
                    {
                        foreach (var item in model.EventUserNotifys.ToList())
                        {
                            item.CreatedOn = DateTime.Now;
                            item.EventId = response.Value.Id;
                            _eventUserNotifyRepository.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }

            try
            {
                // add event leader
                if (model.LeaderEvents != null)
                {
                    if (model.LeaderEvents.Count > 0)
                    {
                        foreach (var item in model.LeaderEvents.ToList())
                        {
                            item.CreatedOn = DateTime.Now;
                            item.EventId = response.Value.Id;
                            _eventLeaderRepository.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }

            return response;
        }

        public BaseResponse<Event> UpdateComplexEvent(ComplexEvent model)
        {
            var response = new BaseResponse<Event>();
            var eventUpdate = _eventRepository.GetById(model.Id);

            eventUpdate.Accepted = model.Accepted;
            eventUpdate.AcceptedBy = model.AcceptedBy;
            eventUpdate.AcceptedOn = model.AcceptedOn;
            eventUpdate.Active = model.Active;
            eventUpdate.Canceled = model.Canceled;
            eventUpdate.CanceledBy = model.CanceledBy;
            eventUpdate.CanceledOn = model.CanceledOn;
            eventUpdate.Content = model.Content;
            eventUpdate.DepartmentId = model.DepartmentId;
            eventUpdate.EditedBy = model.EditedBy;
            eventUpdate.EditedOn = DateTime.Now;
            eventUpdate.EndTime = model.EndTime;
            eventUpdate.EndTimeUndefined = model.EndTimeUndefined;
            eventUpdate.Important = model.Important;
            eventUpdate.MeetingRoomId = model.MeetingRoomId;
            if (model.StartTime != null)
            {
                eventUpdate.Morning = model.StartTime.Value.Hour <= 12 ? true : false;
            }
            else
            {
                eventUpdate.Morning = model.Morning;
            }
            eventUpdate.NotificationTimeSpan = model.NotificationTimeSpan;
            eventUpdate.OccurDate = model.OccurDate;
            eventUpdate.Participant = model.Participant;
            eventUpdate.Personal = model.Personal;
            eventUpdate.Place = model.Place;
            eventUpdate.Public = model.Public;
            eventUpdate.StartTime = model.StartTime;
            eventUpdate.StartTimeUnderfined = model.StartTimeUnderfined;

            var errors = Validate<Event>(eventUpdate, new EventValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Event> errResponse = new BaseResponse<Event>(eventUpdate, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                response.Value = _eventRepository.Edit(eventUpdate);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "EDIT", "Event", eventUpdate.Id.ToString(), "", "", eventUpdate, "", System.Web.HttpContext.Current.Request.UserHostAddress, eventUpdate.CreatedBy);
                }
                catch
                { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            // delete event learde, event notifi
            _eventLeaderRepository.DeleteMulti(x => x.EventId == model.Id);
            _eventUserNotifyRepository.DeleteMulti(x => x.EventId == model.Id);
            if (model.Accepted)
            {
                #region Send notification
                var listUserSendNotifi = new List<SPGetUserNotificationForDocumentAndEvent_Result>();
                listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByDepartmentId(model.DepartmentId));
                var userProfile = _departmentRepository.GetUserMainOrganization(model.EditedBy);
                NotificationCenter resultNotifiCenter = null;
                NotificationCenter notifiFirst = null;
                foreach (var re in listUserSendNotifi)
                {
                    FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                    fcmNotificationCenter.Avatar = userProfile.Avatar;
                    fcmNotificationCenter.FullName = userProfile.FullName;
                    fcmNotificationCenter.Content = "đã cập nhật một sự kiện: \n" + model.Content;
                    fcmNotificationCenter.Title = "vOffice";
                    fcmNotificationCenter.RecordId = model.Id;
                    fcmNotificationCenter.Type = (int)NotificationCode.CreateEvent;
                    fcmNotificationCenter.CreatedBy = model.EditedBy;
                    fcmNotificationCenter.CreatedOn = model.CreatedOn;
                    fcmNotificationCenter.HaveSeen = false;
                    fcmNotificationCenter.ReceivedUserId = re.UserId;
                    fcmNotificationCenter.DeviceId = re.DeviceId;
                    if (notifiFirst != null && notifiFirst.ReceivedUserId == re.UserId)
                    {
                        fcmNotificationCenter.GroupId = notifiFirst.Id;
                    }
                    var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                    resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                    fcmNotificationCenter.Id = resultNotifiCenter.Id;
                    if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                    FCMPushNotification fcmPushNotification = new FCMPushNotification();
                    fcmPushNotification.SendNotification(fcmNotificationCenter, re.ClientId);
                }
                #endregion
            }
            // add event notify
            try
            {
                if (model.EventUserNotifys != null)
                {
                    if (model.EventUserNotifys.Count > 0)
                    {
                        foreach (var item in model.EventUserNotifys.ToList())
                        {
                            item.CreatedOn = DateTime.Now;
                            item.EventId = response.Value.Id;
                            _eventUserNotifyRepository.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }

            try
            {
                // add event leader
                if (model.LeaderEvents != null)
                {
                    if (model.LeaderEvents.Count > 0)
                    {
                        foreach (var item in model.LeaderEvents.ToList())
                        {
                            item.CreatedOn = DateTime.Now;
                            item.EventId = response.Value.Id;
                            _eventLeaderRepository.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }

            return response;
        }

        public BaseListResponse<SPGetEventOfDepartment_Result> GetEventOfDepartment(int type, int departmentId, string userId, bool morning, DateTime date)
        {
            var response = new BaseListResponse<SPGetEventOfDepartment_Result>();
            try
            {
                response.Data = _eventRepository.GetEventOfDepartment(type, departmentId, userId, morning, date);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }

        public BaseListResponse<SPGetEventOfDepartment_Result> GetEventAcceptedOfDepartment(int type, int departmentId, string userId, bool morning, DateTime date)
        {
            var response = new BaseListResponse<SPGetEventOfDepartment_Result>();
            try
            {
                response.Data = _eventRepository.GetEventOfDepartment(type, departmentId, userId, morning, date).Where(x => x.Accepted == true).ToList();
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }

        public BaseListResponse<CustomEventOfWeek> GetDepartmentEventOfWeek(int deparmentId, DateTime date)
        {
            var response = new BaseListResponse<CustomEventOfWeek>();
            try
            {
                response.Data = _eventRepository.GetDeparmentEventOfWeek(deparmentId, date);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<CustomEventOfWeek> GetMultiDepartmentEventOfWeek(string listDeparmentId, DateTime date)
        {
            var response = new BaseListResponse<CustomEventOfWeek>();
            try
            {
                response.Data = _eventRepository.GetMutilDepartmentEventOfWeek(listDeparmentId, date);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseListResponse<CustomDayEventOfWeek> GetDepartmentLeaderEventOfWeek(int deparmentId, DateTime date)
        {
            var response = new BaseListResponse<CustomDayEventOfWeek>();
            try
            {
                response.Data = _eventRepository.GetDeparmentLeaderEventOfWeek(deparmentId, date);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<string> GetEventFileDownLoad(int deparmentId, DateTime date, bool accepted, string userId)
        {
            DepartmentRepository _departmentRepository = new DepartmentRepository();
            List<Department> lstDepartment = _departmentRepository.GetListDepartmentsByUserId(userId);
            bool checkDepartment = false;
            foreach (var item in lstDepartment)
            {
                if (item.Id == deparmentId)
                {
                    checkDepartment = true;
                    break;
                }
            }
            List<CustomEventOfWeek> source;
            source = _eventRepository.GetDeparmentEventOfWeek(deparmentId, date);
            DepartmentRepository deparmentRepository = new DepartmentRepository();
            StringBuilder content = new StringBuilder();
            var response = new BaseResponse<string>();
            var rootDeparment = deparmentRepository.FindBy(x => x.ParentId == 0).FirstOrDefault();
            var deparment = deparmentRepository.FindBy(x => x.Id == deparmentId).FirstOrDefault();
            int i = 0;
            content.Append("<html>");
            content.Append("<body>");
            content.Append("<table style='width:100%'>");
            content.Append("<tr>").Append("<td>");
            content.Append(rootDeparment == null ? "" : rootDeparment.Name.ToUpper());
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td>");
            content.Append(deparment == null ? "" : deparment.Name.ToUpper());
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td align='center'>");
            content.Append("<span style='font-weight:bold;font-size:16pt;padding:10px;'>LỊCH CÔNG TÁC TUẦN</span>");
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td align='center' >");
            content.Append("<span  style ='font-style: italic;'>(").Append(source[0].Week).Append(")</span>");
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td stype='padding-top:10px;'>");
            content.Append(" <strong> I. CÔNG TÁC TRỌNG TÂM TRONG TUẦN </strong>");
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td style='line-height: 30px; padding:4px 4px 4px 10px;'>");
            content.Append(source[0].ImportantJobString);
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td>");
            content.Append("<strong>II. LỊCH HỌP CƠ QUAN </strong>");
            content.Append("</td>").Append("</tr>");
            content.Append("</table>");
            content.Append("<br />");
            content.Append("<table border='1' cellspacing='0' style='width:100 %; border-collapse: collapse;'>");
            content.Append("<tr style='height:70px;'>");
            content.Append("<td style='width:15%;border: solid 1px;  border-style: double solid solid double;'>");
            content.Append("<table style='width:100%;border-collapse: collapse;' cellspacing='0'>").Append("<tr>");
            content.Append("<td align=right>").Append("<strong>BUỔI</strong>").Append("</td>");
            content.Append("</tr><tr>");
            content.Append("<td align=left>").Append("<strong>THỨ</strong>").Append("</td>");
            content.Append("</tr>").Append("</table>");
            content.Append("</td>");
            content.Append("<td style='border:solid 1px; border-style: double solid solid solid; width:42%; text-align:center;'>");
            content.Append("<p style='text-align:center;'><strong>SÁNG</strong></p>");
            content.Append("</td>");
            content.Append("<td style='border: solid 1px; border-style: double double solid solid; text-align:center;'>");
            content.Append("<p style='text-align:center;'><strong>CHIỀU</strong></p>");
            content.Append("</td>");
            content.Append("</tr>");
            foreach (var item in source)
            {
                i++;
                if (i == source.Count)
                {
                    content.Append("<tr style='height: 70px;'>");
                    content.Append("<td style='border-style: solid solid double double;' align='center'>");
                    content.Append("<span style='font-weight: bold;'>").Append(item.Title).Append("</span>");
                    content.Append("</td>");
                    content.Append("<td style='border-style: solid solid double solid;vertical-align:top; line-height: 30px; padding:4px;'  >");
                    foreach (var mo in item.ContentMornings)
                    {
                        if (accepted)
                        {
                            if (mo.Accepted)
                            {
                                if (checkDepartment == true || (checkDepartment == false && mo.Public == true))
                                {
                                    content.Append("<span style='text-align:left;'><strong>").Append(mo.Title).Append("</span></strong>");
                                    content.Append(mo.Content);
                                    if (!string.IsNullOrEmpty(mo.Participant))
                                    {
                                        content.Append("<br /><strong>TP: </strong>").Append(mo.Participant);
                                    }
                                    if (!string.IsNullOrEmpty(mo.Place))
                                    {
                                        content.Append("<br /><strong>Địa điểm: </strong>").Append(mo.Place).Append("<br />");
                                    }
                                    else
                                    {
                                        content.Append("<br />");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!mo.Accepted) content.Append("<span style='text-decoration: line-through;'>");
                            content.Append("<span style='text-align:left;'><strong>").Append(mo.Title).Append("</span></strong>");
                            content.Append(mo.Content);
                            if (!string.IsNullOrEmpty(mo.Participant))
                            {
                                content.Append("<br /><strong>TP: </strong>").Append(mo.Participant);
                            }
                            if (!string.IsNullOrEmpty(mo.Place))
                            {
                                content.Append("<br /><strong>Địa điểm: </strong>").Append(mo.Place).Append("<br />");
                            }
                            else
                            {
                                content.Append("<br />");
                            }
                            if (!mo.Accepted) content.Append("</span>");
                        }
                    }
                    content.Append("</td>");
                    content.Append("<td style='border-style: solid double double solid; vertical-align:top; line-height: 30px; padding:4px;'  >");
                    foreach (var af in item.ContentAfternoons)
                    {
                        if (accepted)
                        {
                            if (af.Accepted)
                            {
                                if (checkDepartment == true || (checkDepartment == false && af.Public == true))
                                {
                                    content.Append("<span style='text-align:left;'><strong>").Append(af.Title).Append("</span></strong>");
                                    content.Append(af.Content);
                                    if (!string.IsNullOrEmpty(af.Participant))
                                    {
                                        content.Append("<br /><strong>TP: </strong>").Append(af.Participant);
                                    }
                                    if (!string.IsNullOrEmpty(af.Place))
                                    {
                                        content.Append("<br /><strong>Địa điểm: </strong>").Append(af.Place).Append("<br />");
                                    }
                                    else
                                    {
                                        content.Append("<br />");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!af.Accepted) content.Append("<span style='text-decoration: line-through;'>");
                            content.Append("<span style='text-align:left;'><strong>").Append(af.Title).Append("</span></strong>");
                            content.Append(af.Content);
                            if (!string.IsNullOrEmpty(af.Participant))
                            {
                                content.Append("<br /><strong>TP: </strong>").Append(af.Participant);
                            }
                            if (!string.IsNullOrEmpty(af.Place))
                            {
                                content.Append("<br /><strong>Địa điểm: </strong>").Append(af.Place).Append("<br />");
                            }
                            else
                            {
                                content.Append("<br />");
                            }
                            if (!af.Accepted) content.Append("</span>");
                        }

                    }
                    content.Append("</td>");
                    content.Append("</tr>");
                }
                else
                {
                    content.Append("<tr style='height: 70px;'>");
                    content.Append("<td style='border-style: solid solid solid double;' align='center'>");
                    content.Append("<span style='font-weight: bold;'>").Append(item.Title).Append("</span>");
                    content.Append("</td>");
                    content.Append("<td style='vertical-align:top; line-height:30px; padding:4px;' align=left> ");
                    foreach (var mo in item.ContentMornings)
                    {
                        if (accepted)
                        {
                            if (mo.Accepted)
                            {
                                if (checkDepartment == true || (checkDepartment == false && mo.Public == true))
                                {
                                    content.Append("<span style='text-align:left;'><strong>").Append(mo.Title).Append("</span></strong>");
                                    content.Append(mo.Content);
                                    if (!string.IsNullOrEmpty(mo.Participant))
                                    {
                                        content.Append("<br /><strong>TP: </strong>").Append(mo.Participant);
                                    }
                                    if (!string.IsNullOrEmpty(mo.Place))
                                    {
                                        content.Append("<br /><strong>Địa điểm: </strong>").Append(mo.Place).Append("<br />");
                                    }
                                    else
                                    {
                                        content.Append("<br />");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!mo.Accepted)
                            {
                                if (checkDepartment == true)
                                {
                                    content.Append("<span style='text-decoration: line-through;'>");
                                    content.Append(mo.Content);
                                    if (!string.IsNullOrEmpty(mo.Participant))
                                    {
                                        content.Append("<br /><strong>TP: </strong>").Append(mo.Participant);
                                    }
                                    if (!string.IsNullOrEmpty(mo.Place))
                                    {
                                        content.Append("<br /><strong>Địa điểm: </strong>").Append(mo.Place).Append("<br />");
                                    }
                                    else
                                    {
                                        content.Append("<br />");
                                    }
                                    if (!mo.Accepted) content.Append("</span>");

                                }
                            }
                            else
                            {
                                content.Append("<span style='text-align:left;'><strong>").Append(mo.Title).Append("</span></strong>");

                            }

                        }
                    }
                    content.Append("</td>");
                    content.Append("<td style='border-style:  solid double solid solid;vertical-align:top; line-height: 30px; padding:4px;' >");
                    foreach (var af in item.ContentAfternoons)
                    {
                        if (accepted)
                        {
                            if (af.Accepted)
                            {
                                if (checkDepartment == true || (checkDepartment == false && af.Public == true))
                                {
                                    content.Append("<span style='text-align:left;'><strong>").Append(af.Title).Append("</span></strong>");
                                    content.Append(af.Content);
                                    if (!string.IsNullOrEmpty(af.Participant))
                                    {
                                        content.Append("<br /><strong>TP: </strong>").Append(af.Participant);
                                    }
                                    if (!string.IsNullOrEmpty(af.Place))
                                    {
                                        content.Append("<br /><strong>Địa điểm: </strong>").Append(af.Place).Append("<br />");
                                    }
                                    else
                                    {
                                        content.Append("<br / >");
                                    }
                                }
                            }
                        }
                        else
                        {

                            if (!af.Accepted)
                                if (checkDepartment == true)
                                {
                                    content.Append("<span style='text-decoration: line-through;'>");
                                }
                                else
                                {
                                    content.Append("<span style='text-align:left;'><strong>").Append(af.Title).Append("</span></strong>");
                                }
                            content.Append(af.Content);
                            if (!string.IsNullOrEmpty(af.Participant))
                            {
                                content.Append("<br /><strong>TP: </strong>").Append(af.Participant);
                            }
                            if (!string.IsNullOrEmpty(af.Place))
                            {
                                content.Append("<br /><strong>Địa điểm: </strong>").Append(af.Place).Append("<br />");
                            }
                            else
                            {
                                content.Append("<br / >");
                            }
                            if (!af.Accepted) content.Append("</span>");
                        }
                    }
                    content.Append("</td>");
                    content.Append("</tr>");
                }
            }
            content.Append("</table>");
            content.Append("<br />");
            content.Append("<span style='font-style: italic;font-weight: bold;'>Ghi chú: </span><span>").Append(source[0].NoteJobString).Append("</span>");
            content.Append("</body>");
            content.Append("</html>");
            response.Value = Util.CreateDocument(content.ToString(), "event", "Demo1");
            return response;
        }
        public BaseResponse<string> GetLeaderEventFileDownLoad(int deparmentId, DateTime date)
        {
            var source = _eventRepository.GetDeparmentLeaderEventOfWeek(deparmentId, date);
            var deparment = new DepartmentRepository().GetById(deparmentId);
            StringBuilder content = new StringBuilder();
            var response = new BaseResponse<string>();
            content.Append("<html><body>");
            content.Append("<table style='width:100%;border-collapse: collapse;' cellspacing='0'>");
            content.Append("<tr>");
            content.Append("<td style='width:13%;'>");
            content.Append("<table style='width:100%;border-collapse: collapse;' cellspacing='0'>");
            content.Append("<tr>").Append("<td align=center style='padding: 10px;border: solid 1px #ddd;background-color: #f4f6fa;'>");
            content.Append("<strong><span style='font-family: arial;'>").Append("Tuần ").Append(source[0].Week).Append("</span></strong>");
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td align=center style='padding: 10px;border: solid 1px #ddd;'>");
            content.Append("<span style='font-family: arial;'>").Append(source[0].FromDate.Year).Append("</span>");
            content.Append("</td>").Append("</tr>");
            content.Append("</table>");
            content.Append("</td>");
            content.Append("<td>");
            content.Append("<table style='width:100%;border-collapse: collapse; ' cellspacing='0'> ");
            content.Append("<tr>").Append("<td align=center style='line-height: 30px; padding:4px 4px 4px 10px;'>");
            content.Append("<strong><span style='font-size:14pt; font-family: arial;'>").Append("LỊCH CÔNG TÁC TUẦN ").Append(source[0].Week).Append("</span></strong>");
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td align=center style='line-height: 30px; padding:4px 4px 4px 10px;'>");
            content.Append("<strong> <span style='font-size:12pt; font-family: arial;'>").Append(deparment.Name.ToUpper()).Append("</span></strong>");
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td align=center style='line-height: 30px; padding:4px 4px 4px 10px;'>");
            content.Append("<i style='font-family: arial;'>").Append(string.Format("Áp dụng từ {0:dd/MM/yyyy} ngày đến {1:dd/MM/yyyy}", source[0].FromDate, source[0].ToDate)).Append("</i>");
            content.Append("</td>").Append("</tr>");
            content.Append("</table>");
            content.Append("</td>");
            content.Append("</tr>");
            content.Append("</table> <br /> ");
            foreach (var item in source)
            {
                content.Append("<table style='width:100%;border-collapse: collapse;font-family: arial;' cellspacing='0'> ");
                content.Append("<tr><td align=center style='line-height: 35px; padding:10px 4px 10px 4px;'>");
                content.Append("<strong><span style='font-family: arial;'>").Append(item.Title).Append("</span></strong>");
                content.Append("</td></tr>");
                content.Append("<tr><td>");
                foreach (var lead in item.EventLeaders)
                {
                    content.Append("<table style='width:100%;border-collapse: collapse;font-family: arial;' cellspacing='0'>");
                    content.Append("<tr>");
                    content.Append("<td rowspan='2'  style='width:30%;border-style: solid solid solid solid; line-height: 30px; padding:4px 4px 4px 10px;' align=center>");
                    content.Append("<span style='font-family: arial;'>").Append(lead.Position).Append("<br />").Append("</span>");
                    content.Append("<strong><span style='font-family: arial;'>").Append(lead.LeaderFullName).Append("</span></strong>");
                    content.Append("</td>");

                    content.Append("<td style='width:7%;border-style: solid solid solid solid; line-height: 30px; padding:4px 4px 4px 10px;'><span style='font-family: arial;'>Sáng</span>");
                    content.Append("</td>");
                    content.Append("<td style='border-style: solid solid solid solid; line-height: 30px; padding:4px 4px 4px 10px;'>");
                    foreach (var mo in lead.ContentMornings)
                    {
                        content.Append("<span style='font-family: arial;'>");
                        content.Append(mo.Content);
                        content.Append("</span>");
                    }
                    content.Append("</td>");
                    content.Append("</tr>");
                    content.Append("<tr>");


                    content.Append("</td>");

                    content.Append("<td style='border-style: solid solid solid solid; line-height: 30px; padding:4px 4px 4px 10px;'><span style='font-family: arial;'>Chiều</span>");

                    content.Append("</td>");
                    content.Append("<td style='border-style: solid solid solid solid; line-height: 30px; padding:4px 4px 4px 10px;'>");
                    foreach (var af in lead.ContentAfternoons)
                    {
                        content.Append("<span style='font-family: arial;'>");
                        content.Append(af.Content);
                        content.Append("</span>");
                    }
                    content.Append("</td>");

                    content.Append("</tr>");
                    content.Append("</table>");
                }

                content.Append("</td></tr>");
                content.Append("</table>");
            }

            content.Append("</body></html>");

            response.Value = Util.CreateDocument(content.ToString(), "leader_event", "Demo1", 20);
            return response;
        }
        #endregion Event

        #region EventUserNotify

        public BaseResponse<EventUserNotify> AddEventUserNotify(EventUserNotify model)
        {
            var response = new BaseResponse<EventUserNotify>();
            var errors = Validate<EventUserNotify>(model, new EventUserNotifyValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<EventUserNotify> errResponse = new BaseResponse<EventUserNotify>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _eventUserNotifyRepository.Add(model);
                _applicationLoggingRepository.Log("EVENT", "CREATE", "EventUserNotify", response.Value.EventId.ToString() + ',' + response.Value.UserId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
            }
            catch (Exception ex)
            {
                _applicationLoggingRepository.Log("ERROR", "CREATE", "EventUserNotify", response.Value.EventId.ToString() + ',' + response.Value.UserId, "", "", model, ex.Message, System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseListResponse<EventUserNotify> AddEventUserNotifies(List<EventUserNotify> lstEventUserNotifies)
        {
            var response = new BaseListResponse<EventUserNotify>();
            List<EventUserNotify> models = new List<EventUserNotify>();
            foreach (var model in lstEventUserNotifies)
            {
                var errors = Validate<EventUserNotify>(model, new EventUserNotifyValidator());
                if (errors.Count() > 0)
                {
                    BaseListResponse<EventUserNotify> errResponse = new BaseListResponse<EventUserNotify>();
                    errResponse.IsSuccess = false;
                    return errResponse;
                }
            }
            foreach (var model in lstEventUserNotifies)
            {
                try
                {
                    model.CreatedOn = DateTime.Now;
                    models.Add(_eventUserNotifyRepository.Add(model));
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "CREATE", "EventUserNotify", model.EventId.ToString() + ',' + model.UserId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch (Exception ex)
                    { }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                }
            }
            return response;
        }

        public BaseResponse DeleteEventUserNotifiesByEvent(CustomEvent model)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                _eventUserNotifyRepository.DeleteMulti(x => x.EventId == model.EventId);
                _applicationLoggingRepository.Log("EVENT", "DELETE", "EventUserNotify", model.EventId.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
            }
            catch (Exception ex)
            {
                _applicationLoggingRepository.Log("ERROR", "DELETE", "EventUserNotify", model.EventId.ToString(), "", "", model, ex.Message, System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
            }
            return response;
        }

        #endregion EventUserNotify

        #region Importaint Job

        public BaseResponse<ImportantJob> GetImportantJobById(int id)
        {
            var response = new BaseResponse<ImportantJob>();
            try
            {
                response.Value = _importantJobRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }

        public BaseResponse<ImportantJob> AddImportantJob(ImportantJob model)
        {
            var response = new BaseResponse<ImportantJob>();
            var errors = Validate<ImportantJob>(model, new ImportantJobValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<ImportantJob> errResponse = new BaseResponse<ImportantJob>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var objEdit = _importantJobRepository.GetImportantJob(model.DepartmentId, model.StartDate.Value, model.EndDate.Value, model.Note);
                if (objEdit != null)
                {
                    var update = _importantJobRepository.GetById(objEdit.Id);
                    update.Content = model.Content;
                    update.EditedBy = model.EditedBy;
                    update.EditedOn = DateTime.Now;
                    response.Value = _importantJobRepository.Edit(update);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "ImportantJob", response.Value.Id.ToString(), "", "", response.Value, "", HttpContext.Current.Request.UserHostAddress, response.Value.CreatedBy);
                    }
                    catch { }
                }
                else
                {

                    model.CreatedOn = DateTime.Now;
                    response.Value = _importantJobRepository.Add(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "CREATE", "ImportantJob", response.Value.Id.ToString(), "", "", response.Value, "", HttpContext.Current.Request.UserHostAddress, response.Value.CreatedBy);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<ImportantJob> EditImportantJob(ImportantJob model)
        {
            BaseResponse<ImportantJob> response = new BaseResponse<ImportantJob>();
            var errors = Validate<ImportantJob>(model, new ImportantJobValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<ImportantJob> errResponse = new BaseResponse<ImportantJob>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var objEdit = _importantJobRepository.GetImportantJob(model.DepartmentId, model.StartDate.Value, model.EndDate.Value, model.Note);
                if (objEdit != null)
                {

                    if (objEdit.Id != model.Id)
                    {
                        var update = _importantJobRepository.GetById(objEdit.Id);
                        update.Deleted = true;
                        update.EditedOn = DateTime.Now;
                        update.EditedBy = model.EditedBy;
                        _importantJobRepository.Edit(update);
                        try
                        {
                            _applicationLoggingRepository.Log("EVENT", "DELETE", "ImportantJob", update.Id.ToString(), "", "", update, "", HttpContext.Current.Request.UserHostAddress, update.CreatedBy);
                        }
                        catch { }
                    }
                    model.EditedOn = DateTime.Now;
                    response.Value = _importantJobRepository.Edit(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "ImportantJob", response.Value.Id.ToString(), "", "", response.Value, "", HttpContext.Current.Request.UserHostAddress, response.Value.CreatedBy);
                    }
                    catch { }
                }
                else
                {
                    model.EditedOn = DateTime.Now;
                    response.Value = _importantJobRepository.Edit(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "ImportantJob", response.Value.Id.ToString(), "", "", response.Value, "", HttpContext.Current.Request.UserHostAddress, response.Value.CreatedBy);
                    }
                    catch { }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse DeleteLogicalImportantJob(int id)
        {

            BaseResponse response = new BaseResponse();
            ImportantJob model = _importantJobRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _importantJobRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "ImportantJob", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }


        #endregion

        #region EventGoogle
        public BaseResponse<EventGoogleEvent> AddEventGoogle(EventGoogleEvent model)
        {
            var response = new BaseResponse<EventGoogleEvent>();
            var errors = Validate<EventGoogleEvent>(model, new EventGoogleEventValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<EventGoogleEvent> errResponse = new BaseResponse<EventGoogleEvent>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _eventGoogleEventRepository.Add(model);
                _applicationLoggingRepository.Log("EVENT", "CREATE", "EventGoogleEvent", response.Value.EventId.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        #endregion
    }
}