using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;

namespace VOffice.API.Controllers.Api.Calendar
{
    /// <summary>
    /// Calendar service
    /// </summary>
    [Authorize]
    public class EventController : ApiController
    {
        ICalendarService calendarService;
        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_calendarService"></param>
        public EventController(ICalendarService _calendarService)
        {
            calendarService = _calendarService;
        }
        /// <summary>
        /// Get a Event by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "calendar")]
        public BaseResponse<ComplexEvent> Get(int id)
        {
            return calendarService.GetEventById(id);
        }
        /// <summary>
        /// Insert a Event to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "registerevent")]
        public BaseResponse<Event> Add(Event model)
        {
            return calendarService.AddEvent(model);
        }
        /// <summary>
        /// Add complex event calendar
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<Event> AddComplexEvent(ComplexEvent model)
        {
            return calendarService.AddComplexEvent(model);
        }
        /// <summary>
        /// Update a Event to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "editevent")]
        public BaseResponse<Event> Update(Event model)
        {
            return calendarService.UpdateEvent(model);
        }
        /// <summary>
        /// Update complex event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "editevent")]
        public BaseResponse<Event> UpdateComplexEvent(ComplexEvent model)
        {
            return calendarService.UpdateComplexEvent(model);
        }
        /// <summary>
        /// Udpate None Authorize event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<Event> UpdateNonAuthorizeComplexEvent(ComplexEvent model)
        {
            return calendarService.UpdateComplexEvent(model);
        }
        /// <summary>
        /// Mark a Event as Deleted
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "deleteevent")]
        public BaseResponse DeleteLogical(int eventId)
        {
            return calendarService.DeleteLogicalEvent(eventId);
        }

        /// <summary>
        /// delete event non authorize
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogicalNonAuthorize(int eventId)
        {
            return calendarService.DeleteLogicalEvent(eventId);
        }
        /// <summary>
        /// Deferred an event
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "cancelevent")]
        public BaseResponse DeferredEvent(CustomEventDeferred input)
        {
            return calendarService.DeferredEvent(input);
        }
        /// <summary>
        /// Accepted an event
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "acceptevent")]
        public async System.Threading.Tasks.Task<BaseResponse> AcceptedEvent(CustomEventAccepted input)
        {
            return await calendarService.AcceptedEvent(input);
        }
        /// <summary>
        /// Get Event of Department
        /// </summary>
        /// <param name="type"></param>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <param name="morning"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetEventOfDepartment_Result> GetEventOfDepartment(int type, int departmentId, string userId, bool morning, DateTime date)
        {
            return calendarService.GetEventOfDepartment(type, departmentId, userId, morning, date);
        }

        /// <summary>
        /// get event accepted
        /// </summary>
        /// <param name="type"></param>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <param name="morning"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetEventOfDepartment_Result> GetEventAcceptedOfDepartment(int type, int departmentId, string userId, bool morning, DateTime date)
        {
            return calendarService.GetEventAcceptedOfDepartment(type, departmentId, userId, morning, date);
        }
        /// <summary>
        /// Insert EventNotify to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<EventUserNotify> AddEventUserNotify(EventUserNotify model)
        {
            return calendarService.AddEventUserNotify(model);
        }
        /// <summary>
        /// Insert list EventNotifies to Database
        /// </summary>
        /// <param name="lstEventUserNotifies"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseListResponse<EventUserNotify> AddEventUserNotifies(List<EventUserNotify> lstEventUserNotifies)
        {
            return calendarService.AddEventUserNotifies(lstEventUserNotifies);
        }

        /// <summary>
        /// Delete list eventNotifies by eventId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse DeleteEventUserNotifiesByEvent(CustomEvent model)
        {
            return calendarService.DeleteEventUserNotifiesByEvent(model);
        }
        /// <summary>
        /// Get event of deparment for week
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<CustomEventOfWeek> GetDeparmentEventOfWeek(int departmentId, DateTime date)
        {
            return calendarService.GetDepartmentEventOfWeek(departmentId, date);
        }

        /// <summary>
        /// Get event of multi deparment for week
        /// </summary>
        /// <param name="listDepartmentId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<CustomEventOfWeek> GetMultiDeparmentEventOfWeek(string listDepartmentId, DateTime date)
        {
            return calendarService.GetMultiDepartmentEventOfWeek(listDepartmentId, date);
        }

        /// <summary>
        /// Get department leader of week
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<CustomDayEventOfWeek> GetDeparmentLeaderEventOfWeek(int departmentId, DateTime date)
        {
            return calendarService.GetDepartmentLeaderEventOfWeek(departmentId, date);
        }
        /// <summary>
        /// Add to google event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<EventGoogleEvent> AddEventGoogle(EventGoogleEvent model)
        {
            return calendarService.AddEventGoogle(model);
        }
        /// <summary>
        /// get event doc
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<string> GetEventFile(int departmentId, DateTime date, bool accepted, string userId)
        {
            return calendarService.GetEventFileDownLoad(departmentId, date, accepted,userId);
        }
        /// <summary>
        /// get event doc
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<string> GetLeaderEventFile(int departmentId, DateTime date)
        {
            return calendarService.GetLeaderEventFileDownLoad(departmentId, date);
        }

    }
}
