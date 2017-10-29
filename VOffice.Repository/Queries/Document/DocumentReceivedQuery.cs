using VOffice.Core.Queries;
using VOffice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Repository.Queries
{
    public class DocumentReceivedQuery : BaseQuery<DocumentReceived>
    {
        public string Type { get; set; }
        public string Keyword { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string UserId { get; set; }
        public string ListSubDepartmentId { get; set; }
        public int DepartmentId { get; set; }

        public bool? DocumentReceived { get; set; }
        public bool? DocumentDelivered { get; set; }
        public bool? LegalDocument { get; set; }

        public DateTime? DocumentDateStart { get; set; }
        public DateTime? DocumentDateEnd { get; set; }
        public DateTime? DocumentDateRDStart { get; set; }
        public DateTime? DocumentDateRDEnd { get; set; }
        public string DocumentSigns { get; set; }

        public string DocumentSignsDelivered { get; set; }
        public string DocumentFields { get; set; }
        public string DocumentTypes { get; set; }
        public string DocumentSecretLevels { get; set; }
        public string DocumentUrgencyLevels { get; set; }

        public int NumberOfMonthsOrWeeks { get; set; }
        public string TypeOfTime { get; set; }
        public int ScopeType { get; set; }
    }
}
