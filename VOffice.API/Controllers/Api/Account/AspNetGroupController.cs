using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text.RegularExpressions;
using System.Web.Http;
using VOffice.ApplicationService.Implementation;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;
using System.Collections.Generic;

namespace VOffice.API.Controllers.Api.Account
{
    /// <summary>
    /// TestController
    /// </summary>
    public class AspNetGroupController : ApiController
    {
        private ISystemService _systemService;

        /// <summary>
        /// AspNetGroupController
        /// </summary>
        public AspNetGroupController()
        {
            _systemService = new SystemService();
        }
        /// <summary>
        /// Get All Group
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<AspNetGroup> Search(bool sysAdmin)
        {
            return _systemService.GetAllGroups(sysAdmin);
        }
        /// <summary>
        /// Get All Group of User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetGroupsForUser_Result> GetGroupsForUser(string userId, bool sysAdmin)
        {
            return _systemService.GetGroupsForUser(userId, sysAdmin);
        }
        /// <summary>
        /// Insert groups for user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<AspNetGroupUser> AddGroupsForUser(List<ComplexGroupUser> models)
        {
            return _systemService.AddGroupsForUser(models);
        }
        /// <summary>
        /// Delete roles of group
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteRolesOfGroup(ComplexGroupRole model)
        {
            return _systemService.DeleteRolesOfGroup(model);
        }
        /// <summary>
        /// Get All Roles of group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetRolesOfGroup_Result> GetRolesOfGroup(string groupId)
        {
            return _systemService.GetRolesOfGroup(groupId);
        }
        /// <summary>
        /// Add Roles for group
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<ComplexRoleOfGroup> AddRolesForGroup(List<ComplexRoleOfGroup> models)
        {
            return _systemService.AddRolesForGroup(models);
        }
    }
}