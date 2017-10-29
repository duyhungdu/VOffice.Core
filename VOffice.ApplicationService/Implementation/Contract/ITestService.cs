using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Core.Messages;
using VOffice.Model;

namespace VOffice.ApplicationService.Implementation.Contract
{
    public interface ITestService : IService
    {
        BaseListResponse<SPGetAllRoleByUserId_Result> TestAddUserRole(string groupCode, string username);
    }
}
