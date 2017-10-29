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
    public partial class DepartmentStaffsRepository : BaseRepository<DepartmentStaff>
    {
        public DepartmentStaffsRepository()
        {

        }
        public List<SPGetDepartmentStaff_Result> GetDepartmentStaff(int type, int departmentId, int staffId)
        {
            List<SPGetDepartmentStaff_Result> result = new List<SPGetDepartmentStaff_Result>();
            result = _entities.SPGetDepartmentStaff(type, departmentId, staffId).ToList();
            return result;
        }
    }
}
