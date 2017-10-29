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
    public partial class SystemConfigDepartmentRepository : BaseRepository<SystemConfigDepartment>
    {
        public SystemConfigDepartmentRepository()
        {

        }
        public List<SPGetSystemConfigDepartment_Result> Filter(SystemConfigDepartmentQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var start = 0;
            var limit = query.PageSize;
            var depatmentId = query.DepartmentId;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetSystemConfigDepartment_Result> result = new List<SPGetSystemConfigDepartment_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetSystemConfigDepartment(Util.DetecVowel(keyword), depatmentId, start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public string GetSystemConfigDepartmentValue(SystemConfigDepartmentQuery query)
        {
            var systemConfig = new SystemConfig();
            systemConfig = _entities.SystemConfigs.FirstOrDefault(x => x.Title == query.Title && x.Deleted == false && x.Active == true);
            if (systemConfig != null)
            {
                if (!systemConfig.AllowClientEdit)
                {
                    return systemConfig.Value;
                }
                else
                {
                    var systemConfigDepartment = new SystemConfigDepartment();
                    systemConfigDepartment = _entities.SystemConfigDepartments.FirstOrDefault(x => x.DepartmentId == query.DepartmentId && x.ConfigId == systemConfig.Id);
                    if (systemConfigDepartment != null)
                    {
                        return systemConfigDepartment.Value;
                    }
                    else
                    {
                        return systemConfig.Value;
                    }
                }
            }
            else
            {
                return query.DefaultValue;
            }
        }
        public SystemConfigDepartment GetByTitle(string title, int departmentId)
        {
            var result = new SystemConfigDepartment();
            result = (from scd in _entities.SystemConfigDepartments join sc in _entities.SystemConfigs on scd.ConfigId equals sc.Id where sc.Deleted == false && sc.Active == true && scd.DepartmentId == departmentId && sc.Title == title select scd).FirstOrDefault();
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
