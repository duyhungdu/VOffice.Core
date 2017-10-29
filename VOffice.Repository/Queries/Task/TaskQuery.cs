using VOffice.Core.Queries;
using VOffice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Repository.Queries
{
    public class TaskQuery : BaseQuery<Model.Task>
    {
        public string Keyword { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string TimeType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? DocumentId { get; set; }
        public int? DocumentReceived { get; set; }
        public int StatusId { get; set; }
        public bool? AssignToMe { get; set; }
        public bool? RelativeToMe { get; set; }
        public bool? CoprocessorToMe { get; set; }
        public int DepartmentId { get; set; }
        public DateTime? StartFromDate { get; set; }
        public DateTime? StartToDate { get; set; }
        public DateTime? DueFromDate { get; set; }
        public DateTime? DueToDate { get; set; }
        public string TaskType { get; set; }
        public string Project { get; set; }
        public string Assignee { get; set; }
        public string Coprocessor { get; set; }
        public string Supervisor { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public bool TaskAssignee { get; set; }
        public string KeywordDoc { get; set; }
        public string ListStatusId { get; set; }
        public bool Experied { get; set; }
    }
}
