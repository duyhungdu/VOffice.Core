using System.Web.Http;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;

namespace VOffice.API.Controllers.Api.Organization
{
    /// <summary>
    /// DocumentType API. An element of DocumentService
    /// </summary>
    public class StaffController : ApiController
    {
        private IOrganizationService _organizationService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="organizationService"></param>
        public StaffController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        /// <summary>
        /// Get a Staff by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<Staff> GetById(int id)
        {
            return _organizationService.GetStaffById(id);
            //return documentService.GetDocumentTypeById(id);
        }

        /// <summary>
        /// Get a list of staffs via SQL Store
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="parentId"></param>
        /// <param name="keyword"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPSearchStaff_Result> SearchStaff(int departmentId, int parentId, string keyword, bool? active)
        {
            return _organizationService.SearchStaffs(departmentId, parentId, keyword, active);
        }

        /// <summary>
        /// Get all Staff by DepartmentId
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetStaffByDepartmentId_Result> GetByDepartment(int departmentId)
        {
            return _organizationService.GetStaffByDepartmentId(departmentId);
        }
        /// <summary>
        /// Get all Staff by DepartmentId
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetStaffNoAccountByDepartmentId_Result> GetStaffNoAccountByDepartment(int departmentId)
        {
            return _organizationService.GetStaffNoAccountByDepartmentId(departmentId);
        }
        /// <summary>
        /// Get all Staffs
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetStaffByDepartmentId_Result> GetAllStaffs(int departmentId)
        {
            return _organizationService.GetStaffByDepartmentId(0);
        }

        /// <summary>
        /// Get All Staff
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<Staff> GetAll()
        {
            return _organizationService.GetAllStaff();
        }

        /// <summary>
        /// Insert a Staff to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<int> Add(ComplexStaff model)
        {
            return _organizationService.AddStaff(model);
        }

        /// <summary>
        /// Update a staff
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<Staff> Update(Staff model)
        {
            return _organizationService.UpdateStaff(model);
        }

        /// <summary>
        /// Update Staff Account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<Staff> UpdateStaffAccount(Staff model)
        {
            return _organizationService.UpdateStaffAccount(model);
        }
        /// <summary>
        /// update general calendar
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<Staff> UpdateStaffGeneralCalendar(Staff model)
        {
            return _organizationService.UpdateStaffGeneralCalendar(model);
        }
        /// <summary>
        /// get staff general user id
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet]
       public  BaseResponse<Staff> GetStaffGeneralCalendar(string userid)
        {
            return _organizationService.GetStaffGeneralCalendar(userid);
        }
        /// <summary>
        /// Mark a Staff as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return _organizationService.DeleteLogicalStaff(id);
        }

        /// <summary>
        /// get staff profile by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<UserDepartment> GetStaffProfile(string userId)
        {
            return _organizationService.GetStaffProfile(userId);
        }

        /// <summary>
        /// Get staff by userid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SPGetStaffByUserId_Result> GetStaffByUserId(string userId)
        {
            return _organizationService.GetStaffByUserId(userId);
        }

        /// <summary>
        /// get birth of day by deparmentid
        /// </summary>
        /// <param name="deparmentId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetBirthDayByDepartmentId_Result> GetBirthDayByDeparmentId(int deparmentId)
        {
            return _organizationService.GetBirthDayByDepartment(deparmentId);
        }

        /// <summary>
        /// Get seniorLeader by departmentId
        /// </summary>
        /// <param name="deparmentId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetSeniorLeaderStaff_Result> GetSeniorLeaderStaff(int deparmentId)
        {
            return _organizationService.GetSeniorLeaderStaff(deparmentId);
        }
        /// <summary>
        /// Get Get Staff Non Or Extra Department
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGETStaffNonOrExtraDepartment_Result> GetStaffNonOrExtraDepartment()
        {
            return _organizationService.GetStaffNonOrExtraDepartment();
        }
    }
}