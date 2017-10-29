using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Model.Validators;
using VOffice.Repository;
using VOffice.Repository.Queries;

namespace VOffice.ApplicationService
{

    public partial class ShareService : BaseService, IShareService
    {
        protected readonly SystemConfigRepository _systemConfigRepository;
        protected readonly SystemConfigDepartmentRepository _systemConfigDepartmentRepository;
        protected readonly DepartmentRepository _departmentRepository;
        protected readonly LoginHistoryRepository _loginHistoryRepository;
        protected readonly ApplicationLoggingRepository _applicationLoggingRepository;
        protected readonly UserNotificationRepository _userNotificationRepository;
        protected readonly NotificationCenterRepository _notificationCenterRepository;
        public ShareService()
        {
            _systemConfigRepository = new SystemConfigRepository();
            _systemConfigDepartmentRepository = new SystemConfigDepartmentRepository();
            _departmentRepository = new DepartmentRepository();
            _loginHistoryRepository = new LoginHistoryRepository();
            _applicationLoggingRepository = new ApplicationLoggingRepository();
            _userNotificationRepository = new UserNotificationRepository();
            _notificationCenterRepository = new NotificationCenterRepository();
        }
        #region SystemConfig
        public BaseResponse<SystemConfig> GetSystemConfigById(int id)
        {
            var response = new BaseResponse<SystemConfig>();
            try
            {
                response.Value = _systemConfigRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SystemConfig> GetAllSystemConfig()
        {
            var response = new BaseListResponse<SystemConfig>();
            try
            {
                var result = _systemConfigRepository.GetAll().Where(x => x.Deleted == false).ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetConfig_Result> FilterSystemConfig(SystemConfigQuery query)
        {
            var response = new BaseListResponse<SPGetConfig_Result>();
            int count = 0;
            try
            {
                response.Data = _systemConfigRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<SystemConfig> AddSystemConfig(SystemConfig model)
        {
            var response = new BaseResponse<SystemConfig>();
            var errors = Validate<SystemConfig>(model, new SystemConfigValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<SystemConfig> errResponse = new BaseResponse<SystemConfig>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _systemConfigRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "SystemConfig", response.Value.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
                if (model.IsSystemConfig == false)
                {
                    DepartmentQuery query = new DepartmentQuery();
                    query.Keyword = "";
                    query.ParentId = 0;
                    query.Active = true;
                    List<SPGetDepartment_Result> arrDepartments = _departmentRepository.Filter(query).Where(n => n.ParentId != 0).ToList();
                    SystemConfigDepartment config;
                    if (arrDepartments.Count() > 0)
                    {
                        foreach (var item in arrDepartments)
                        {
                            config = new SystemConfigDepartment();
                            config.ConfigId = response.Value.Id;
                            config.Value = model.Value;
                            config.Description = model.Description;
                            config.DepartmentId = item.Id;
                            config.CreatedBy = model.CreatedBy;
                            config.CreatedOn = model.CreatedOn;
                            config.EditedBy = model.EditedBy;
                            config.EditedOn = model.EditedOn;
                            _systemConfigDepartmentRepository.Add(config);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "SystemConfigDepartment", response.Value.Id.ToString(), "", "", config, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                            }
                            catch
                            { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<SystemConfig> UpdateSystemConfig(SystemConfig model)
        {
            BaseResponse<SystemConfig> response = new BaseResponse<SystemConfig>();
            var errors = Validate<SystemConfig>(model, new SystemConfigValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<SystemConfig> errResponse = new BaseResponse<SystemConfig>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var config = _systemConfigRepository.GetById(model.Id);
                if (config.Title != model.Title)
                {
                    IEnumerable<SystemConfig> listSystemConfig = _systemConfigRepository.FindBy(x => x.Value.ToLower() == model.Value.ToLower() && x.Deleted == false);
                    if (listSystemConfig.Count() > 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Cấu hình hệ thống đã tồn tại.";
                    }
                    else
                    {
                        model.EditedOn = DateTime.Now;
                        response.Value = _systemConfigRepository.Update(config, model);
                    }
                }
                else
                {
                    model.EditedOn = DateTime.Now;
                    response.Value = _systemConfigRepository.Update(config, model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "SystemConfig", model.Id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    { }
                }
                //nếu biến cấu hình cũ là có isSystemConfig=false bây giờ =true thì phải thêm các tham số HT đơn vị.
                if (config.IsSystemConfig == false && model.IsSystemConfig == true)
                {
                    IEnumerable<SystemConfigDepartment> lstConfigDepartment = _systemConfigDepartmentRepository.FindBy(n => n.ConfigId == model.Id);
                    if (lstConfigDepartment.Count() > 0)
                    {
                        _systemConfigDepartmentRepository.DeleteMulti(x => x.ConfigId == model.Id);
                    }
                }
                else if (config.IsSystemConfig == true && model.IsSystemConfig == false)
                {
                    DepartmentQuery query = new DepartmentQuery();
                    query.Keyword = "";
                    query.ParentId = 0;
                    query.Active = true;
                    List<SPGetDepartment_Result> arrDepartments = _departmentRepository.Filter(query).Where(n => n.ParentId != 0).ToList();
                    SystemConfigDepartment configSub;
                    if (arrDepartments.Count() > 0)
                    {
                        foreach (var item in arrDepartments)
                        {
                            configSub = new SystemConfigDepartment();
                            configSub.ConfigId = response.Value.Id;
                            configSub.Value = model.Value;
                            configSub.Description = model.Description;
                            configSub.DepartmentId = item.Id;
                            configSub.CreatedBy = model.CreatedBy;
                            configSub.CreatedOn = model.CreatedOn;
                            configSub.EditedBy = model.EditedBy;
                            configSub.EditedOn = model.EditedOn;
                            _systemConfigDepartmentRepository.Add(configSub);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "SystemConfigDepartment", response.Value.Id.ToString(), "", "", configSub, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                            }
                            catch
                            { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalSystemConfig(int id)
        {
            BaseResponse response = new BaseResponse();
            SystemConfig model = _systemConfigRepository.GetById(id);
            try
            {
                model.EditedOn = DateTime.Now;
                model.Deleted = true;
                _systemConfigRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "SystemConfig", id.ToString(), "", "", model, "", System.Web.HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch
                { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse<SystemConfig> GetSystemConfigByCode(string code, bool systemConfig)
        {
            var response = new BaseResponse<SystemConfig>();
            try
            {
                response.Value = _systemConfigRepository.GetAll().FirstOrDefault(n => n.Active == true && n.Deleted == false && n.IsSystemConfig == systemConfig && n.Title == code);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }

        public BaseResponse CloneSystemConfig(string listDepartmentId)
        {
            BaseResponse<SystemConfig> response = new BaseResponse<SystemConfig>();
            var result = new BaseListResponse<SPCopySystemConfig_Result>();
            string[] arr = listDepartmentId.Split(',');
            if (arr.Count() > 0)
            {
                foreach (var item_arr in arr)
                {
                    if (!string.IsNullOrEmpty(item_arr.Replace(',', ' ')))
                    {
                        result.Data = _systemConfigRepository.GetSystemConfigNotInDepartment(int.Parse(item_arr));
                        foreach (var item in result.Data)
                        {
                            SystemConfigDepartment model = new SystemConfigDepartment();
                            model.Value = item.Value;
                            model.ConfigId = item.Id;
                            model.Description = item.Description;
                            model.DepartmentId = int.Parse(item_arr);
                            model.CreatedOn = DateTime.Now;
                            model.CreatedBy = item.CreatedBy;
                            model.EditedOn = DateTime.Now;
                            model.EditedBy = item.EditedBy;
                            _systemConfigDepartmentRepository.Add(model);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "SystemConfigDepartment", model.Value, "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                            }
                            catch { }
                        }
                    }
                }
            }
            return response;
        }
        #endregion SystemConfig

        #region System Config Department
        public BaseListResponse<SystemConfigDepartment> GetAllSystemConfigDepartment()
        {
            var response = new BaseListResponse<SystemConfigDepartment>();
            try
            {
                var result = _systemConfigDepartmentRepository.GetAll().ToList();
                response.Data = result;
                response.TotalItems = result.Count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetSystemConfigDepartment_Result> FilterSystemConfigDepartment(SystemConfigDepartmentQuery query)
        {
            var response = new BaseListResponse<SPGetSystemConfigDepartment_Result>();
            int count = 0;
            try
            {
                response.Data = _systemConfigDepartmentRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<SystemConfigDepartment> AddSystemConfigDepartment(SystemConfigDepartment model)
        {
            var response = new BaseResponse<SystemConfigDepartment>();
            var errors = Validate<SystemConfigDepartment>(model, new SystemConfigDepartmentValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<SystemConfigDepartment> errResponse = new BaseResponse<SystemConfigDepartment>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _systemConfigDepartmentRepository.Add(model);
            }
            catch (Exception ex)

            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<SystemConfigDepartment> UpdateSystemConfigDepartment(SystemConfigDepartment model)
        {
            BaseResponse<SystemConfigDepartment> response = new BaseResponse<SystemConfigDepartment>();
            var errors = Validate<SystemConfigDepartment>(model, new SystemConfigDepartmentValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<SystemConfigDepartment> errResponse = new BaseResponse<SystemConfigDepartment>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                SystemConfigDepartment systemConfig = _systemConfigDepartmentRepository.GetAll().Where(n => n.ConfigId == model.ConfigId && n.DepartmentId == model.DepartmentId).FirstOrDefault();
                if (systemConfig == null)
                {
                    try
                    {
                        model.CreatedOn = DateTime.Now;
                        response.Value = _systemConfigDepartmentRepository.Add(model);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    model.EditedOn = DateTime.Now;
                    response.Value = _systemConfigDepartmentRepository.Update(systemConfig, model);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<SystemConfigDepartment> GetSystemConfigDepartmentValue(SystemConfigDepartmentQuery query)
        {
            var response = new BaseResponse<SystemConfigDepartment>();
            try
            {
                SystemConfigDepartment systemConfigDepartment = new SystemConfigDepartment();
                systemConfigDepartment.Value = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(query);
                response.Value = systemConfigDepartment;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse<SystemConfigDepartment> GetSystemConfigDepartmentByConfigIdAndDepartmentId(int departmentId, int configId)
        {
            var response = new BaseResponse<SystemConfigDepartment>();
            try
            {
                response.Value = _systemConfigDepartmentRepository.GetAll().FirstOrDefault(n => n.ConfigId == configId && n.DepartmentId == departmentId);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        #endregion System Config Department

        #region LoginHistory
        public BaseResponse<LoginHistory> AddLoginHistory(LoginHistory model)
        {
            var response = new BaseResponse<LoginHistory>();
            var errors = Validate<LoginHistory>(model, new LoginHistoryValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<LoginHistory> errResponse = new BaseResponse<LoginHistory>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                response.Value = _loginHistoryRepository.AddLoginHistory(model);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseListResponse<SPGetLoginHistory_Result> FilterLoginHistory(LoginHistoryQuery query)
        {
            var response = new BaseListResponse<SPGetLoginHistory_Result>();
            int count = 0;
            try
            {
                response.Data = _loginHistoryRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        #endregion LoginHistory

        #region ApplicationLogging
        public BaseListResponse<SPGetApplicationLogging_Result> FilterApplicationLogging(ApplicationLoggingQuery query)
        {
            var response = new BaseListResponse<SPGetApplicationLogging_Result>();
            int count = 0;
            try
            {
                response.Data = _applicationLoggingRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<ApplicationLogging> GetApplicationLoggingById(int id)
        {
            var response = new BaseResponse<ApplicationLogging>();
            try
            {
                response.Value = _applicationLoggingRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse DeleteApplicationLogging(ComplexApplicationLogging applicationLogging)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                if (!string.IsNullOrEmpty(applicationLogging.Type))
                {
                    string[] arr = applicationLogging.Type.Split(',');
                    foreach (var item in arr)
                    {
                        _applicationLoggingRepository.DeleteMulti(x => x.CreatedOn.Day >= applicationLogging.FromDate.Day && x.CreatedOn.Month >= applicationLogging.FromDate.Month && x.CreatedOn.Year >= applicationLogging.FromDate.Year && x.CreatedOn.Day <= applicationLogging.ToDate.Day && x.CreatedOn.Month <= applicationLogging.ToDate.Month && x.CreatedOn.Year <= applicationLogging.ToDate.Year && x.ModelName == item);

                    }
                }
                else
                {
                    _applicationLoggingRepository.DeleteMulti(x => x.CreatedOn.Day >= applicationLogging.FromDate.Day && x.CreatedOn.Month >= applicationLogging.FromDate.Month && x.CreatedOn.Year >= applicationLogging.FromDate.Year && x.CreatedOn.Day <= applicationLogging.ToDate.Day && x.CreatedOn.Month <= applicationLogging.ToDate.Month && x.CreatedOn.Year <= applicationLogging.ToDate.Year);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        #endregion

        #region UserNotification
        public BaseResponse<UserNotification> AddUserNotification(UserNotification model)
        {

            var response = new BaseResponse<UserNotification>();
            var errors = Validate<UserNotification>(model, new UserNotificationValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<UserNotification> errResponse = new BaseResponse<UserNotification>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            var userNotification = _userNotificationRepository.FindBy(x => x.UserId == model.UserId && x.DeviceId == model.DeviceId).FirstOrDefault();
            if (userNotification != null)
            {
                userNotification.ClientId = model.ClientId;
                response.Value = _userNotificationRepository.Edit(userNotification);
                return response;
            }
            try
            {
                response.Value = _userNotificationRepository.Add(model);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<NotificationCenter> AddItemToNotificationCenter(NotificationCenter model)
        {

            var response = new BaseResponse<NotificationCenter>();
            var errors = Validate<NotificationCenter>(model, new NotificationCenterValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<NotificationCenter> errResponse = new BaseResponse<NotificationCenter>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                response.Value = _notificationCenterRepository.Add(model);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseListResponse<SPGetNotificationCenter_Result> GetNotifcationCenter(NotificationCenterQuery query)
        {
            var response = new BaseListResponse<SPGetNotificationCenter_Result>();
            int count = 0;
            try
            {
                response.Data = _notificationCenterRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<NotificationCenter> UpdateNotificationCenter(NotificationCenter model)
        {
            var response = new BaseResponse<NotificationCenter>();
            try
            {
                var notificationCenter = _notificationCenterRepository.GetById(model.Id);
                if (notificationCenter != null)
                {
                    if (notificationCenter.GroupId != null)
                    {
                        var listUpdateNotificationCenter = _notificationCenterRepository.FindBy(x => x.ReceivedUserId == notificationCenter.ReceivedUserId && x.GroupId == notificationCenter.GroupId).ToList();
                        foreach (var item in listUpdateNotificationCenter)
                        {
                            item.HaveSeen = model.HaveSeen;
                            _notificationCenterRepository.Edit(item);
                        }
                        // update recode root
                        var notifiRoot = _notificationCenterRepository.GetById(notificationCenter.GroupId.Value);
                        notifiRoot.HaveSeen = model.HaveSeen;
                        _notificationCenterRepository.Edit(notifiRoot);
                        notificationCenter.HaveSeen = model.HaveSeen;
                        response.Value = notificationCenter;
                    }
                    else
                    {
                        var listUpdateNotificationCenter = _notificationCenterRepository.FindBy(x => x.ReceivedUserId == notificationCenter.ReceivedUserId && x.GroupId == notificationCenter.Id).ToList();
                        foreach (var item in listUpdateNotificationCenter)
                        {
                            item.HaveSeen = model.HaveSeen;
                            response.Value = _notificationCenterRepository.Edit(item);
                        }
                        // update recode root                        
                        notificationCenter.HaveSeen = model.HaveSeen;
                        response.Value = _notificationCenterRepository.Edit(notificationCenter);
                    }                     
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.ToString();
            }
            return response;
        }

        public BaseResponse<int> GetCountNotificationUnread(string userId, string deviceId)
        {
            var response = new BaseResponse<int>();
            try
            {
                response.Value = _notificationCenterRepository.GetCountNotificationUnread(userId, deviceId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.ToString();
            }
            return response;
        }
        #endregion UserNotification

    }
}
