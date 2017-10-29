using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Model
{
    public partial class UserDepartment
    {
        public UserDepartment()
        { }
        public UserDepartment(string userId, int subDepartmentId, string listSubDepartmentId, int departmentId, int rootDepartmentId, bool office, string subDepartmentName, string subDepartmentShortName, string departmentName, string departmentShortName, int parentId, string position, bool mainDepartment, int subDepartmentOrder, int departmentOrder, string staffCode, string fullName, string firstName, string email, string phoneNumber, DateTime? dateOfBirth, int? gender, string avatar, bool? leader, bool? seniorLeader, bool? superLeader, bool? signedBy, string googleAccount)
        {
            this.UserId = userId;
            this.SubDepartmentId = subDepartmentId;
            this.ListSubDepartmentId = listSubDepartmentId;
            this.DepartmentId = departmentId;
            this.RootDepartmentId = rootDepartmentId;
            this.Office = office;
            this.SubDepartmentName = !string.IsNullOrEmpty(subDepartmentName) ? subDepartmentName : "";
            this.SubDepartmentShortName = subDepartmentShortName;
            this.DepartmentName = departmentName;
            this.DepartmentShortName = !string.IsNullOrEmpty(departmentShortName) ? departmentShortName : "";
            this.ParentId = parentId;
            this.Position = !string.IsNullOrEmpty(position) ? position : "";
            this.MainDepartment = mainDepartment;
            this.SubDepartmentOrder = subDepartmentOrder;
            this.DepartmentOrder = departmentOrder;
            this.StaffCode = !string.IsNullOrEmpty(staffCode) ? staffCode : "";
            this.FullName = !string.IsNullOrEmpty(fullName) ? fullName : "";
            this.FirstName = !string.IsNullOrEmpty(firstName) ? firstName : "";
            this.Email = !string.IsNullOrEmpty(email) ? email : "";
            this.PhoneNumber = !string.IsNullOrEmpty(phoneNumber) ? phoneNumber: "";
            this.DateOfBirth = dateOfBirth;
            this.Gender = gender;
            this.Avatar = avatar;
            this.Leader = leader;
            this.SeniorLeader = seniorLeader;
            this.SuperLeader = superLeader;
            this.SignedBy = signedBy;
            this.GoogleAccount = !string.IsNullOrEmpty(googleAccount) ? googleAccount : ""; ;
        }
        public int StaffId { get; set; }
        public string Address { get; set; }
        public string UserId { get; set; }
        public int SubDepartmentId { get; set; }
        public string ListSubDepartmentId { get; set; }
        public int DepartmentId { get; set; }
        public int RootDepartmentId { get; set; }
        public bool Office { get; set; }
        public string SubDepartmentName { get; set; }
        public string SubDepartmentShortName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentShortName { get; set; }
        public int ParentId { get; set; }
        public string Position { get; set; }
        public bool MainDepartment { get; set; }
        public int SubDepartmentOrder { get; set; }
        public int DepartmentOrder { get; set; }
        public string StaffCode { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string Avatar { get; set; }

        public bool? Leader { get; set; }
        public bool? SeniorLeader { get; set; }
        public bool? SuperLeader { get; set; }
        public bool? SignedBy { get; set; }
        public string GoogleAccount { get; set; }


    }
}
