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
    public interface ICategoryService : IService
    {
        #region Customer
        BaseResponse<Customer> GetCustomerById(int id);
        BaseListResponse<Customer> GetAllCustomer();
        BaseListResponse<SPGetCustomer_Result> FilterCustomer(CustomerQuery query);
        BaseListResponse<SPGetCustomerByDepartmentId_Result> FilterCustomerByDepartmentId(int departmentId, string keyword);
        BaseResponse<Customer> AddCustomer(Customer model);
        BaseResponse<Customer> UpdateCustomer(Customer model);
        BaseResponse DeleteLogicalCustomer(int id);

        #endregion Customer

        #region MeetingRoom
        BaseResponse<MeetingRoom> GetMeetingRoomById(int id);
        BaseListResponse<MeetingRoom> GetAllMeetingRoom();
        BaseListResponse<SPGetMeetingRoom_Result> FilterMeetingRoom(MeetingRoomQuery query);
        BaseResponse<MeetingRoom> AddMeetingRoom(MeetingRoom model);
        BaseResponse<MeetingRoom> UpdateMeetingRoom(MeetingRoom model);
        BaseResponse DeleteLogicalMeetingRoom(int id);
        BaseListResponse<SPGetMeetingRoomByDepartmentId_Result> GetMeetingRoomByDepartmentId(int departmentId);
        #endregion MeetingRoom

        #region Status
        BaseListResponse<SPGetStatus_Result> FilterStatus(StatusQuery query);
        BaseListResponse<Status> GetStatusByType(string type);
        BaseResponse<SPGetStatusByCode_Result> GetStatusByCode(StatusQuery query);
        BaseResponse<Status> GetStatusById(int id);
        BaseListResponse<Status> GetAllStatus();
        BaseResponse<Status> AddStatus(Status model);
        BaseResponse<Status> UpdateStatus(Status model);
        BaseResponse DeleteLogicalStatus(int id);
        #endregion Status

        #region Notice
        BaseResponse<Notice> GetNoticeById(int id);
        BaseResponse<Notice> AddNotice(Notice model);
        BaseResponse<Notice> UpdateNotice(Notice model);
        BaseResponse DeleteLogicalNotice(int id);
        BaseListResponse<SPGetNotice_Result> FilterNotice(NoticeQuery query);
        BaseListResponse<Notice> NoticesInTop();

        #endregion
    }
}
