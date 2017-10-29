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
    public class NotificationCenterController : ApiController
    {
        private IShareService shareService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_shareService"></param>
        public NotificationCenterController(IShareService _shareService)
        {
            shareService = _shareService;
        }
        /// <summary>
        /// Insert a Notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<NotificationCenter> Add(NotificationCenter model)
        {
            return shareService.AddItemToNotificationCenter(model);
        }
        /// <summary>
        /// Update notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<NotificationCenter> Update(NotificationCenter model)
        {
            return shareService.UpdateNotificationCenter(model);
        }
        /// <summary>
        /// return count notification unread
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<int> GetCountNotificationUnread(string userId, string deviceId)
        {
            return shareService.GetCountNotificationUnread(userId, deviceId);
        }
        /// <summary>
        /// Get notification center
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetNotificationCenter_Result> GetNotificationCenter([FromUri] NotificationCenterQuery query)
        {
            return shareService.GetNotifcationCenter(query);
        }
    }
}