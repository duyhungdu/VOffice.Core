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
    public partial class StaffRepository : BaseRepository<Staff>
    {
        public StaffRepository()
        {

        }
        public List<SPGetStaffByDepartmentId_Result> GetStaffByDepartmentId(int departmentId)
        {
            List<SPGetStaffByDepartmentId_Result> result = new List<SPGetStaffByDepartmentId_Result>();
            result = _entities.SPGetStaffByDepartmentId(departmentId).ToList();
            return result;
        }
        public SPGetStaffByUserId_Result GetStaffByUserId(string userId)
        {
            return _entities.SPGetStaffByUserId(userId).FirstOrDefault();
        }
        public List<SPGetStaffNoAccountByDepartmentId_Result> GetStaffNoAccountByDepartmentId(int departmentId)
        {
            List<SPGetStaffNoAccountByDepartmentId_Result> result = new List<SPGetStaffNoAccountByDepartmentId_Result>();
            result = _entities.SPGetStaffNoAccountByDepartmentId(departmentId).ToList();
            return result;
        }
        public List<SPSearchStaff_Result> SearchStaffs(int departmentId, int parentId, string keyword, bool? active)
        {
            List<SPSearchStaff_Result> result = new List<SPSearchStaff_Result>();
            result = _entities.SPSearchStaff(departmentId, parentId, Util.DetecVowel(keyword), active).ToList();
            return result;
        }

        public List<SPGetBirthDayByDepartmentId_Result> GetBirthDayByDeparmentId(int deparmentId)
        {
            List<SPGetBirthDayByDepartmentId_Result> result = new List<SPGetBirthDayByDepartmentId_Result>();
            result = _entities.SPGetBirthDayByDepartmentId(deparmentId).ToList();
            return result;
        }
        public List<SPGetSeniorLeaderStaff_Result> GetSeniorLeaderStaff(int departmentId)
        {
            List<SPGetSeniorLeaderStaff_Result> result = new List<SPGetSeniorLeaderStaff_Result>();
            result = _entities.SPGetSeniorLeaderStaff(departmentId).ToList();
            return result;
        }
        public List<SPGETStaffNonOrExtraDepartment_Result> GetStaffNonOrExtraDepartment()
        {
            List<SPGETStaffNonOrExtraDepartment_Result> result = new List<SPGETStaffNonOrExtraDepartment_Result>();
            result = _entities.SPGETStaffNonOrExtraDepartment().ToList();
            return result;
        }
    }
}
