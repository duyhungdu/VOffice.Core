using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text.RegularExpressions;
using System.Web.Http;
using VOffice.ApplicationService.Implementation;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;

namespace VOffice.API.Controllers.Api.Account
{
    /// <summary>
    /// TestController
    /// </summary>
    public class ClientDeviceAnalysticController : ApiController
    {
        private ITestService _testService;

        /// <summary>
        /// TestController
        /// </summary>
        public ClientDeviceAnalysticController()
        {
            _testService = new TestService();
        }
        /// <summary>
        /// Add Role to user like Group
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseListResponse<SPGetAllRoleByUserId_Result> AddUserRole(string groupCode, string userName)
        {
            return _testService.TestAddUserRole(groupCode, userName);
        }
    }
}