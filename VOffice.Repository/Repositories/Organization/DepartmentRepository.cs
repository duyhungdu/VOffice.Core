using VOffice.Model;
using VOffice.Repository.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Repository.Infrastructure;

namespace VOffice.Repository
{
    public partial class DepartmentRepository : BaseRepository<Department>
    {
        public DepartmentRepository()
        {

        }
        public List<SPGetDepartment_Result> Filter(DepartmentQuery query)
        {
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            List<SPGetDepartment_Result> result = new List<SPGetDepartment_Result>();
            result = _entities.SPGetDepartment(Util.DetecVowel(keyword), query.ParentId, query.Active).ToList();
            return result;
        }
        public List<SPBuildOrganizationTree_Result> BuildOrganizationTree(DepartmentQuery query)
        {
            List<SPBuildOrganizationTree_Result> result = new List<SPBuildOrganizationTree_Result>();
            result = _entities.SPBuildOrganizationTree(query.DepartmentId, query.Type, query.Action).ToList();
            return result;
        }
        public UserDepartment GetUserMainOrganization(string userId)
        {
            var department = new Department();
            var userDepartment = new UserDepartment();
            var subDepartment = new SPGetSubDepartmentByUserId_Result();
            subDepartment = _entities.SPGetSubDepartmentByUserId(userId).FirstOrDefault();
            if (subDepartment != null)
            {
                try
                {

                    #region UserProfile

                    userDepartment.StaffCode = subDepartment.StaffCode;
                    userDepartment.FullName = subDepartment.FullName;
                    userDepartment.FirstName = subDepartment.FirstName;
                    userDepartment.Email = subDepartment.Email;
                    userDepartment.PhoneNumber = subDepartment.PhoneNumber;
                    userDepartment.DateOfBirth = subDepartment.DateOfBirth;
                    userDepartment.Gender = subDepartment.Gender;
                    userDepartment.Avatar = subDepartment.Avatar;
                    userDepartment.Leader = subDepartment.Leader;
                    userDepartment.SeniorLeader = subDepartment.SeniorLeader;
                    userDepartment.SuperLeader = subDepartment.SuperLeader;
                    userDepartment.SignedBy = subDepartment.SignedBy;
                    userDepartment.GoogleAccount = subDepartment.GoogleAccount;
                    userDepartment.Address = subDepartment.Address;
                    userDepartment.StaffId = subDepartment.StaffId;
                    #endregion UserProfile

                    userDepartment.SubDepartmentId = subDepartment.Id;
                    userDepartment.ListSubDepartmentId = "," + subDepartment.Id.ToString() + ",";
                    userDepartment.SubDepartmentName = subDepartment.Name;
                    userDepartment.SubDepartmentShortName = subDepartment.ShortName;
                    userDepartment.ParentId = subDepartment.ParentId;
                    userDepartment.MainDepartment = subDepartment.MainDepartment;
                    userDepartment.Position = subDepartment.Position;
                    userDepartment.SubDepartmentOrder = subDepartment.Order;
                    userDepartment.Office = false;
                    userDepartment.RootDepartmentId = 0;
                    #region query multiLevelDepartment
                    var firstLevelDepartment = new Department();
                    firstLevelDepartment = _entities.Departments.FirstOrDefault(x => x.Id == subDepartment.ParentId && x.Deleted == false && x.Active == true);
                    if (firstLevelDepartment != null)
                    {
                        if (firstLevelDepartment.Office)
                        {
                            userDepartment.Office = true;
                            userDepartment.RootDepartmentId = firstLevelDepartment.ParentId;
                        }
                        if (firstLevelDepartment.Root == 1)
                        {
                            department = firstLevelDepartment;
                        }
                        else
                        {
                            var secondLevelDepartment = new Department();
                            secondLevelDepartment = _entities.Departments.FirstOrDefault(x => x.Id == firstLevelDepartment.ParentId && x.Deleted == false && x.Active == true);
                            if (secondLevelDepartment != null)
                            {
                                if (secondLevelDepartment.Office)
                                {
                                    userDepartment.Office = true;
                                    userDepartment.RootDepartmentId = secondLevelDepartment.ParentId;
                                }
                                if (secondLevelDepartment.Root == 1)
                                {
                                    department = secondLevelDepartment;
                                }
                                else
                                {
                                    var thirdLevelDepartment = new Department();
                                    thirdLevelDepartment = _entities.Departments.FirstOrDefault(x => x.Id == secondLevelDepartment.ParentId && x.Deleted == false && x.Active == true);
                                    if (thirdLevelDepartment != null)
                                    {
                                        if (thirdLevelDepartment.Office)
                                        {
                                            userDepartment.Office = true;
                                            userDepartment.RootDepartmentId = thirdLevelDepartment.ParentId;
                                        }
                                        if (thirdLevelDepartment.Root == 1)
                                        {
                                            department = thirdLevelDepartment;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion query multiLevelDepartment
                    if (department != null)
                    {
                        userDepartment.DepartmentId = department.Id;
                        userDepartment.DepartmentName = department.Name;
                        userDepartment.DepartmentShortName = department.ShortName;
                        userDepartment.DepartmentOrder = department.Order;
                    }
                }
                catch
                {

                }
            }
            return userDepartment;
        }

        public List<SPGetDepartmentOrganiz_Result> GetDepartmentOrganiz(int type, int departmentId, string keyword)
        {
            List<SPGetDepartmentOrganiz_Result> result = new List<SPGetDepartmentOrganiz_Result>();
            result = _entities.SPGetDepartmentOrganiz(type, departmentId, keyword).ToList();
            return result;
        }

        public List<Department> GetListDepartmentsByUserId(string userId)
        {
            List<Department> results = new List<Department>();
            var items = (from au in _entities.AspNetUsers join s in _entities.Staffs on au.Id equals s.UserId join ds in _entities.DepartmentStaffs on s.Id equals ds.StaffId where s.Deleted == false && au.Id == userId select ds).ToList();
            foreach (var sub in items)
            {
                if (sub != null)
                {
                    Department subDepartment = GetById(sub.DepartmentId);
                    if (subDepartment != null)
                    {
                        if (subDepartment.Root == 1)
                        {

                            //Check result has already exist subDepartment
                            results.Add(subDepartment);
                        }
                        else
                        {
                            Department parentDepartment = GetById(subDepartment.ParentId);
                            if (parentDepartment != null)
                            {
                                if (parentDepartment.Root == 1)
                                {
                                    results.Add(parentDepartment);
                                }
                                else
                                {
                                    Department grandDepartment = new Department();
                                    grandDepartment = GetById(parentDepartment.ParentId);
                                    if (grandDepartment != null)
                                    {
                                        if (grandDepartment.Root == 1)
                                        {
                                            results.Add(grandDepartment);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            results = results.Distinct().ToList();
            return results;
        }
        public List<SPGetListDepartmentByStaffId_Result> GetListDepartmentByStaffId(int staffId)
        {
            List<SPGetListDepartmentByStaffId_Result> result = new List<SPGetListDepartmentByStaffId_Result>();
            result = _entities.SPGetListDepartmentByStaffId(staffId).ToList();
            return result;
        }
    }
}
