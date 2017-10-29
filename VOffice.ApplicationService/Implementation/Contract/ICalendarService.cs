using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.ApplicationService.Implementation.Contract
{
    public interface ICalendarService : IService
    {
        #region Event
        BaseResponse<ComplexEvent> GetEventById(int id);
        BaseResponse<Event> AddEvent(Event model);
        BaseResponse<Event> AddComplexEvent(ComplexEvent model);
        BaseResponse<Event> UpdateComplexEvent(ComplexEvent model);
        BaseResponse<Event> UpdateEvent(Event model);
        BaseResponse DeferredEvent(CustomEventDeferred model);
        System.Threading.Tasks.Task<BaseResponse> AcceptedEvent(CustomEventAccepted model);
        BaseResponse DeleteLogicalEvent(int id);
        BaseListResponse<SPGetEventOfDepartment_Result> GetEventOfDepartment(int type, int departmentId, string userId, bool morning, DateTime date);
        BaseListResponse<SPGetEventOfDepartment_Result> GetEventAcceptedOfDepartment(int type, int departmentId, string userId, bool morning, DateTime date);
        BaseListResponse<CustomEventOfWeek> GetDepartmentEventOfWeek(int deparmentId, DateTime date);
        BaseListResponse<CustomDayEventOfWeek> GetDepartmentLeaderEventOfWeek(int deparmentId, DateTime date);
        BaseListResponse<CustomEventOfWeek> GetMultiDepartmentEventOfWeek(string listDeparmentId, DateTime date);
        BaseResponse<string> GetEventFileDownLoad(int departmentId, DateTime date, bool accepted, string userId);
        BaseResponse<string> GetLeaderEventFileDownLoad(int deparmentId, DateTime date);
        #endregion

        #region EventUserNotify
        BaseResponse<EventUserNotify> AddEventUserNotify(EventUserNotify model);
        BaseListResponse<EventUserNotify> AddEventUserNotifies(List<EventUserNotify> lstEventUserNotifies);
        BaseResponse DeleteEventUserNotifiesByEvent(CustomEvent model);
        #endregion

        #region ImportantJob
        BaseResponse<ImportantJob> GetImportantJobById(int id);
        BaseResponse<ImportantJob> AddImportantJob(ImportantJob model);
        BaseResponse<ImportantJob> EditImportantJob(ImportantJob model);
        BaseResponse DeleteLogicalImportantJob(int id);
        #endregion

        #region EventGoogle
        BaseResponse<EventGoogleEvent> AddEventGoogle(EventGoogleEvent model);
        #endregion
    }
}
