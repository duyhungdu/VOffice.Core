using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;

namespace VOffice.API.Controllers.Api.Category
{
    /// <summary>
    /// Customer API. An element of CategoryService
    /// </summary>
    [Authorize]
    public class StatusController : ApiController
    {
        ICategoryService categoryService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_categoryService"></param>
        public StatusController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }
        /// <summary>
        /// Get a Status by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<Status> Get(int id)
        {
            return categoryService.GetStatusById(id);
        }
        /// <summary>
        /// Get a Status by Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<Status> GetByType(string type)
        {
            return categoryService.GetStatusByType(type);
        }
        /// <summary>
        /// Get a Status by Code
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SPGetStatusByCode_Result> GetByCode([FromUri] StatusQuery query)
        {
            return categoryService.GetStatusByCode(query);
        }
        /// <summary>
        /// Get a list of Status via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetStatus_Result> Search([FromUri] StatusQuery query)
        {
            return categoryService.FilterStatus(query);
        }
        /// <summary>
        /// Get All Status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<Status> GetAll()
        {
            return categoryService.GetAllStatus();
        }
        /// <summary>
        /// Insert a Status to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<Status> Add(Status model)
        {
            return categoryService.AddStatus(model);
        }
        /// <summary>
        /// Update a Status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<Status> Update(Status model)
        {
            return categoryService.UpdateStatus(model);
        }
        /// <summary>
        /// Mark a Status as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return categoryService.DeleteLogicalStatus(id);
        }
    }
}