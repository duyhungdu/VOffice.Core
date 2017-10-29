using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text.RegularExpressions;
using System.Web.Http;
using VOffice.ApplicationService.Implementation;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using System.Collections.Generic;


namespace VOffice.API.Controllers.Api.Account
{
    /// <summary>
    /// Application User Account
    /// </summary>
    public class AspNetUserController : ApiController
    {
        private ISystemService _systemService;

        /// <summary>
        /// Contructor
        /// </summary>
        public AspNetUserController()
        {
            _systemService = new SystemService();
        }
        /// <summary>
        /// Get tree menu
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<String> GetTreeMenu(string userId, bool menu)
        {
            var response = new BaseResponse<string>();
            var listTree = _systemService.GetMenuByUserId(userId, menu).Data;
            var tree = MenuRole.RawCollectionToTree(listTree);
            string json = JsonConvert.SerializeObject(tree, Formatting.Indented,
                                 new JsonSerializerSettings
                                 {
                                     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                     NullValueHandling = NullValueHandling.Ignore,
                                     ContractResolver = new CamelCasePropertyNamesContractResolver()

                                 });

            response.Value = json.ToString();
            return response;
        }
        /// <summary>
        /// Get all role by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetAllRoleByUserId_Result> GetAllRoleByUserId(string userId)
        {
            return _systemService.GetAllRoleByUserId(userId);
        }
        /// <summary>
        /// Get All User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<AspNetUser> GetAllUser()
        {
            return _systemService.GetAllUser();
        }


        /// <summary>
        /// Get User by Username
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SPGetAspNetUserByUserIdOrUserName_Result> GetUserByUserName(string userName)
        {
            return _systemService.GetUserByUserName(userName);
        }
        /// <summary>
        /// Get User by UserId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SPGetAspNetUserByUserIdOrUserName_Result> GetUserByUserId(string userId)
        {
            return _systemService.GetUserByUserId(userId);
        }

        /// <summary>
        /// Update AspNetUser
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<AspNetUser> UpdateAspNetUser(AspNetUser model)
        {
            return _systemService.UpdateUser(model);
        }

        /// <summary>
        /// Mark an account as deleted, remove refrence Staff and User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<AspNetUser> DeleteAspNetUser(string userId)
        {
            return _systemService.DeleteUser(userId);
        }

        /// <summary>
        /// Get All User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetAspNetUsers_Result> Search([FromUri]AspNetUserQuery query)
        {
            return _systemService.FilterAspNetUsers(query);
        }
        /// <summary>
        /// Delete all role of user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteRolesOfUser(ComplexUserRole model)
        {
            return _systemService.DeleteRolesOfUser(model);

        }
        /// <summary>
        /// Add Roles for group
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<ComplexRoleOfUser> AddRolesForUser(List<ComplexRoleOfUser> models)
        {
            return _systemService.AddRolesForUser(models);
        }
        /// <summary>
        /// Lock or Unlock of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<AspNetUser> LockOrUnlockUser(string userId)
        {
            return _systemService.LockOrUnlockUser(userId);
        }
    }
}