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
    public class LoginHistoryController : ApiController
    {
        private IShareService shareService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_shareService"></param>
        public LoginHistoryController(IShareService _shareService)
        {
            shareService = _shareService;
        }
        /// <summary>
        /// Insert a LoginHistory  to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<LoginHistory> Add(LoginHistory model)
        {
            return shareService.AddLoginHistory(model);
        }

        /// <summary>
        /// Get a list of LoginHistory via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetLoginHistory_Result> Search([FromUri] LoginHistoryQuery query)
        {
            return shareService.FilterLoginHistory(query);
        }
    }
}