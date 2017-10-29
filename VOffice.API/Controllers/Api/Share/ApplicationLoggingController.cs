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
    public class ApplicationLoggingController : ApiController
    {
        private IShareService shareService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_shareService"></param>
        public ApplicationLoggingController(IShareService _shareService)
        {
            shareService = _shareService;
        }
        /// <summary>
        /// Get a list of ApplicationLogging via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetApplicationLogging_Result> Search([FromUri] ApplicationLoggingQuery query)
        {
            return shareService.FilterApplicationLogging(query);
        }
        /// <summary>
        /// Get A Application By id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<ApplicationLogging> Get(int id)
        {
            BaseResponse<ApplicationLogging> result = shareService.GetApplicationLoggingById(id);
            return result;
        }

        /// <summary>
        /// Delete ApplicationLogging
        /// </summary>
        /// <param name="applicationLogging"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Delete(ComplexApplicationLogging applicationLogging)
        {
            return shareService.DeleteApplicationLogging(applicationLogging);
        }
    }
}