using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Model
{
    public class ComplexDocumentDelivered
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; }
        public string Title { get; set; }
        public System.DateTime DocumentDate { get; set; }
        public System.DateTime DeliveredDate { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<int> ExternalReceiveDivisionId { get; set; }
        public string ExternalReceiveDivision { get; set; }
        public string RecipientsDivision { get; set; }
        public Nullable<int> SignedById { get; set; }
        public string SignedBy { get; set; }
        public int DocumentTypeId { get; set; }
        public int NumberOfCopies { get; set; }
        public int NumberOfPages { get; set; }
        public byte SecretLevel { get; set; }
        public byte UrgencyLevel { get; set; }
        public string Note { get; set; }
        public string OriginalSavingPlace { get; set; }
        public bool LegalDocument { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        public Nullable<int> ReceivedDocumentId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime EditedOn { get; set; }
        public string EditedBy { get; set; }
        public bool SendOut { get; set; }

        public virtual List<CustomDocumentField> CustomDocumentFileds { get; set; }
        public virtual ICollection<DocumentRecipent> DocumentRecipents { get; set; }

        public virtual ICollection<DocumentDocumentField> DocumentFields { get; set; }
    }
}
