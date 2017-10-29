using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Model
{
    public partial class ComplexTaskResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> TaskTypeId { get; set; }
        public int StatusId { get; set; }
        public int Order { get; set; }
        public int Priority { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> Estimated { get; set; }
        public Nullable<int> TimeSpent { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string ContactInformation { get; set; }
        public int Rating { get; set; }
        public string Result { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime EditedOn { get; set; }
        public string EditedBy { get; set; }
        public virtual ICollection<SPGetTaskActivityByTaskId_Result> ResponseTaskActivities { get; set; }
    }
    public class TaskActivityResponse
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public string Action { get; set; }
        public int Type { get; set; }
        public bool Display { get; set; }
        public string Description { get; set; }
        public string TaskField { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime EditedOn { get; set; }
        public string EditedBy { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string ActivityContent { get; set; }
    }
    public class TaskOpinionComplex
    {
        public int Id { get; set; }
        public string OpiniOnContent { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime EditedOn { get; set; }
        public string EditedBy { get; set; }
        public string OpinionFileName { get; set; }
        public string OpinionFilePath { get; set; }
        public string NewValue { get; set; }
        public string ActivityContent { get; set; }
        public string DESCRIPTION { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Model { get; set; }
        public virtual ICollection<SPGetSubTaskOpinionByTaskOpinionId_Result> SubComent { get; set; }
    }
}
