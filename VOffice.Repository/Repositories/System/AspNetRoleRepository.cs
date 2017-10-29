using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Model;
using VOffice.Repository.Infrastructure;

namespace VOffice.Repository
{
    public class AspNetRoleRepository : BaseRepository<AspNetRole>
    {
        public List<SPGetAllRoleByUserId_Result> GetRoleByUserId(string userId, bool menu)
        {
            return _entities.SPGetAllRoleByUserId(userId, menu).ToList();
        }

        //public AspNetRole UpdateRole()
    }
}
