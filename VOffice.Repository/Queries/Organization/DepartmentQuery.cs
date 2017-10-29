using VOffice.Core.Queries;
using VOffice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Repository.Queries
{
    public class DepartmentQuery : BaseQuery<Department>
    {
        public string Keyword { get; set; }
        public int ParentId { get; set; }
        public bool? Active { get; set; }
        public string Type { get; set; }
        public int DepartmentId { get; set; }
        public string Action { get; set; }
    }
}
