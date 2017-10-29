using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Model
{
    public partial class ComplexUser
    {
    }
    public partial class ComplexGroupUser : AspNetGroupUser
    {
        public string CreatedBy { get; set; }
    }
    public partial class ComplexGroupRole
    {
        public string GroupId { get; set; }
        public string CreatedBy { get; set; }
        public ComplexGroupRole(string _groupId, string _createdBy)
        {
            this.GroupId = _groupId;
            this.CreatedBy = _createdBy;
        }
    }
    public partial class ComplexUserRole
    {
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public ComplexUserRole(string _userId, string _createdBy)
        {
            this.UserId = _userId;
            this.CreatedBy = _createdBy;
        }
    }
    public partial class ComplexRoleOfGroup : AspNetGroupRole
    {
        public string CreatedBy { get; set; }
    }
    public partial class ComplexRoleOfUser : AspNetUserRole
    {
        public string CreatedBy { get; set; }
    }
}
