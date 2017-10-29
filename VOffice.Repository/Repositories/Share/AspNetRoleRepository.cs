using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Model;
using VOffice.Repository.Infrastructure;

namespace VOffice.Repository.Repositories.Share
{
    public class AspNetRoleRepository : BaseRepository<AspNetRole>
    {
        public List<SPGetAllRoleByUserId_Result> GetRoleByUserId(string userId)
        {
            return _entities.SPGetAllRoleByUserId(userId).ToList();
        }
    }
}
