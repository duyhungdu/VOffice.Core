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
    
    public partial class SPGetTaskReport_Result
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> Estimated { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string Processor { get; set; }
    }
}
