using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Model
{
    public class ComplexStaff : Staff
    {
        public int DepartmentId { get; set; }
    }
    public class ComplexDepartmentStaff
    {
        public int StaffId { get; set; }
    }
    public class ComplexDepartmentOfStaff
    {
        public int StaffId { get; set; }
        public int DepartmentId { get; set; }
        public string CreatedBy { get; set; }
    }
    public class ComplexStaffDepartment
    {
        public string EditedBy { get; set; }
        public int StaffId { get; set; }
        public string Position { get; set; }
        public int DepartmentId { get; set; }
    }
}
