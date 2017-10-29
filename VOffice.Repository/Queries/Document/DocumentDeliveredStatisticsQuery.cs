using System;
using VOffice.Core.Queries;
using VOffice.Model;

namespace VOffice.Repository.Queries
{
    public class DocumentDeliveredStatisticsQuery : BaseQuery<SearchDocumentInfo>
    {
        public int? DepartmentId { get; set; }
        public string Keyword { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ListSignById { get; set; }
        public string ListDocumentFieldId { get; set; }
        public string ListDocumentTypeId { get; set; }
        public string GroupBy { get; set; }
        public string GroupTitle { get; set; }
        public string UserId { get; set; }
    }
}