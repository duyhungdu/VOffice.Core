using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;

namespace VOffice.API.Controllers.Api.Organization
{
    /// <summary>
    /// Department API. An element of SystemConfig Deparment Service
    /// </summary>
    [Authorize]
    public class DepartmentController : ApiController
    {
        IOrganizationService organizationService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_organizationService"></param>
        public DepartmentController(IOrganizationService _organizationService)
        {
            organizationService = _organizationService;

        }
        /// <summary>
        /// Get a list of Deparment via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDepartment_Result> Search([FromUri] DepartmentQuery query)
        {
            return organizationService.FilterDepartment(query);
        }
        /// <summary>
        /// Get a list of Deparment via SQL Store for Document
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPBuildOrganizationTree_Result> BuildOrganizationTree([FromUri] DepartmentQuery query)
        {
            return organizationService.BuildOrganizationTree(query);
        }
        /// <summary>
        /// Get All Deparments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<Department> GetAll()
        {
            BaseListResponse<Department> result = organizationService.GetAllDepartment();
            return result;
        }
        /// <summary>
        /// Get All Deparments by userId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<Department> GetListDepartmentByUserId(string userId)
        {
            BaseListResponse<Department> result = organizationService.GetListDepartmentByUserId(userId);
            return result;
        }
        /// <summary>
        /// Insert a Deparment to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<Department> Add(Department model)
        {
            return organizationService.AddDepartment(model);
        }
        /// <summary>
        /// Update a Deparment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<Department> Update(Department model)
        {
            return organizationService.UpdateDepartment(model);
        }
        /// <summary>
        /// Get a list of Deparment not include itself via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDepartment_Result> GetSubDepartmentNonSeft([FromUri] DepartmentQuery query)
        {
            return organizationService.GetSubDepartmentNonSeft(query);
        }

        /// <summary>
        /// Get a list of Deparment follow keyword in form organiz via SQL Store
        /// </summary>
        /// <param name="type"></param>
        /// <param name="departmentId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]

        [Authorize(Roles = "department")]
        public BaseListResponse<SPGetDepartmentOrganiz_Result> FilterDepartmentOrganiz(int type, int departmentId, string keyword)
        {
            return organizationService.FilterDepartmentOrganiz(type, departmentId, keyword);
        }
        /// <summary>
        /// Insert a Deparment to Database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<Department> Get(int id)
        {
            return organizationService.GetDepartmentById(id);
        }
        /// <summary>
        /// Delete Logical a Deparment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return organizationService.DeleteLogicalDepartment(id);
        }
        /// <summary>
        /// Get All Deparments by staffId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetListDepartmentByStaffId_Result> GetListDepartmentByStaffId(int staffId)
        {
            BaseListResponse<SPGetListDepartmentByStaffId_Result> result = organizationService.GetListDepartmentByStaffId(staffId);
            return result;
        }
    }

}