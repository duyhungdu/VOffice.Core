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
    
    public partial class SPGetTaskDocumentByTaskId_Result
    {
        public Nullable<int> CountRead { get; set; }
        public int TaskDocumentId { get; set; }
        public int TaskId { get; set; }
        public int Id { get; set; }
        public bool ReceivedDocument { get; set; }
        public string DocumentNumber { get; set; }
        public System.DateTime DocumentDate { get; set; }
        public string Title { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        public string DocumentInfo { get; set; }
        public bool Deleted { get; set; }
        public bool Retrieved { get; set; }
    }
}
