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
    public class UserNotificationController : ApiController
    {
        private IShareService shareService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_shareService"></param>
        public UserNotificationController(IShareService _shareService)
        {
            shareService = _shareService;
        }
        /// <summary>
        /// Insert a User-ClientId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<UserNotification> Add(UserNotification model)
        {
            return shareService.AddUserNotification(model);
        }

    }
}