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
    public class NoticeController : ApiController
    {
        ICategoryService categoryService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_categoryService"></param>
        public NoticeController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;

        }
        /// <summary>
        /// Get a Notice by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<Notice> Get(int id)
        {
            return categoryService.GetNoticeById(id);
        }
        /// <summary>
        /// Insert a Notice to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<Notice> Add(Notice model)
        {
            return categoryService.AddNotice(model);
        }
        /// <summary>
        /// Update a Notice
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<Notice> Update(Notice model)
        {
            return categoryService.UpdateNotice(model);
        }
        /// <summary>
        /// Mark a Notice as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return categoryService.DeleteLogicalNotice(id);
        }
        /// <summary>
        /// Get a list of Notice via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetNotice_Result> Search([FromUri] NoticeQuery query)
        {
            return categoryService.FilterNotice(query);
        }
        /// <summary>
        /// Get a list of Notice display in top via SQL Store
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<Notice> NoticesInTop()
        {
            return categoryService.NoticesInTop();
        }
    }
}