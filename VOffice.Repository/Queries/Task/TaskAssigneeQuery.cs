using VOffice.Core.Queries;
using VOffice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Repository.Queries
{
    public class TaskAssigneeQuery : BaseQuery<TaskAssignee>
    {
        public string Keyword { get; set; }
        public string UserId { get; set; }
        public int TaskId { get; set; }
    }
}
