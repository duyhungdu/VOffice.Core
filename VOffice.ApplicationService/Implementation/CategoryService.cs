using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Model.Validators;
using VOffice.Repository;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.ApplicationService
{
    public partial class CategoryService : BaseService, ICategoryService
    {
        protected readonly MeetingRoomRepository _meetingRoomRepository;
        protected readonly CustomerRepository _customerRepository;
        protected readonly StatusRepository _statusRepository;
        protected readonly ApplicationLoggingRepository _applicationLoggingRepository;
        protected readonly NoticeRepository _noticeRepository;

        public CategoryService()
        {
            _customerRepository = new CustomerRepository();
            _meetingRoomRepository = new MeetingRoomRepository();
            _statusRepository = new StatusRepository();
            _applicationLoggingRepository = new ApplicationLoggingRepository();
            _noticeRepository = new NoticeRepository();

        }
        #region Customer
        public BaseResponse<Customer> AddCustomer(Customer model)
        {
            var response = new BaseResponse<Customer>();
            var errors = Validate<Customer>(model, new CustomerValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Customer> errResponse = new BaseResponse<Customer>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }

            try
            {
                var listCustomer = _customerRepository.GetAll().Where(x => x.Code.ToLower() == model.Code.ToLower() && x.DepartmentId == model.DepartmentId).ToList();
                if (listCustomer.Count > 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã khách hàng đã tồn tại";
                }
                else
                {
                    model.CreatedOn = DateTime.Now;
                    response.Value = _customerRepository.Add(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "CREATE", "Customer", response.Value.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<Customer> UpdateCustomer(Customer model)
        {
            BaseResponse<Customer> response = new BaseResponse<Customer>();
            var errors = Validate<Customer>(model, new CustomerValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Customer> errResponse = new BaseResponse<Customer>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var customer = _customerRepository.GetById(model.Id);
                if (customer.Code != model.Code)
                {
                    IEnumerable<Customer> listCustomer = _customerRepository.FindBy(x => x.Code.ToLower() == model.Code.ToLower() && x.DepartmentId == model.DepartmentId && x.Deleted == false);
                    if (listCustomer.Count() > 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Mã khách hàng đã tồn tại.";
                    }
                    else
                    {
                        model.EditedOn = DateTime.Now;
                        response.Value = _customerRepository.Update(customer, model);
                        try
                        {
                            _applicationLoggingRepository.Log("EVENT", "UPDATE", "Customer", response.Value.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                        }
                        catch
                        { }
                    }
                }
                else
                {
                    model.EditedOn = DateTime.Now;
                    response.Value = _customerRepository.Update(customer, model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "Customer", response.Value.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalCustomer(int id)
        {
            BaseResponse response = new BaseResponse();
            Customer model = _customerRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _customerRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "Customer", model.Id.ToString(), "", model, "", "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<Customer> GetAllCustomer()
        {
            var response = new BaseListResponse<Customer>();
            try
            {
                var result = _customerRepository.GetAll().Where(x => x.Deleted == false && x.Active == true).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<Customer> GetCustomerById(int id)
        {
            var response = new BaseResponse<Customer>();
            try
            {
                response.Value = _customerRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetCustomer_Result> FilterCustomer(CustomerQuery query)
        {
            var response = new BaseListResponse<SPGetCustomer_Result>();
            int count = 0;
            try
            {
                response.Data = _customerRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetCustomerByDepartmentId_Result> FilterCustomerByDepartmentId(int departmentId, string keyword)
        {
            var response = new BaseListResponse<SPGetCustomerByDepartmentId_Result>();
            try
            {
                var result = _customerRepository.FilterByDepartmentId(departmentId, keyword);
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        #endregion Customer

        #region MeetingRoom
        public BaseResponse<MeetingRoom> GetMeetingRoomById(int id)
        {
            var response = new BaseResponse<MeetingRoom>();
            try
            {
                response.Value = _meetingRoomRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetMeetingRoom_Result> FilterMeetingRoom(MeetingRoomQuery query)
        {
            var response = new BaseListResponse<SPGetMeetingRoom_Result>();
            int count = 0;
            try
            {
                response.Data = _meetingRoomRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<MeetingRoom> GetAllMeetingRoom()
        {
            var response = new BaseListResponse<MeetingRoom>();
            try
            {
                var result = _meetingRoomRepository.GetAll().Where(x => x.Deleted == false && x.Active == true).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<MeetingRoom> AddMeetingRoom(MeetingRoom model)
        {
            var response = new BaseResponse<MeetingRoom>();
            var errors = Validate<MeetingRoom>(model, new MeetingRoomValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<MeetingRoom> errResponse = new BaseResponse<MeetingRoom>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _meetingRoomRepository.Add(model);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<MeetingRoom> UpdateMeetingRoom(MeetingRoom model)
        {
            BaseResponse<MeetingRoom> response = new BaseResponse<MeetingRoom>();
            var errors = Validate<MeetingRoom>(model, new MeetingRoomValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<MeetingRoom> errResponse = new BaseResponse<MeetingRoom>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _meetingRoomRepository.Edit(model);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalMeetingRoom(int id)
        {
            BaseResponse response = new BaseResponse();
            MeetingRoom model = _meetingRoomRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _meetingRoomRepository.Edit(model);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetMeetingRoomByDepartmentId_Result> GetMeetingRoomByDepartmentId(int departmentId)
        {
            var response = new BaseListResponse<SPGetMeetingRoomByDepartmentId_Result>();
            try
            {
                response.Data = _meetingRoomRepository.GetMeetingRoomByDepartmentId(departmentId);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        #endregion MeetingRoom

        #region Status
        public BaseListResponse<SPGetStatus_Result> FilterStatus(StatusQuery query)
        {
            var response = new BaseListResponse<SPGetStatus_Result>();
            int count = 0;
            try
            {
                response.Data = _statusRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<Status> GetStatusByType(string type)
        {
            var response = new BaseListResponse<Status>();
            try
            {
                var result = _statusRepository.GetAll().Where(x => x.Deleted == false && x.Active == true && x.Type == type).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<SPGetStatusByCode_Result> GetStatusByCode(StatusQuery query)
        {
            var response = new BaseResponse<SPGetStatusByCode_Result>();
            try
            {
                response.Value = _statusRepository.GetStatusByCode(query).FirstOrDefault();
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse<Status> GetStatusById(int id)
        {
            var response = new BaseResponse<Status>();
            try
            {
                response.Value = _statusRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<Status> GetAllStatus()
        {
            var response = new BaseListResponse<Status>();
            try
            {
                var result = _statusRepository.GetAll().Where(x => x.Deleted == false).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<Status> AddStatus(Status model)
        {
            var response = new BaseResponse<Status>();
            var errors = Validate<Status>(model, new StatusValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Status> errResponse = new BaseResponse<Status>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var listStatus = _statusRepository.GetAll().Where(x => x.Code.ToLower() == model.Code.ToLower() && x.Type.ToLower() == model.Type.ToLower()).ToList();
                if (listStatus.Count > 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã trạng thái đã tồn tại";
                }
                else
                {
                    model.CreatedOn = DateTime.Now;
                    response.Value = _statusRepository.Add(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "CREATE", "Status", response.Value.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<Status> UpdateStatus(Status model)
        {
            BaseResponse<Status> response = new BaseResponse<Status>();
            var errors = Validate<Status>(model, new StatusValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Status> errResponse = new BaseResponse<Status>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _statusRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "Status", model.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
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
        public BaseResponse DeleteLogicalStatus(int id)
        {
            BaseResponse response = new BaseResponse();
            Status model = _statusRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _statusRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "Status", model.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        #endregion Status

        #region Notice
        public BaseResponse<Notice> GetNoticeById(int id)
        {
            var response = new BaseResponse<Notice>();
            try
            {
                response.Value = _noticeRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse<Notice> AddNotice(Notice model)
        {
            var response = new BaseResponse<Notice>();
            var errors = Validate<Notice>(model, new NoticeValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Notice> errResponse = new BaseResponse<Notice>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _noticeRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "Notice", response.Value.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
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
        public BaseResponse<Notice> UpdateNotice(Notice model)
        {
            BaseResponse<Notice> response = new BaseResponse<Notice>();
            var errors = Validate<Notice>(model, new NoticeValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Notice> errResponse = new BaseResponse<Notice>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _noticeRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "Notice", response.Value.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
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
        public BaseResponse DeleteLogicalNotice(int id)
        {
            BaseResponse response = new BaseResponse();
            Notice model = _noticeRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _noticeRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "Notice", id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetNotice_Result> FilterNotice(NoticeQuery query)
        {
            var response = new BaseListResponse<SPGetNotice_Result>();
            int count = 0;
            try
            {
                response.Data = _noticeRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<Notice> NoticesInTop()
        {
            var response = new BaseListResponse<Notice>();
            try
            {
                var result = _noticeRepository.GetAll().Where(x => x.Deleted == false && x.Active == true && x.StartDate <=DateTime.Now && x.EndDate >=DateTime.Now).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
    } 
        #endregion

}

