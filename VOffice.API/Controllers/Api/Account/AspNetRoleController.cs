using System.Web.Http;
using VOffice.ApplicationService.Implementation;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;

namespace VOffice.API.Controllers.Api.Account
{
    /// <summary>
    /// Manager application roles
    /// </summary>
    public class AspNetRoleController : ApiController
    {
        private ISystemService _systemService;
        /// <summary>
        /// Contractor
        /// </summary>
        public AspNetRoleController()
        {
            _systemService = new SystemService();
        }
        /// <summary>
        /// Get all records
        /// </summary>
        /// <returns></returns>
        public BaseListResponse<AspNetRole> GetAll()
        {
            return _systemService.GetAllRole();
        }
        /// <summary>
        /// Check permission of user
        /// </summary>
        /// <param name="userId"></param>
        ///  <param name="roleName"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<bool> CheckPermission(string userId, string roleName)
        {
            return _systemService.CheckPermission(userId, roleName);

        }


        /// <summary>
        /// Add AspNetRole
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<AspNetRole> AddAspNetRole(AspNetRole model)
        {
            return _systemService.AddRole(model);
        }

        /// <summary>
        /// Update AspNetRole
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<AspNetRole> UpdateAspNetRole(AspNetRole model)
        {
            return _systemService.UpdateRole(model);
        }
        /// <summary>
        /// Delete AspNetRole
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<AspNetRole> DeleteAspNetRole(string roleId)
        {
            return _systemService.DeleteRole(roleId);
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
        /// Get Role by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<AspNetRole> GetRoleById(string id)
        {
            return _systemService.GetRoleById(id);
        }
        /// <summary>
        /// Get Roles of User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetRolesOfUser_Result> GetRolesOfUser(string userId)
        {
            return _systemService.GetRolesOfUser(userId);
        }

    }
}