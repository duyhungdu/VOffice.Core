using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Model;
using VOffice.Repository.Infrastructure;

namespace VOffice.Repository
{
    public class AspNetGroupRepository : BaseRepository<AspNetGroup>
    {
        AspNetGroupUsersRepository _groupUsersRepository = new AspNetGroupUsersRepository();
        ApplicationLoggingRepository _applicationLoggingRepository = new ApplicationLoggingRepository();
        AspNetGroupRolesRepository _aspNetGroupRolesRepository = new AspNetGroupRolesRepository();
        AspNetUserRolesRepository _aspNetUserRolesRepository = new AspNetUserRolesRepository();
        public List<AspNetGroup> GetAllGroups(bool sysAdmin)
        {
            if (!sysAdmin)
            {
                return _entities.AspNetGroups.Where(x => x.Deleted == false && x.Active == true && x.AllowClientAccess == true).ToList();
            }
            else
            {
                return _entities.AspNetGroups.Where(x => x.Deleted == false && x.Active == true).ToList();
            }
        }
        public void AddUserToDefaultGroup(string userId, string createdBy)
        {

            var defaultGroup = _entities.AspNetGroups.FirstOrDefault(x => x.Code.ToLower() == "staffs" && x.Deleted == false && x.Active == true);
            if (defaultGroup != null)
            {
                //Add a record in table aspNetGroupUsers
                AspNetGroupUser groupUser = new AspNetGroupUser();
                groupUser.UserId = userId;
                groupUser.GroupId = defaultGroup.Id;
                try
                {
                    _groupUsersRepository.Add(groupUser);
                }
                catch
                {
                    return;
                }
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "AspNetGroupUser", userId + '-' + defaultGroup.Id, "", "", groupUser, "", System.Web.HttpContext.Current.Request.UserHostAddress, createdBy);
                }
                catch
                { }

                try
                {
                    //Add all Roles of defaultGroup from table aspNetGroupRoles to table aspNetUserRoles
                    IEnumerable<AspNetGroupRole> listRoleOfGroup = _aspNetGroupRolesRepository.GetAll().Where(n => n.GroupId == defaultGroup.Id);
                    if (listRoleOfGroup.Count() > 0)
                    {
                        foreach (var item in listRoleOfGroup)
                        {
                            AspNetUserRole itemUserRole = new AspNetUserRole();
                            itemUserRole.UserId = userId;
                            itemUserRole.RoleId = item.RoleId;
                            itemUserRole.Grant = true;
                            _aspNetUserRolesRepository.Add(itemUserRole);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "AspNetUserRole", userId + '-' + item.RoleId, "", "", itemUserRole, "", System.Web.HttpContext.Current.Request.UserHostAddress, createdBy);
                            }
                            catch
                            { }
                        }
                    }
                }
                catch { }
            }
        }
        public List<SPGetGroupsForUser_Result> GetGroupsForUser(string userId, bool sysAdmin)
        {
            return _entities.SPGetGroupsForUser(userId, sysAdmin).ToList();
        }
        public List<SPGetRolesOfGroup_Result> GetRolesOfGroup(string groupId)
        {
            return _entities.SPGetRolesOfGroup(groupId).ToList();
        }

    }
}
