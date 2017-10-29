using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;
namespace VOffice.API.Controllers.Api.Document
{
    /// <summary>
    /// DocumentStaff API. An element of DocumentService
    /// </summary>
    [Authorize]
    public class DepartmentStaffController : ApiController
    {
        IOrganizationService organizationService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_organizationService"></param>
        public DepartmentStaffController(IOrganizationService _organizationService)
        {
            organizationService = _organizationService;
        }
        /// <summary>
        /// Insert a DepartmentStaff to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<DepartmentStaff> Add(DepartmentStaff model)
        {
            return organizationService.AddDepartmentStaff(model);
        }
        /// <summary>
        /// Get a list of DeparmentStaff via SQL Store
        /// </summary>
        /// <param name="type"></param>
        /// <param name="departmentId"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDepartmentStaff_Result> GetDepartmentStaff(int type, int departmentId, int staffId)
        {
            return organizationService.GetDepartmentStaff(type, departmentId, staffId);
        }
        /// <summary>
        /// Insert list DepartmentStaff to Database
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<DepartmentStaff> AddDepartmentStaffs(List<DepartmentStaff> models)
        {
            return organizationService.AddDepartmentStaffs(models);
        }
        /// <summary>
        /// Delete list DepartmentStaff by StaffId to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResponse DeleteDepartmentStaffByStaff(ComplexDepartmentStaff model)
        {
            return organizationService.DeleteDepartmentStaffByStaff(model);
        }
        /// <summary>
        /// Delete list DepartmentStaff to Database
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteDepartmentsStaff(List<ComplexDepartmentOfStaff> models)
        {
            return organizationService.DeleteDepartmentsStaff(models);
        }
        /// <summary>
        /// Update staff inside department
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<ComplexStaffDepartment> AddStaffsDepartment(List<ComplexStaffDepartment> models)
        {
            return organizationService.AddStaffsDepartment(models);
        }

    }
}
