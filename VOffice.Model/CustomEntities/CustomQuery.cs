using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice
{
    class CustomQuery
    {

    }
    public class CustomDocumentField
    {
        public int DocumentFieldId { get; set; }
        public int DepartmentId { get; set; }
        public CustomDocumentField() { }
        public CustomDocumentField(int documentFieldId, int departmentId)
        {
            this.DocumentFieldId = documentFieldId;
            this.DepartmentId = departmentId;
        }
    }
    public class CustomDocumentRecipent
    {
        public int DocumentId { get; set; }
        public bool ReceivedDocument { get; set; }
    }
}
