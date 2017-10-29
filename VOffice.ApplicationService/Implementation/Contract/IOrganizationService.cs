using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;

namespace VOffice.ApplicationService.Implementation.Contract
{
    public interface IOrganizationService : IService
    {
        #region Department
        BaseResponse<Department> GetDepartmentById(int id);
        BaseListResponse<SPGetDepartment_Result> FilterDepartment(DepartmentQuery query);
        BaseListResponse<SPBuildOrganizationTree_Result> BuildOrganizationTree(DepartmentQuery query);
        BaseListResponse<Department> GetAllDepartment();
        BaseResponse<Department> AddDepartment(Department model);
        BaseResponse<Department> UpdateDepartment(Department model);
        BaseResponse DeleteLogicalDepartment(int id);
        BaseListResponse<SPGetDepartment_Result> GetSubDepartmentNonSeft(DepartmentQuery query);
        BaseListResponse<SPGetDepartmentOrganiz_Result> GetDepartmentOrganiz(int type, int departmentId, string keyword);
        BaseListResponse<SPGetDepartmentOrganiz_Result> FilterDepartmentOrganiz(int type, int departmentId, string keyword);
        BaseListResponse<Department> GetListDepartmentByUserId(string userId);
        BaseListResponse<SPGetListDepartmentByStaffId_Result> GetListDepartmentByStaffId(int staffId);

        #endregion Department

        #region Staff
        BaseListResponse<SPGetStaffByDepartmentId_Result> GetStaffByDepartmentId(int id);
        BaseListResponse<SPGetStaffNoAccountByDepartmentId_Result> GetStaffNoAccountByDepartmentId(int id);
        BaseListResponse<SPSearchStaff_Result> SearchStaffs(int departmentId, int parentId, string keyword, bool? active);
        BaseResponse<UserDepartment> GetStaffProfile(string userId);
        BaseResponse<Staff> GetStaffById(int id);
        BaseResponse<Staff> UpdateStaff(Staff model);
        BaseResponse<Staff> UpdateStaffAccount(Staff model);
        BaseResponse<Staff> UpdateStaffGeneralCalendar(Staff model);
        BaseResponse<Staff> GetStaffGeneralCalendar(string userid);
        BaseResponse<SPGetStaffByUserId_Result> GetStaffByUserId(string userId);
        BaseListResponse<Staff> GetAllStaff();
        BaseResponse<int> AddStaff(ComplexStaff model);
        BaseResponse DeleteLogicalStaff(int id);
        BaseListResponse<SPGetBirthDayByDepartmentId_Result> GetBirthDayByDepartment(int deparmentId);
        BaseListResponse<SPGetSeniorLeaderStaff_Result> GetSeniorLeaderStaff(int departmentId);

        BaseListResponse<SPGETStaffNonOrExtraDepartment_Result> GetStaffNonOrExtraDepartment();
        #endregion Staff

        #region Department-Staff
        BaseResponse<DepartmentStaff> AddDepartmentStaff(DepartmentStaff model);
        BaseResponse<DepartmentStaff> AddDepartmentStaffs(List<DepartmentStaff> models);
        BaseListResponse<SPGetDepartmentStaff_Result> GetDepartmentStaff(int type, int departmentId, int staffId);
        BaseResponse DeleteDepartmentStaffByStaff(ComplexDepartmentStaff model);
        BaseResponse DeleteDepartmentsStaff(List<ComplexDepartmentOfStaff> models);
        BaseResponse<ComplexStaffDepartment> AddStaffsDepartment(List<ComplexStaffDepartment> models);
        #endregion

    }
}
