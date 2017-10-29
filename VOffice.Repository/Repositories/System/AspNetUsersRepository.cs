using VOffice.Model;
using VOffice.Repository.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Repository.Infrastructure;


namespace VOffice.Repository
{
    public partial class AspNetUsersRepository : BaseRepository<AspNetUser>
    {
        StaffRepository _staffRepository;
        public AspNetUsersRepository()
        {
             _staffRepository = new StaffRepository();
        }
        public SPGetAspNetUserByUserIdOrUserName_Result GetUserByUserId(string userId)
        {
            return _entities.SPGetAspNetUserByUserIdOrUserName("id", userId).FirstOrDefault();
        }
        public SPGetAspNetUserByUserIdOrUserName_Result GetUserByUserName(string userName)
        {
            return _entities.SPGetAspNetUserByUserIdOrUserName("username", userName).FirstOrDefault();
        }
        public List<SPGetAspNetUsers_Result> Filter(AspNetUserQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var groupId = string.IsNullOrEmpty(query.GroupId) != true ? query.GroupId : "";
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetAspNetUsers_Result> result = new List<SPGetAspNetUsers_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetAspNetUsers(Util.DetecVowel(keyword), groupId, query.DepartmentId, start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }

        #region Test
        public void TestAddUserRole(string groupCode, string username)
        {
            List<AspNetUserRole> listOldUserRole = new List<AspNetUserRole>();

            AspNetGroup group = _entities.AspNetGroups.FirstOrDefault(x => x.Code == groupCode);
            SPGetAspNetUserByUserIdOrUserName_Result u = GetUserByUserName(username);
            List<AspNetGroupRole> listGroupRole = new List<AspNetGroupRole>();
            listGroupRole = _entities.AspNetGroupRoles.Where(x => x.GroupId == group.Id).ToList();
            foreach (var groupRole in listGroupRole)
            {
                AspNetUserRole userRole = new AspNetUserRole();
                userRole.UserId = u.Id;
                userRole.RoleId = groupRole.RoleId;
                userRole.Grant = true;
                _entities.AspNetUserRoles.Add(userRole);
                _entities.SaveChanges();
            }
        }
        #endregion Test
    }
}
