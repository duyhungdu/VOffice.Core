//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VOffice.Model
{
    using System;
    
    public partial class SPGetDocument_Result
    {
        public Nullable<int> HistoryId { get; set; }
        public Nullable<int> TaskId { get; set; }
        public string TaskCode { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string DocumentNumber { get; set; }
        public string ReceivedNumber { get; set; }
        public string ExternalFromDivision { get; set; }
        public string SignedBy { get; set; }
        public System.DateTime documentDate { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        public int ReceivedDocument { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool SendOut { get; set; }
    }
}
