using System;
using System.Linq;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Model.Validators;
using VOffice.Repository;
using VOffice.Repository.Queries;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
namespace VOffice.ApplicationService
{
    public partial class OrganizationService : BaseService, IOrganizationService
    {
        protected readonly DepartmentRepository _departmentRepository;
        protected readonly StaffRepository _staffRepository;
        protected readonly ApplicationLoggingRepository _applicationLoggingRepository;
        protected readonly SystemConfigDepartmentRepository _systemConfigDepartmentRepository;
        protected readonly DepartmentStaffsRepository _departmentStaffsRepository;
        AspNetUsersRepository _userRepository;
        public OrganizationService()
        {
            _departmentRepository = new DepartmentRepository();
            _staffRepository = new StaffRepository();
            _applicationLoggingRepository = new ApplicationLoggingRepository();
            _systemConfigDepartmentRepository = new SystemConfigDepartmentRepository();
            _departmentStaffsRepository = new DepartmentStaffsRepository();
            _userRepository = new AspNetUsersRepository();
        }

        #region Department

        public BaseResponse<Department> GetDepartmentById(int id)
        {
            var response = new BaseResponse<Department>();
            try
            {
                response.Value = _departmentRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }

        public BaseListResponse<SPGetDepartment_Result> FilterDepartment(DepartmentQuery query)
        {
            var response = new BaseListResponse<SPGetDepartment_Result>();
            try
            {
                response.Data = _departmentRepository.Filter(query);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseListResponse<SPBuildOrganizationTree_Result> BuildOrganizationTree(DepartmentQuery query)
        {
            var response = new BaseListResponse<SPBuildOrganizationTree_Result>();
            try
            {
                response.Data = _departmentRepository.BuildOrganizationTree(query);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseListResponse<Department> GetAllDepartment()
        {
            var response = new BaseListResponse<Department>();
            try
            {
                var result = _departmentRepository.GetAll().Where(x => x.Deleted == false).ToList();
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
        public BaseListResponse<Department> GetListDepartmentByUserId(string userId)
        {
            var response = new BaseListResponse<Department>();
            try
            {
                var result = _departmentRepository.GetListDepartmentsByUserId(userId);
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

        public BaseResponse<Department> AddDepartment(Department model)
        {
            var response = new BaseResponse<Department>();
            var errors = Validate<Department>(model, new DepartmentValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Department> errResponse = new BaseResponse<Department>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var lstDepartment = _departmentRepository.GetAll().Where(n => n.Code.ToLower() == model.Code.ToLower()).ToList();
                if (lstDepartment.Count() == 0)
                {
                    model.CreatedOn = DateTime.Now;
                    response.Value = _departmentRepository.Add(model);
                    _applicationLoggingRepository.Log("EVENT", "ADD", "Department", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                else
                {
                    response.Message = "Mã đơn vị đã tồn tại";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<Department> UpdateDepartment(Department model)
        {
            BaseResponse<Department> response = new BaseResponse<Department>();
            var errors = Validate<Department>(model, new DepartmentValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Department> errResponse = new BaseResponse<Department>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var department = _departmentRepository.GetById(model.Id);
                if (department.Code != model.Code)
                {
                    IEnumerable<Department> lstDepartment = _departmentRepository.FindBy(x => x.Code.ToLower() == model.Code.ToLower());
                    if (lstDepartment.Count() > 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Mã đơn vị đã tồn tại";
                    }
                    else
                    {
                        model.EditedOn = DateTime.Now;
                        response.Value = _departmentRepository.Update(department, model);
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "Department", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                }
                else
                {
                    model.EditedOn = DateTime.Now;
                    response.Value = _departmentRepository.Update(department, model);
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "Department", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse DeleteLogicalDepartment(int id)
        {
            BaseResponse response = new BaseResponse();
            Department model = _departmentRepository.GetById(id);
            try
            {
                DepartmentQuery query = new DepartmentQuery();
                query.Keyword = "";
                query.Active = null;
                query.ParentId = id;

                BaseListResponse<SPGetDepartment_Result> lstChild = GetSubDepartmentNonSeft(query);
                var lstStaffs = _staffRepository.SearchStaffs(id, 1, "", null);
                if (lstChild.Data.Count() > 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Tồn tại đơn vị con. Xóa không thành công";
                }
                else if (lstStaffs.Count() > 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Tồn tại cán bộ thuộc đơn vị. Xóa không thành công";
                }
                else
                {
                    model.EditedOn = DateTime.Now;
                    model.Deleted = true;
                    _departmentRepository.Edit(model);
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "Department", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);

                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetDepartment_Result> GetSubDepartmentNonSeft(DepartmentQuery query)
        {
            var response = new BaseListResponse<SPGetDepartment_Result>();
            try
            {
                response.Data = _departmentRepository.Filter(query).Where(n => n.Id != query.ParentId).OrderBy(n => n.Order).ToList();
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetDepartmentOrganiz_Result> GetDepartmentOrganiz(int type, int departmentId, string keyword)
        {
            var response = new BaseListResponse<SPGetDepartmentOrganiz_Result>();
            try
            {
                response.Data = _departmentRepository.GetDepartmentOrganiz(type, departmentId, keyword).ToList();
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public bool checkDepartment(SPGetDepartmentOrganiz_Result obj, List<SPGetDepartmentOrganiz_Result> result)
        {
            int count = 0;
            foreach (var item in result)
            {
                if (obj.IdData != item.IdData)
                    count++;
            }
            if (count == result.Count())
                return true;
            else return false;
        }

        public BaseListResponse<SPGetDepartmentOrganiz_Result> FilterDepartmentOrganiz(int type, int departmentId, string keyword)
        {
            var response = new BaseListResponse<SPGetDepartmentOrganiz_Result>();
            List<SPGetDepartmentOrganiz_Result> lstDepartmentFirst = new List<SPGetDepartmentOrganiz_Result>();
            List<SPGetDepartmentOrganiz_Result> lstDepartmentSecond;
            List<SPGetDepartmentOrganiz_Result> result = new List<SPGetDepartmentOrganiz_Result>();
            keyword = keyword == null ? "" : keyword;
            try
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    lstDepartmentFirst = _departmentRepository.GetDepartmentOrganiz(type, departmentId, keyword).ToList();
                    if (lstDepartmentFirst.Count() > 0)
                    {
                        foreach (var item in lstDepartmentFirst)
                        {
                            lstDepartmentSecond = new List<SPGetDepartmentOrganiz_Result>();
                            lstDepartmentSecond = _departmentRepository.GetDepartmentOrganiz(2, int.Parse(item.IdData), keyword).ToList();
                            if (lstDepartmentSecond.Count() > 0)
                            {
                                foreach (var obj in lstDepartmentSecond)
                                {
                                    if (checkDepartment(obj, result))
                                    {
                                        result.Add(obj);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    result = _departmentRepository.GetDepartmentOrganiz(type, departmentId, keyword).ToList();
                }
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetListDepartmentByStaffId_Result> GetListDepartmentByStaffId(int staffId)
        {
            var response = new BaseListResponse<SPGetListDepartmentByStaffId_Result>();
            try
            {
                response.Data = _departmentRepository.GetListDepartmentByStaffId(staffId).ToList();
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        #endregion Department

        #region Staff
        public BaseListResponse<SPGetStaffByDepartmentId_Result> GetStaffByDepartmentId(int departmentId)
        {
            var response = new BaseListResponse<SPGetStaffByDepartmentId_Result>();
            try
            {

                response.Data = _staffRepository.GetStaffByDepartmentId(departmentId);
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseListResponse<SPGetStaffNoAccountByDepartmentId_Result> GetStaffNoAccountByDepartmentId(int departmentId)
        {
            var response = new BaseListResponse<SPGetStaffNoAccountByDepartmentId_Result>();
            try
            {
                response.Data = _staffRepository.GetStaffNoAccountByDepartmentId(departmentId);
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<UserDepartment> GetStaffProfile(string userId)
        {
            var response = new BaseResponse<UserDepartment>();
            try
            {
                response.IsSuccess = true;
                response.Value = _departmentRepository.GetUserMainOrganization(userId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPSearchStaff_Result> SearchStaffs(int departmentId, int parentId, string keyword, bool? active)
        {
            var response = new BaseListResponse<SPSearchStaff_Result>();
            try
            {
                response.Data = _staffRepository.SearchStaffs(departmentId, parentId, keyword, active);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<Staff> GetStaffById(int id)
        {
            var response = new BaseResponse<Staff>();
            try
            {
                response.Value = _staffRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<Staff> UpdateStaff(Staff model)
        {
            BaseResponse<Staff> response = new BaseResponse<Staff>();
            var errors = Validate<Staff>(model, new StaffValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Staff> errResponse = new BaseResponse<Staff>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _staffRepository.Edit(model);
                _applicationLoggingRepository.Log("EVENT", "UPDATE", "Staff", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<Staff> UpdateStaffGeneralCalendar(Staff model)
        {
            BaseResponse<Staff> response = new BaseResponse<Staff>();
            try
            {
                if (model.UserId != null)
                {
                    var staffEdit = _staffRepository.FindBy(x => x.UserId == model.UserId).FirstOrDefault();
                    if (staffEdit != null)
                    {
                        staffEdit.GeneralCalendar = model.GeneralCalendar;
                        staffEdit.EditedOn = DateTime.Now;
                        response.Value = _staffRepository.Edit(staffEdit);
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
        public BaseResponse<Staff> GetStaffGeneralCalendar(string userid)
        {
            BaseResponse<Staff> response = new BaseResponse<Staff>();
            try
            {

                var staffEdit = _staffRepository.FindBy(x => x.UserId == userid).FirstOrDefault();
                if (staffEdit != null)
                {
                    response.Value = staffEdit;
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }

            return response;
        }
        public BaseResponse<Staff> UpdateStaffAccount(Staff model)
        {
            BaseResponse<Staff> response = new BaseResponse<Staff>();
            var errors = Validate<Staff>(model, new StaffValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Staff> errResponse = new BaseResponse<Staff>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var oldStaff = _staffRepository.GetStaffByUserId(model.UserId);
                model.EditedOn = DateTime.Now;
                response.Value = _staffRepository.Edit(model);
                if (oldStaff != null)
                {
                    if (oldStaff.Id != model.Id)
                    {
                        Staff oldStaffModel = _staffRepository.GetById(oldStaff.Id);
                        oldStaffModel.UserId = null;
                        oldStaffModel.EditedOn = DateTime.Now;
                        oldStaffModel.EditedBy = model.EditedBy;
                        _staffRepository.Edit(oldStaffModel);
                    }
                }

                _applicationLoggingRepository.Log("EVENT", "UPDATE", "Staff", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<SPGetStaffByUserId_Result> GetStaffByUserId(string userId)
        {
            var response = new BaseResponse<SPGetStaffByUserId_Result>();
            try
            {
                response.IsSuccess = true;
                response.Value = _staffRepository.GetStaffByUserId(userId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<Staff> GetAllStaff()
        {
            var response = new BaseListResponse<Staff>();
            try
            {
                response.IsSuccess = true;
                response.Data = _staffRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<int> AddStaff(ComplexStaff model)
        {
            Staff obj = new Staff();
            obj.StaffCode = model.StaffCode;
            obj.FirstName = model.FirstName;
            obj.LastName = model.LastName;
            obj.FullName = model.FullName;
            obj.UserId = model.UserId;
            obj.Email = model.Email;
            obj.PhoneNumber = model.PhoneNumber;
            obj.DateOfBirth = model.DateOfBirth;
            obj.Gender = model.Gender;
            obj.Address = model.Address;
            obj.Avatar = model.Avatar;
            obj.Order = model.Order;
            obj.Leader = model.Leader;
            obj.SeniorLeader = model.SeniorLeader;
            obj.SuperLeader = model.SuperLeader;
            obj.SignedBy = model.SignedBy;
            obj.Active = model.Active;
            obj.Deleted = model.Deleted;
            obj.CreatedBy = model.CreatedBy;
            obj.CreatedOn = model.CreatedOn;
            obj.EditedBy = model.EditedBy;
            obj.EditedOn = model.EditedOn;

            var response = new BaseResponse<int>();
            var responseTemp = new BaseResponse<Staff>();
            #region Generate TaskCode
            string taskCode = "";
            string taskCodeText = "";
            string taskCodeNumber = "";
            try
            {
                var systemConfigDepartmentQuery = new SystemConfigDepartmentQuery();
                systemConfigDepartmentQuery.DepartmentId = model.DepartmentId;
                systemConfigDepartmentQuery.DefaultValue = "CB";
                systemConfigDepartmentQuery.Title = "STAFFCODE";
                taskCodeText = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(systemConfigDepartmentQuery);
                systemConfigDepartmentQuery.DefaultValue = "0";
                systemConfigDepartmentQuery.Title = "STAFFNUMBER";
                string taskCodeNumberConfig = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(systemConfigDepartmentQuery);
                taskCodeNumber = (int.Parse(taskCodeNumberConfig) + 1).ToString("D4");
                taskCode = taskCodeText + taskCodeNumber;
            }
            catch
            {
            }
            #endregion Generate TaskCode
            try
            {
                obj.CreatedOn = DateTime.Now;
                responseTemp.Value = _staffRepository.Add(obj);
                response.Value = responseTemp.Value.Id;
                _applicationLoggingRepository.Log("EVENT", "ADD", "Staff", "", "", "", obj, "", HttpContext.Current.Request.UserHostAddress, obj.CreatedBy);
                //Update SystemConfigDepartment
                SystemConfigDepartment taskCodeNumberSystemConfigDepartment = new SystemConfigDepartment();
                taskCodeNumberSystemConfigDepartment = _systemConfigDepartmentRepository.GetByTitle("STAFFNUMBER", model.DepartmentId);
                if (taskCodeNumberSystemConfigDepartment != null)
                {
                    taskCodeNumberSystemConfigDepartment.Value = taskCodeNumber;
                    _systemConfigDepartmentRepository.Edit(taskCodeNumberSystemConfigDepartment);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalStaff(int id)
        {
            BaseResponse response = new BaseResponse();
            Staff model = _staffRepository.GetById(id);
            try
            {
                AspNetUser user = _userRepository.GetById(model.UserId);
                if(user!=null)
                {
                    user.GoogleAccount = user.UserName;
                    user.UserName = user.Id;
                    user.Email = user.Id;
                    user.Deleted = true;
                    user.EditedOn = DateTime.Now;
                    _userRepository.Edit(user);
                }
                //Xóa cán bộ thì xóa bỏ tài khoản được gán cho cán bộ
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _staffRepository.Edit(model);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetBirthDayByDepartmentId_Result> GetBirthDayByDepartment(int departmentId)
        {
            var response = new BaseListResponse<SPGetBirthDayByDepartmentId_Result>();
            try
            {
                response.IsSuccess = true;
                response.Data = _staffRepository.GetBirthDayByDeparmentId(departmentId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetSeniorLeaderStaff_Result> GetSeniorLeaderStaff(int departmentId)
        {
            var response = new BaseListResponse<SPGetSeniorLeaderStaff_Result>();
            try
            {
                response.IsSuccess = true;
                response.Data = _staffRepository.GetSeniorLeaderStaff(departmentId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseListResponse<SPGETStaffNonOrExtraDepartment_Result> GetStaffNonOrExtraDepartment()
        {
            var response = new BaseListResponse<SPGETStaffNonOrExtraDepartment_Result>();
            try
            {
                response.IsSuccess = true;
                response.Data = _staffRepository.GetStaffNonOrExtraDepartment();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        #endregion Staff

        #region Department-Staff
        public BaseResponse<DepartmentStaff> AddDepartmentStaff(DepartmentStaff model)
        {
            var response = new BaseResponse<DepartmentStaff>();
            var errors = Validate<DepartmentStaff>(model, new DepartmentStaffsValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<DepartmentStaff> errResponse = new BaseResponse<DepartmentStaff>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                if (model.MainDepartment == true)
                {
                    IEnumerable<DepartmentStaff> ds = _departmentStaffsRepository.GetAll().Where(n => n.StaffId == model.StaffId && n.DepartmentId == n.DepartmentId).ToList();
                    if (ds.Count() > 0)
                    {
                        _departmentStaffsRepository.DeleteMulti(x => x.StaffId == model.StaffId && x.DepartmentId == model.DepartmentId);
                    }
                }
                model.CreatedOn = DateTime.Now;
                response.Value = _departmentStaffsRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "ADD", "DepartmentStaff", model.DepartmentId + "," + model.StaffId.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
                //}
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<DepartmentStaff> AddDepartmentStaffs(List<DepartmentStaff> models)
        {
            var response = new BaseResponse<DepartmentStaff>();
            foreach (var model in models)
            {
                var errors = Validate<DepartmentStaff>(model, new DepartmentStaffsValidator());
                if (errors.Count() > 0)
                {
                    BaseResponse<DepartmentStaff> errResponse = new BaseResponse<DepartmentStaff>(model, errors);
                    errResponse.IsSuccess = false;
                    return errResponse;
                }
            }
            try
            {
                ComplexDepartmentStaff staff = new ComplexDepartmentStaff();
                foreach (var model in models)
                {
                    staff.StaffId = model.StaffId;
                    break;
                }
                DeleteDepartmentStaffByStaff(staff);
                foreach (var model in models)
                {
                    AddDepartmentStaff(model);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetDepartmentStaff_Result> GetDepartmentStaff(int type, int departmentId, int staffId)
        {
            var response = new BaseListResponse<SPGetDepartmentStaff_Result>();
            try
            {
                response.Data = _departmentStaffsRepository.GetDepartmentStaff(type, departmentId, staffId);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteDepartmentStaffByStaff(ComplexDepartmentStaff model)
        {
            BaseResponse response = new BaseResponse();
            _departmentStaffsRepository.DeleteMulti(x => x.StaffId == model.StaffId);
            response.IsSuccess = true;
            return response;
        }
        public BaseResponse DeleteDepartmentsStaff(List<ComplexDepartmentOfStaff> models)
        {
            BaseResponse response = new BaseResponse();
            foreach (var model in models)
            {
                _departmentStaffsRepository.DeleteMulti(n => n.DepartmentId == model.DepartmentId && n.StaffId == model.StaffId);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "DepartmentStaff", model.DepartmentId + "," + model.StaffId.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
            }
            return response;
        }

        public BaseResponse<ComplexStaffDepartment> AddStaffsDepartment(List<ComplexStaffDepartment> models)
        {
            var response = new BaseResponse<ComplexStaffDepartment>();
            DepartmentStaff obj = new DepartmentStaff();
            string userId = "";
            int departmentId = 0;
            try
            {
                foreach (var item in models)
                {
                    userId = item.EditedBy;
                    departmentId = item.DepartmentId;
                    DepartmentStaff st = _departmentStaffsRepository.GetAll().FirstOrDefault(n => n.StaffId == item.StaffId && n.DepartmentId == item.DepartmentId);
                    if (st != null)
                    {
                        st.Position = item.Position;
                        st.EditedBy = item.EditedBy;
                        st.EditedOn = DateTime.Now;
                        st.MainDepartment = true;
                        _departmentStaffsRepository.Edit(st);
                    }
                    else
                    {
                        obj.StaffId = item.StaffId;
                        obj.DepartmentId = item.DepartmentId;
                        obj.MainDepartment = true;
                        obj.ShortPosition = "";
                        obj.Position = item.Position;
                        obj.CreatedOn = DateTime.Now;
                        obj.CreatedBy = item.EditedBy;
                        obj.EditedOn = DateTime.Now;
                        obj.EditedBy = item.EditedBy;
                        _departmentStaffsRepository.Add(obj);
                    }
                }
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "ADD", "DepartmentStaff", departmentId.ToString(), "", "", null, "", HttpContext.Current.Request.UserHostAddress, userId);
                }
                catch
                {

                }
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