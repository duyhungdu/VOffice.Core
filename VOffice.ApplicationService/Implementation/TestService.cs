using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository;

namespace VOffice.ApplicationService.Implementation
{
    public class TestService : ITestService
    {
        protected readonly AspNetRoleRepository _aspNetRoleRepository;
        protected readonly AspNetUserRolesRepository _aspNetUserRolesRepository;
        protected readonly AspNetUsersRepository _userRepository;

        public TestService()
        {
            _aspNetRoleRepository = new AspNetRoleRepository();
            _userRepository = new AspNetUsersRepository();
            _aspNetUserRolesRepository = new AspNetUserRolesRepository();
        }
        #region Test
        public BaseListResponse<SPGetAllRoleByUserId_Result> TestAddUserRole(string groupCode, string username)
        {
            BaseListResponse<SPGetAllRoleByUserId_Result> result = new BaseListResponse<SPGetAllRoleByUserId_Result>();
            _userRepository.TestAddUserRole(groupCode, username);
            var u = new SPGetAspNetUserByUserIdOrUserName_Result();
            u = _userRepository.GetUserByUserName(username);
            result.Data = _aspNetRoleRepository.GetRoleByUserId(u.Id, false).ToList();
            return result;
        }
        #endregion Test
    }
}
