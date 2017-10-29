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
    public interface IShareService : IService
    {
        #region System Config
        BaseResponse<SystemConfig> GetSystemConfigById(int id);
        BaseListResponse<SystemConfig> GetAllSystemConfig();
        BaseListResponse<SPGetConfig_Result> FilterSystemConfig(SystemConfigQuery query);
        BaseResponse<SystemConfig> AddSystemConfig(SystemConfig model);
        BaseResponse<SystemConfig> UpdateSystemConfig(SystemConfig model);
        BaseResponse<SystemConfig> GetSystemConfigByCode(string code, bool systemConfig);
        BaseResponse DeleteLogicalSystemConfig(int id);
        BaseResponse CloneSystemConfig(string listDepartmentId);
        #endregion System Config
        #region System Config Department
        BaseListResponse<SystemConfigDepartment> GetAllSystemConfigDepartment();
        BaseListResponse<SPGetSystemConfigDepartment_Result> FilterSystemConfigDepartment(SystemConfigDepartmentQuery query);
        BaseResponse<SystemConfigDepartment> AddSystemConfigDepartment(SystemConfigDepartment model);
        BaseResponse<SystemConfigDepartment> UpdateSystemConfigDepartment(SystemConfigDepartment model);        
        BaseResponse<SystemConfigDepartment> GetSystemConfigDepartmentValue(SystemConfigDepartmentQuery query);
        BaseResponse<SystemConfigDepartment> GetSystemConfigDepartmentByConfigIdAndDepartmentId(int departmentId, int configId);

        #endregion System Config Department
        #region LoginHistory
        BaseResponse<LoginHistory> AddLoginHistory(LoginHistory model);
        BaseListResponse<SPGetLoginHistory_Result> FilterLoginHistory(LoginHistoryQuery query);
        #endregion LoginHistory
        #region ApplicationLogging
        BaseListResponse<SPGetApplicationLogging_Result> FilterApplicationLogging(ApplicationLoggingQuery query);
        BaseResponse<ApplicationLogging> GetApplicationLoggingById(int id);

        BaseResponse DeleteApplicationLogging(ComplexApplicationLogging applicationLogging);
        #endregion
        #region UserNotification
        BaseResponse<UserNotification> AddUserNotification(UserNotification model);
        BaseResponse<NotificationCenter> AddItemToNotificationCenter(NotificationCenter model);
        BaseResponse<NotificationCenter> UpdateNotificationCenter(NotificationCenter model);
        BaseListResponse<SPGetNotificationCenter_Result> GetNotifcationCenter(NotificationCenterQuery model);
        BaseResponse<int> GetCountNotificationUnread(string userId, string deviceId);
        #endregion UserNotification
    }
}
