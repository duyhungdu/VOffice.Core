using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Model.Validators;
using VOffice.Repository;
using VOffice.Repository.Queries;

namespace VOffice.ApplicationService.Implementation.Contract
{
    public interface ISystemService : IService
    {
        BaseListResponse<AspNetRole> GetAllRole();
        BaseListResponse<MenuRole> GetMenuByUserId(string userId, bool menu);
        BaseListResponse<SPGetAllRoleByUserId_Result> GetAllRoleByUserId(string userId);
        #region AspNetUser
        BaseListResponse<AspNetUser> GetAllUser();
        BaseListResponse<SPGetAspNetUsers_Result> FilterAspNetUsers(AspNetUserQuery query);
        BaseResponse<bool> CheckPermission(string userId, string roleId);
        BaseResponse<SPGetAspNetUserByUserIdOrUserName_Result> GetUserByUserName(string username);
        BaseResponse<SPGetAspNetUserByUserIdOrUserName_Result> GetUserByUserId(string userId);
        BaseResponse<AspNetUser> LockOrUnlockUser(string userId);
        BaseResponse<AspNetUser> UpdateUser(AspNetUser model);
        BaseResponse<AspNetUser> DeleteUser(string userId);
        #endregion AspNetUser
        #region AspnetUserRole
        BaseResponse<AspNetUserRole> AddUserRole(AspNetUserRole model);
        BaseResponse DeleteRolesOfUser(string userId);
        BaseResponse<AspNetRole> AddRole(AspNetRole model);
        BaseResponse<ComplexRoleOfUser> AddRolesForUser(List<ComplexRoleOfUser> models);
        BaseResponse<AspNetRole> UpdateRole(AspNetRole model);
        BaseResponse<AspNetRole> DeleteRole(string roleId);
        BaseListResponse<SPGetRolesOfUser_Result> GetRolesOfUser(string userId);
        BaseResponse DeleteRolesOfUser(ComplexUserRole model);
        #endregion
        #region AspNetGroup
        BaseListResponse<AspNetGroup> GetAllGroups(bool sysAdmin);
        BaseListResponse<SPGetGroupsForUser_Result> GetGroupsForUser(string userId, bool sysAdmin);
        BaseResponse<AspNetGroupUser> AddGroupForUser(ComplexGroupUser model);
        BaseResponse<AspNetGroupUser> AddGroupsForUser(List<ComplexGroupUser> models);
        BaseResponse DeleteGroupsUser(string userId);
        BaseResponse<AspNetRole> GetRoleById(string id);

        #endregion AspNetGroup
        #region AspNetGroupRoles
        BaseResponse<ComplexRoleOfGroup> AddRolesForGroup(List<ComplexRoleOfGroup> models);
        BaseResponse DeleteRolesOfGroup(ComplexGroupRole model);
        BaseListResponse<SPGetRolesOfGroup_Result> GetRolesOfGroup(string groupId);
       
        #endregion
    }
}
