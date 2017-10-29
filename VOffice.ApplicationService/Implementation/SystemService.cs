using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Model.Validators;
using VOffice.Repository;
using VOffice.Repository.Queries;

namespace VOffice.ApplicationService.Implementation
{
    public class SystemService : BaseService, ISystemService
    {
        protected readonly AspNetRoleRepository _aspNetRoleRepository;
        protected readonly AspNetUserRolesRepository _aspNetUserRolesRepository;
        protected readonly AspNetUsersRepository _userRepository;
        protected readonly AspNetGroupRepository _groupRepository;
        protected readonly AspNetGroupUsersRepository _groupUsersRepository;
        protected readonly StaffRepository _staffRepository;
        protected readonly ApplicationLoggingRepository _applicationLoggingRepository;
        protected readonly AspNetGroupRolesRepository _aspNetGroupRolesRepository;

        public SystemService()
        {
            _aspNetRoleRepository = new AspNetRoleRepository();
            _userRepository = new AspNetUsersRepository();
            _aspNetUserRolesRepository = new AspNetUserRolesRepository();
            _groupRepository = new AspNetGroupRepository();
            _staffRepository = new StaffRepository();
            _applicationLoggingRepository = new ApplicationLoggingRepository();
            _groupUsersRepository = new AspNetGroupUsersRepository();
            _aspNetGroupRolesRepository = new AspNetGroupRolesRepository();
        }
        #region AspNetRole
        public BaseListResponse<AspNetRole> GetAllRole()
        {
            var response = new BaseListResponse<AspNetRole>();
            try
            {
                response.IsSuccess = true;
                response.Data = _aspNetRoleRepository.GetAll().Where(x => x.Deleted == false).ToList();
                response.TotalItems = response.Data.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseListResponse<MenuRole> GetMenuByUserId(string userId, bool menu)
        {
            var response = new BaseListResponse<MenuRole>();
            try
            {
                var listGetRoleById = _aspNetRoleRepository.GetRoleByUserId(userId, menu).ToList();
                var listMenuRole = new List<MenuRole>();
                foreach (var item in listGetRoleById)
                {
                    listMenuRole.Add(new MenuRole
                    {
                        Id = item.Id,
                        ParentId = item.ParentId,
                        Title = item.Title,
                        Active = item.Active.GetValueOrDefault(),
                        Code = item.Code,
                        Href = item.Href,
                        Icon = item.Icon,
                        Leaf = item.Leaf.GetValueOrDefault(),
                        OnMainMenu = item.OnMainMenu.GetValueOrDefault(),
                        OnRightMenu = item.OnRightMenu.GetValueOrDefault(),
                        OnTopMenu = item.OnTopMenu.GetValueOrDefault(),
                        Order = item.Order,
                        Root = item.Root.GetValueOrDefault(),
                        Target = item.Target
                    });
                }

                response.Data = listMenuRole;
            }
            catch (Exception ex)
            {
                response.Message = ex.ToString();
            }


            return response;
        }

        public BaseListResponse<SPGetAllRoleByUserId_Result> GetAllRoleByUserId(string userId)
        {
            var response = new BaseListResponse<SPGetAllRoleByUserId_Result>();
            try
            {
                response.IsSuccess = true;
                response.Data = _aspNetRoleRepository.GetRoleByUserId(userId, false);
                response.TotalItems = response.Data.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<bool> CheckPermission(string userId, string roleName)
        {
            var response = new BaseResponse<bool>();
            BaseListResponse<AspNetUserRole> lstAspNetUserRole = new BaseListResponse<AspNetUserRole>();
            try
            {
                BaseResponse<AspNetRole> role = new BaseResponse<AspNetRole>();
                role.Value = _aspNetRoleRepository.GetAll().Where(n => n.Name.ToLower() == roleName.ToLower()).FirstOrDefault();
                lstAspNetUserRole.Data = _aspNetUserRolesRepository.GetAll().Where(n => n.UserId == userId && n.RoleId == role.Value.Id).ToList();
                if (lstAspNetUserRole.Data.Count() > 0)
                {
                    response.Value = true;
                }
                else
                {
                    response.Value = false;
                }
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<AspNetRole> AddRole(AspNetRole model)
        {
            var response = new BaseResponse<AspNetRole>();
            var exist = _aspNetRoleRepository.FindBy(x => x.Name == model.Name).FirstOrDefault();
            if (exist != null)
            {
                response.IsSuccess = false;
                response.Message = "Mã quyền đã tồn tại ";
                return response;
            }
            var errors = Validate<AspNetRole>(model, new AspNetRoleValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<AspNetRole> errResponse = new BaseResponse<AspNetRole>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.Id = Guid.NewGuid().ToString();
                model.CreatedOn = DateTime.Now;
                response.Value = _aspNetRoleRepository.Add(model);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<AspNetRole> UpdateRole(AspNetRole model)
        {
            BaseResponse<AspNetRole> result = new BaseResponse<AspNetRole>();
            var exist = _aspNetRoleRepository.FindBy(x => x.Name == model.Name && x.Id != model.Id).FirstOrDefault();
            if (exist != null)
            {
                result.IsSuccess = false;
                result.Message = "Mã quyền đã tồn tại ";
                return result;
            }

            _aspNetRoleRepository.Edit(model);
            result.Value = model;
            return result;
        }
        public BaseResponse<AspNetRole> DeleteRole(string roleId)
        {
            BaseResponse<AspNetRole> result = new BaseResponse<AspNetRole>();
            AspNetRole role = _aspNetRoleRepository.GetById(roleId);
            var allRole = _aspNetRoleRepository.GetAll().Where(x => x.Deleted == false).ToList();
            role.EditedOn = DateTime.Now;
            role.Deleted = true;
            role.Active = false;
            _aspNetRoleRepository.Edit(role);
            foreach (var item in allRole)
            {
                if (item.ParentId == roleId)
                {
                    item.ParentId = "#";
                    _aspNetRoleRepository.Edit(item);
                }
            }
            return result;
        }
        public BaseResponse<AspNetRole> GetRoleById(string id)
        {
            var response = new BaseResponse<AspNetRole>();
            try
            {
                response.IsSuccess = true;
                response.Value = _aspNetRoleRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion AspNetRole

        #region Login Analystic
        public BaseResponse<string> AnalysticDevice(string userAgent)
        {
            BaseResponse<string> result = new BaseResponse<string>();



            return result;

        }
        #endregion Login Analystic

        #region AspnetUser
        public BaseListResponse<AspNetUser> GetAllUser()
        {
            var response = new BaseListResponse<AspNetUser>();
            try
            {
                var result = _userRepository.GetAll().Where(x => x.Deleted == false).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<SPGetAspNetUserByUserIdOrUserName_Result> GetUserByUserName(string username)
        {
            var response = new BaseResponse<SPGetAspNetUserByUserIdOrUserName_Result>();
            try
            {
                response.Value = _userRepository.GetUserByUserName(username);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse<SPGetAspNetUserByUserIdOrUserName_Result> GetUserByUserId(string userId)
        {
            var response = new BaseResponse<SPGetAspNetUserByUserIdOrUserName_Result>();
            try
            {
                response.Value = _userRepository.GetUserByUserId(userId);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetAspNetUsers_Result> FilterAspNetUsers(AspNetUserQuery query)
        {
            var response = new BaseListResponse<SPGetAspNetUsers_Result>();
            int count = 0;
            try
            {
                response.Data = _userRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<AspNetUser> UpdateUser(AspNetUser model)
        {
            BaseResponse<AspNetUser> result = new BaseResponse<AspNetUser>();
            AspNetUser user = _userRepository.GetById(model.Id);
            user.GoogleAccount = model.GoogleAccount;
            user.EditedBy = model.EditedBy;
            user.EditedOn = DateTime.Now;
            _userRepository.Edit(user);
            result.Value = user;
            return result;

        }
        public BaseResponse<AspNetUser> DeleteUser(string userId)
        {
            BaseResponse<AspNetUser> result = new BaseResponse<AspNetUser>();
            AspNetUser user = _userRepository.GetById(userId);
            var staff = new SPGetStaffByUserId_Result();
            staff = _staffRepository.GetStaffByUserId(userId);
            user.GoogleAccount = user.UserName;
            user.UserName = user.Id;
            user.Email = user.Id;
            user.Deleted = true;
            user.EditedOn = DateTime.Now;

            if (staff != null)
            {
                Staff modelStaff = new Staff();
                modelStaff = _staffRepository.GetById(staff.Id);
                modelStaff.EditedOn = DateTime.Now;
                _staffRepository.Edit(modelStaff);
            }
            _userRepository.Edit(user);
            result.Value = user;
            return result;

        }
        public BaseResponse<AspNetUser> LockOrUnlockUser(string userId)
        {
            BaseResponse<AspNetUser> result = new BaseResponse<AspNetUser>();
            try
            {
                AspNetUser user = _userRepository.GetById(userId);
                user.EditedOn = DateTime.Now;
                if (user.LockoutEnabled == true)
                {
                    user.LockoutEnabled = false;
                }
                else
                {
                    user.LockoutEnabled = true;
                }
                _userRepository.Edit(user);

            }
            catch { }
            return result;
        }

        #endregion

        #region  AspNetUserRole
        public BaseResponse<AspNetUserRole> AddUserRole(AspNetUserRole model)
        {
            BaseResponse<AspNetUserRole> result = new BaseResponse<AspNetUserRole>();
            result.Value = _aspNetUserRolesRepository.Add(model);
            return result;
        }
        public BaseResponse<ComplexRoleOfUser> AddRolesForUser(List<ComplexRoleOfUser> models)
        {
            var response = new BaseResponse<ComplexRoleOfUser>();
            AspNetUserRole userRole;
            try
            {
                foreach (var model in models)
                {
                    userRole = new AspNetUserRole();
                    userRole.RoleId = model.RoleId;
                    userRole.Grant = model.Grant;
                    userRole.UserId = model.UserId;
                    _aspNetUserRolesRepository.Add(userRole);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "ADD", "AspNetGroupRoles", model.RoleId + "," + model.UserId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            response.IsSuccess = true;
            return response;
        }
        public BaseResponse DeleteRolesOfUser(string userId)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                _aspNetUserRolesRepository.DeleteMulti(x => x.UserId == userId);
            }
            catch
            {

            }
            return response;
        }
        public BaseListResponse<SPGetRolesOfUser_Result> GetRolesOfUser(string userId)
        {
            var response = new BaseListResponse<SPGetRolesOfUser_Result>();
            try
            {
                var result = _aspNetUserRolesRepository.GetRolesOfUser(userId);
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteRolesOfUser(ComplexUserRole model)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                _aspNetUserRolesRepository.DeleteMulti(x => x.UserId == model.UserId);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "AspNetUserRoles", model.UserId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
            }
            catch { }
            return response;
        }
        #endregion

        #region AspNetGroup
        public BaseListResponse<AspNetGroup> GetAllGroups(bool sysAdmin)
        {
            var response = new BaseListResponse<AspNetGroup>();
            try
            {
                var result = _groupRepository.GetAllGroups(sysAdmin);
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetGroupsForUser_Result> GetGroupsForUser(string userId, bool sysAdmin)
        {
            var response = new BaseListResponse<SPGetGroupsForUser_Result>();
            try
            {
                var result = _groupRepository.GetGroupsForUser(userId, sysAdmin);
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<AspNetGroupUser> AddGroupForUser(ComplexGroupUser model)
        {
            var response = new BaseResponse<AspNetGroupUser>();
            AspNetGroupUser groupUser = new AspNetGroupUser();
            AspNetUserRole userRole;
            try
            {
                groupUser.UserId = model.UserId;
                groupUser.GroupId = model.GroupId;
                response.Value = _groupUsersRepository.Add(groupUser);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "AspNetGroupUser", response.Value.UserId + '-' + response.Value.GroupId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
                IEnumerable<AspNetGroupRole> listRoleOfGroup = _aspNetGroupRolesRepository.GetAll().Where(n => n.GroupId == model.GroupId);
                if (listRoleOfGroup.Count() > 0)
                {
                    foreach (var item in listRoleOfGroup)
                    {
                        IEnumerable<AspNetUserRole> listUserRole = _aspNetUserRolesRepository.GetAll().Where(n => n.RoleId == item.RoleId && n.UserId == model.UserId);
                        if (listUserRole.Count() == 0)
                        {
                            userRole = new AspNetUserRole();
                            userRole.RoleId = item.RoleId;
                            userRole.UserId = model.UserId;
                            userRole.Grant = true;
                            AddUserRole(userRole);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "AspNetUserRole", userRole.UserId + '-' + userRole.RoleId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                            }
                            catch
                            { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<AspNetGroupUser> AddGroupsForUser(List<ComplexGroupUser> models)
        {
            var response = new BaseResponse<AspNetGroupUser>();

            foreach (var model in models)
            {
                IEnumerable<AspNetGroupUser> groupsUser = _groupUsersRepository.GetAll().Where(n => n.UserId == model.UserId);
                if (groupsUser.Count() > 0)
                {
                    try
                    {
                        DeleteGroupsUser(model.UserId);
                        DeleteRolesOfUser(model.UserId);
                        _applicationLoggingRepository.Log("EVENT", "DELETE", "AspNetGroupUser", model.UserId + '-' + model.GroupId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                        _applicationLoggingRepository.Log("EVENT", "DELETE", "AspNetUserRole", model.UserId + '-' + model.GroupId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    { }
                    break;
                }
            }
            foreach (var model in models)
            {
                if (!string.IsNullOrEmpty(model.GroupId))
                {
                    AddGroupForUser(model);
                }
            }
            response.IsSuccess = true;
            return response;
        }
        public BaseResponse DeleteGroupsUser(string userId)
        {
            BaseResponse response = new BaseResponse();
            _groupUsersRepository.DeleteMulti(x => x.UserId == userId);
            response.IsSuccess = true;
            return response;
        }
        #endregion AspNetGroup

        #region AspNetGroupRoles 
        public BaseResponse<ComplexRoleOfGroup> AddRolesForGroup(List<ComplexRoleOfGroup> models)
        {
            var response = new BaseResponse<ComplexRoleOfGroup>();
            AspNetGroupRole groupRole;
            string groupId = "";
            try
            {
                for (int i = 0; i < models.Count(); i++)
                {
                    if (i == 0)
                    {
                        groupId = models[i].GroupId;
                        break;
                    }
                }
                foreach (var model in models)
                {
                    groupRole = new AspNetGroupRole();
                    groupRole.RoleId = model.RoleId;
                    groupRole.GroupId = model.GroupId;
                    _aspNetGroupRolesRepository.Add(groupRole);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "ADD", "AspNetGroupRoles", model.GroupId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    { }
                }
                //sau khi cập nhật lại quyền cho group -> xóa hết quyền của người dùng, kiểm tra người dùng thuộc những nhóm nào > add lại quyền cho người dùng

                IEnumerable<AspNetGroupUser> lstGroupUser = _groupUsersRepository.FindBy(n => n.GroupId == groupId);
                AspNetUserRole aspNetUserRole;
                if (lstGroupUser.Count() > 0)
                {
                    foreach (var item in lstGroupUser)
                    {
                        //xóa hết quyền cũ của người dùng
                        _aspNetUserRolesRepository.DeleteMulti(x => x.UserId == item.UserId);
                        IEnumerable<AspNetGroupUser> lstGroupOfUser = _groupUsersRepository.FindBy(n => n.GroupId == groupId);
                        //add lại quyền mới theo nhóm của người dùng
                        foreach (var group in lstGroupOfUser)
                        {
                            IEnumerable<AspNetGroupRole> lstGroupRoles = _aspNetGroupRolesRepository.FindBy(n => n.GroupId == group.GroupId);
                            foreach (var model in lstGroupRoles)
                            {
                                aspNetUserRole = new AspNetUserRole();
                                aspNetUserRole.RoleId = model.RoleId;
                                aspNetUserRole.UserId = group.UserId;
                                _aspNetUserRolesRepository.Add(aspNetUserRole);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            response.IsSuccess = true;
            return response;
        }
        public BaseResponse DeleteRolesOfGroup(ComplexGroupRole model)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                _aspNetGroupRolesRepository.DeleteMulti(x => x.GroupId == model.GroupId);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "AspNetGroupRoles", model.GroupId, "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
            }
            catch { }
            return response;
        }
        public BaseListResponse<SPGetRolesOfGroup_Result> GetRolesOfGroup(string groupId)
        {
            var response = new BaseListResponse<SPGetRolesOfGroup_Result>();
            try
            {
                var result = _groupRepository.GetRolesOfGroup(groupId);
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        #endregion
    }
}
