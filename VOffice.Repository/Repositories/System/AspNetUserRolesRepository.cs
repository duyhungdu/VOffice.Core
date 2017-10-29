using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Model;
using VOffice.Repository.Infrastructure;

namespace VOffice.Repository
{
    public class AspNetUserRolesRepository : BaseRepository<AspNetUserRole>
    {
        public AspNetUserRolesRepository()
        {

        }
        public List<SPGetRolesOfUser_Result> GetRolesOfUser(string userId)
        {
            return _entities.SPGetRolesOfUser(userId).ToList();
        }

    }
}
