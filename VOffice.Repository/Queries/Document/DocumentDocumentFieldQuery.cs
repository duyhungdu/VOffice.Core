using VOffice.Core.Queries;
using VOffice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Repository.Queries
{
    public class DocumentDocumentFieldQuery : BaseQuery<DocumentDocumentField>
    {
        public string Keyword { get; set; }
        public int DocumentID { get; set; }
        public int DocumentFieldDepartmentId { get; set; }
        public bool ReceivedDocument { get; set; }
    }
}
