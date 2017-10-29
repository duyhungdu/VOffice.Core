using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;
namespace VOffice.API.Controllers.Api.Share
{
    /// <summary>
    /// SystemConfigDepartment API. An element of SystemConfig Deparment Service
    /// </summary>
    public class SystemConfigDepartmentController : ApiController
    {
        IShareService shareService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_shareService"></param>
        public SystemConfigDepartmentController(IShareService _shareService)
        {
            shareService = _shareService;

        }

        /// <summary>
        /// Get a list of SystemConfig Deparment via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetSystemConfigDepartment_Result> Search([FromUri] SystemConfigDepartmentQuery query)
        {
            return shareService.FilterSystemConfigDepartment(query);
        }
        /// <summary>
        /// Get All SystemConfig Deparment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SystemConfigDepartment> GetAll()
        {
            BaseListResponse<SystemConfigDepartment> result = shareService.GetAllSystemConfigDepartment();
            return result;
        }
        /// <summary>
        /// Insert a SystemConfig Deparment to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<SystemConfigDepartment> Add(SystemConfigDepartment model)
        {
            return shareService.AddSystemConfigDepartment(model);
        }
        /// <summary>
        /// Update a SystemConfig Deparment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<SystemConfigDepartment> Update(SystemConfigDepartment model)
        {
            return shareService.UpdateSystemConfigDepartment(model);
        }
        /// <summary>
        /// Get value of systemconfig for department. query.Title, query.DepartmentId, query.DefaultValue
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SystemConfigDepartment> GetValue([FromUri] SystemConfigDepartmentQuery query)
        {
            return shareService.GetSystemConfigDepartmentValue(query);
        }
        /// <summary>
        /// Get systemconfig of department by configId, departmentId. 
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SystemConfigDepartment> Get(int departmentId, int configId)
        {
            return shareService.GetSystemConfigDepartmentByConfigIdAndDepartmentId(departmentId, configId);
        }
    }
}