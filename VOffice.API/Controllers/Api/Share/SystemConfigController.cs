using System.Web.Http;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;

namespace VOffice.API.Controllers.Api.Share
{
    /// <summary>
    /// Customer API. An element of SystemConfig Deparment Service
    /// </summary>
    public class SystemConfigController : ApiController
    {
        private IShareService shareService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_shareService"></param>
        public SystemConfigController(IShareService _shareService)
        {
            shareService = _shareService;
        }

        /// <summary>
        /// Get a list of SystemConfig via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetConfig_Result> Search([FromUri] SystemConfigQuery query)
        {
            return shareService.FilterSystemConfig(query);
        }

        /// <summary>
        /// Get All SystemConfig
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SystemConfig> GetAll()
        {
            BaseListResponse<SystemConfig> result = shareService.GetAllSystemConfig();
            return result;
        }

        /// <summary>
        /// Insert a SystemConfig  to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<SystemConfig> Add(SystemConfig model)
        {
            return shareService.AddSystemConfig(model);
        }

        /// <summary>
        /// Update a SystemConfig Deparment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<SystemConfig> Update(SystemConfig model)
        {
            return shareService.UpdateSystemConfig(model);
        }
        /// <summary>
        /// Get A SystemConfig By Code
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SystemConfig> GetSystemConfigByCode(string code, bool systemConfig)
        {
            BaseResponse<SystemConfig> result = shareService.GetSystemConfigByCode(code, systemConfig);
            return result;
        }
        /// <summary>
        /// Get A SystemConfig By id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SystemConfig> Get(int id)
        {
            BaseResponse<SystemConfig> result = shareService.GetSystemConfigById(id);
            return result;
        }
        /// <summary>
        /// Clone systemConfig  to Database
        /// </summary>
        /// <param name="listDepartmentId"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse CloneSystemConfig(string listDepartmentId)
        {
            return shareService.CloneSystemConfig(listDepartmentId);
        }
        /// <summary>
        /// Delete Logical systemConfig
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return shareService.DeleteLogicalSystemConfig(id);
        }
    }
}