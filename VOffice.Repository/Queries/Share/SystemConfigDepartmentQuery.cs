using VOffice.Core.Queries;
using VOffice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Repository.Queries
{
    public class SystemConfigDepartmentQuery : BaseQuery<SystemConfigDepartment>
    {
        public string Keyword { get; set; }
        public int DepartmentId { get; set; }
        public string Title { get; set; }
        public string DefaultValue { get; set; }
    }
}
