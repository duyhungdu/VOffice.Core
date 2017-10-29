using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Model.Validators;
using VOffice.Repository;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace VOffice.ApplicationService.Implementation
{
    public partial class TaskService : BaseService, ITaskService
    {
        protected readonly TaskRepository _taskRepository;
        protected readonly TaskAssigneeRepository _taskAssigneeRepository;
        protected readonly SystemConfigDepartmentRepository _systemConfigDepartmentRepository;
        protected readonly TaskDocumentsRepository _taskDocumentRepository;
        protected readonly DocumentReceivedRepository _documentReceivedRepository;
        protected readonly DocumentRecipentRepository _documentRecipentRepository;
        protected readonly ProjectRepository _projectRepository;
        protected readonly TaskTypeRepository _taskTypeRepository;
        protected readonly StatusRepository _statusRepository;
        protected readonly TaskAttachmentRepository _taskAttachmentRepository;
        protected readonly TaskOpinionRepository _taskOpinionRepository;
        protected readonly ApplicationLoggingRepository _applicationLoggingRepository;
        protected readonly TaskActivityRepository _taskActivityRepository;
        protected readonly AspNetUsersRepository _userRepository;
        protected readonly DepartmentRepository _departmentRepository;
        protected readonly StaffRepository _staffRepository;
        protected readonly CategoryService _categoryService;
        protected readonly UserNotificationRepository _userNotificationRepository;
        protected readonly NotificationCenterRepository _notificationCenterRepository;
        public TaskService()
        {
            _taskRepository = new TaskRepository();
            _taskAssigneeRepository = new TaskAssigneeRepository();
            _systemConfigDepartmentRepository = new SystemConfigDepartmentRepository();
            _taskDocumentRepository = new TaskDocumentsRepository();
            _documentReceivedRepository = new DocumentReceivedRepository();
            _documentRecipentRepository = new DocumentRecipentRepository();
            _projectRepository = new ProjectRepository();
            _taskTypeRepository = new TaskTypeRepository();
            _statusRepository = new StatusRepository();
            _taskAttachmentRepository = new TaskAttachmentRepository();
            _taskOpinionRepository = new TaskOpinionRepository();
            _applicationLoggingRepository = new ApplicationLoggingRepository();
            _taskActivityRepository = new TaskActivityRepository();
            _userRepository = new AspNetUsersRepository();
            _departmentRepository = new DepartmentRepository();
            _staffRepository = new StaffRepository();
            _categoryService = new CategoryService();
            _userNotificationRepository = new UserNotificationRepository();
            _notificationCenterRepository = new NotificationCenterRepository();
        }
        #region Task
        public BaseResponse<VOffice.Model.Task> GetTaskById(int id)
        {
            var response = new BaseResponse<VOffice.Model.Task>();
            try
            {
                response.Value = _taskRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse<SPGetTaskDetailById_Result> GetTaskDetailById(int id)
        {
            var response = new BaseResponse<SPGetTaskDetailById_Result>();
            try
            {
                response.Value = _taskRepository.SPGetTaskDetailById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse<ComplexTaskResponse> GetComplexTaskDetailById(int id)
        {
            var response = new BaseResponse<ComplexTaskResponse>();
            var complexTaskResponse = new ComplexTaskResponse();
            var taskDetail = new SPGetTaskDetailById_Result();
            try
            {
                taskDetail = _taskRepository.SPGetTaskDetailById(id);
                if (taskDetail != null)
                {
                    complexTaskResponse.Active = taskDetail.Active;
                    complexTaskResponse.Code = taskDetail.Code;
                    complexTaskResponse.ContactInformation = taskDetail.ContactInformation;
                    complexTaskResponse.CreatedBy = taskDetail.CreatedBy;
                    complexTaskResponse.CreatedOn = taskDetail.CreatedOn;
                    complexTaskResponse.CustomerId = taskDetail.CustomerId;
                    complexTaskResponse.Deleted = taskDetail.Deleted;
                    complexTaskResponse.DepartmentId = taskDetail.DepartmentId;
                    complexTaskResponse.Description = taskDetail.Description;
                    complexTaskResponse.DueDate = taskDetail.DueDate;
                    complexTaskResponse.EditedBy = taskDetail.EditedBy;
                    complexTaskResponse.EditedOn = taskDetail.EditedOn;
                    complexTaskResponse.EndDate = taskDetail.EndDate;
                    complexTaskResponse.Estimated = taskDetail.Estimated;
                    complexTaskResponse.Id = taskDetail.Id;
                    complexTaskResponse.Order = taskDetail.Order;
                    complexTaskResponse.Priority = taskDetail.Priority;
                    complexTaskResponse.ProjectId = taskDetail.ProjectId;
                    complexTaskResponse.Rating = taskDetail.Rating;
                    complexTaskResponse.Result = taskDetail.Result;
                    complexTaskResponse.StartDate = taskDetail.StartDate;
                    complexTaskResponse.StatusId = taskDetail.StatusId;
                    complexTaskResponse.TaskTypeId = taskDetail.TaskTypeId;
                    complexTaskResponse.TimeSpent = taskDetail.TimeSpent;
                    complexTaskResponse.Title = taskDetail.Title;
                    try
                    {
                        complexTaskResponse.ResponseTaskActivities = _taskActivityRepository.GetTaskActivityByTaskId(id);

                        if (taskDetail.DueDate.HasValue)
                        {
                            SPGetTaskActivityByTaskId_Result dueDateActivity = new SPGetTaskActivityByTaskId_Result();
                            dueDateActivity.Action = "DUEDATE";
                            var dueDate = taskDetail.DueDate.Value;
                            dueDateActivity.CreatedOn = new DateTime(dueDate.Year, dueDate.Month, dueDate.Day, 23, 59, 0);
                            dueDateActivity.ActivityContent = "Thời hạn hoàn thành";
                            dueDateActivity.Avatar = "";
                            dueDateActivity.CreatedBy = taskDetail.CreatedBy;
                            dueDateActivity.Description = "Thời hạn hoàn thành";
                            dueDateActivity.Display = true;
                            dueDateActivity.EditedBy = taskDetail.CreatedBy;
                            dueDateActivity.EditedOn = taskDetail.DueDate.Value;
                            dueDateActivity.FlowDescription = "Thời hạn hoàn thành";
                            dueDateActivity.FullName = "";
                            dueDateActivity.Id = 0;
                            dueDateActivity.RecordId = taskDetail.Id;
                            dueDateActivity.Type = 1;
                            complexTaskResponse.ResponseTaskActivities.Add(dueDateActivity);

                            var expiryDate = new DateTime(taskDetail.DueDate.Value.AddDays(1).Year, taskDetail.DueDate.Value.AddDays(1).Month, taskDetail.DueDate.Value.AddDays(1).Day, 0, 0, 0);
                            if (expiryDate < DateTime.Now)
                            {
                                bool done = false;
                                StatusQuery query = new StatusQuery();
                                SPGetStatusByCode_Result taskStatus = new SPGetStatusByCode_Result();
                                query.Type = "TASK";
                                query.Code = "RESOLVED";
                                taskStatus = _statusRepository.GetStatusByCode(query).FirstOrDefault();
                                if (taskDetail.StatusId == taskStatus.Id)
                                {
                                    done = true;
                                }
                                query.Code = "CLOSED";
                                taskStatus = _statusRepository.GetStatusByCode(query).FirstOrDefault();
                                if (taskDetail.StatusId == taskStatus.Id)
                                {
                                    done = true;
                                }
                                if (!done)
                                {
                                    SPGetTaskActivityByTaskId_Result expiryDateActivity = new SPGetTaskActivityByTaskId_Result();
                                    expiryDateActivity.Action = "EXPIRED";
                                    expiryDateActivity.CreatedOn = expiryDate;
                                    expiryDateActivity.ActivityContent = "Quá hạn xử lý";
                                    expiryDateActivity.Avatar = "";
                                    expiryDateActivity.CreatedBy = taskDetail.CreatedBy;
                                    expiryDateActivity.Description = "Quá hạn xử lý";
                                    expiryDateActivity.Display = true;
                                    expiryDateActivity.EditedBy = taskDetail.CreatedBy;
                                    expiryDateActivity.EditedOn = expiryDate;
                                    expiryDateActivity.FlowDescription = "Quá hạn xử lý";
                                    expiryDateActivity.FullName = "";
                                    expiryDateActivity.Id = 0;
                                    expiryDateActivity.RecordId = taskDetail.Id;
                                    expiryDateActivity.Type = 1;
                                    complexTaskResponse.ResponseTaskActivities.Add(expiryDateActivity);
                                    complexTaskResponse.ResponseTaskActivities = complexTaskResponse.ResponseTaskActivities.OrderBy(x => x.CreatedOn).ToList();
                                }
                            }

                        }
                    }
                    catch { }
                    complexTaskResponse.ResponseTaskActivities = complexTaskResponse.ResponseTaskActivities.OrderBy(x => x.CreatedOn).ToList();
                    response.Value = complexTaskResponse;
                }

            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseResponse<Task> AddTask(VOffice.Model.Task model)
        {
            var response = new BaseResponse<Task>();
            var errors = Validate<Task>(model, new TaskValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Task> errResponse = new BaseResponse<Task>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                #region Generate TaskCode
                string taskCode = "";
                string taskCodeText = "";
                string taskCodeNumber = "";
                try
                {

                    var systemConfigDepartmentQuery = new SystemConfigDepartmentQuery();
                    systemConfigDepartmentQuery.DepartmentId = model.DepartmentId;
                    systemConfigDepartmentQuery.DefaultValue = "TASK";
                    systemConfigDepartmentQuery.Title = "TASKCODE";
                    taskCodeText = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(systemConfigDepartmentQuery);
                    systemConfigDepartmentQuery.DefaultValue = "0";
                    systemConfigDepartmentQuery.Title = "TASKNUMBER";
                    string taskCodeNumberConfig = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(systemConfigDepartmentQuery);
                    taskCodeNumber = (int.Parse(taskCodeNumberConfig) + 1).ToString("D4");
                    taskCode = taskCodeText + taskCodeNumber;

                }
                catch
                {

                }
                #endregion Generate TaskCode
                model.Code = taskCode;
                response.Value = _taskRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "Task", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
                //Update SystemConfigDepartment
                SystemConfigDepartment taskCodeNumberSystemConfigDepartment = new SystemConfigDepartment();
                taskCodeNumberSystemConfigDepartment = _systemConfigDepartmentRepository.GetByTitle("TASKNUMBER", model.DepartmentId);
                if (taskCodeNumberSystemConfigDepartment != null)
                {
                    taskCodeNumberSystemConfigDepartment.Value = taskCodeNumber;
                    _systemConfigDepartmentRepository.Edit(taskCodeNumberSystemConfigDepartment);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<Task> UpdateTask(VOffice.Model.Task model)
        {
            VOffice.Model.Task oldModel = new Task();
            oldModel = _taskRepository.GetById(model.Id);
            oldModel.Code = model.Code;
            oldModel.Title = model.Title;
            oldModel.Description = model.Description;
            oldModel.DepartmentId = model.DepartmentId;
            oldModel.ProjectId = model.ProjectId;
            oldModel.TaskTypeId = model.TaskTypeId;
            oldModel.Order = model.Order;
            oldModel.Priority = model.Priority;
            oldModel.DueDate = model.DueDate;
            oldModel.StartDate = model.StartDate;
            oldModel.EndDate = model.EndDate;
            oldModel.CustomerId = model.CustomerId;
            oldModel.ContactInformation = model.ContactInformation;
            oldModel.Rating = model.Rating;
            oldModel.Result = model.Result;
            oldModel.Active = model.Active;
            oldModel.Deleted = model.Deleted;
            oldModel.CreatedBy = model.CreatedBy;
            oldModel.CreatedOn = model.CreatedOn;
            oldModel.EditedBy = model.EditedBy;
            oldModel.EditedOn = model.EditedOn;
            Status oldStatus = _statusRepository.GetById(oldModel.StatusId);
            Status newStatus = _statusRepository.GetById(model.StatusId);

            string oldStatusCode = oldStatus != null ? oldStatus.Code : "DEFAULT";
            string newStatusCode = newStatus != null ? newStatus.Code : "DEFAULT";

            switch (oldStatusCode)
            {
                case "DEFAULT":
                    oldModel.StatusId = model.StatusId;
                    break;
                case "INPROCESS":
                    oldModel.StatusId = model.StatusId;
                    break;
                case "RESOLVED":
                    switch (newStatusCode)
                    {
                        case "CLOSED":
                            oldModel.StatusId = model.StatusId;
                            break;
                        case "REOPEN":
                            oldModel.StatusId = model.StatusId;
                            break;
                    }
                    break;
                case "CLOSED":
                    switch (newStatusCode)
                    {
                        case "REOPEN":
                            oldModel.StatusId = model.StatusId;
                            break;
                    }
                    break;
                case "REOPEN":
                    switch (newStatusCode)
                    {
                        case "RESOLVED":
                            oldModel.StatusId = model.StatusId;
                            break;
                        case "CLOSED":
                            oldModel.StatusId = model.StatusId;
                            break;
                    }
                    break;
            }
            var response = new BaseResponse<Task>();
            var errors = Validate<Task>(oldModel, new TaskValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Task> errResponse = new BaseResponse<Task>(oldModel, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                response.Value = _taskRepository.Edit(oldModel);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "Task", oldModel.Id.ToString(), "", "", oldModel, "", HttpContext.Current.Request.UserHostAddress, oldModel.CreatedBy);
                }
                catch { }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public System.Threading.Tasks.Task<BaseResponse<VOffice.Model.Task>> AddSetOfTask(ComplexTask model)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                BaseResponse<VOffice.Model.Task> response = new BaseResponse<VOffice.Model.Task>();
                VOffice.Model.Task task = new Model.Task();
                string taskCode = "";
                string taskCodeText = "";
                string taskCodeNumber = "";
                List<string> listUserRelate = new List<string>();
                string mainAssignee = "";
                string mainAssigneeFullName = "";
                try
                {
                    #region Generate TaskCode

                    try
                    {

                        var systemConfigDepartmentQuery = new SystemConfigDepartmentQuery();
                        systemConfigDepartmentQuery.DepartmentId = model.DepartmentId;
                        systemConfigDepartmentQuery.DefaultValue = "TASK";
                        systemConfigDepartmentQuery.Title = "TASKCODE";
                        taskCodeText = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(systemConfigDepartmentQuery);
                        systemConfigDepartmentQuery.DefaultValue = "0";
                        systemConfigDepartmentQuery.Title = "TASKNUMBER";
                        string taskCodeNumberConfig = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(systemConfigDepartmentQuery);
                        taskCodeNumber = (int.Parse(taskCodeNumberConfig) + 1).ToString("D4");
                        taskCode = taskCodeText + taskCodeNumber;
                        task.Code = taskCode;
                    }
                    catch
                    {

                    }
                    #endregion Generate TaskCode
                }
                catch
                {
                    response.IsSuccess = false;
                    response.Message = "Không thể tạo mã công việc. Xin vui lòng kiểm tra thiết lập hệ thống.";
                    return response;
                }
                task.Title = model.Title;
                task.Description = !string.IsNullOrEmpty(model.Description) ? model.Description : "";
                task.DepartmentId = model.DepartmentId;
                task.ProjectId = model.ProjectId;
                task.TaskTypeId = model.TaskTypeId;
                StatusQuery query = new StatusQuery();
                query.Type = "TASK";
                query.Code = "DEFAULT";
                try
                {
                    task.StatusId = _statusRepository.GetStatusByCode(query).FirstOrDefault().Id;
                }
                catch
                {
                    response.IsSuccess = false;
                    response.Message = "Không thể thiết lập trạng thái công việc. Xin vui lòng kiểm tra thiết lập hệ thống.";
                    return response;
                }
                task.Order = model.Order;
                task.Priority = model.Priority;
                task.DueDate = model.DueDate;
                if (model.StartDate != null)
                {
                    task.StartDate = model.StartDate;
                }
                else
                {
                    task.StartDate = DateTime.Now;
                }
                task.Estimated = model.Estimated;
                task.TimeSpent = model.TimeSpent;
                task.EndDate = model.EndDate;
                task.CustomerId = model.CustomerId;
                task.ContactInformation = model.ContactInformation;
                task.Rating = model.Rating;
                task.Result = model.Result;
                task.Active = model.Active;
                task.Deleted = model.Deleted;
                task.CreatedOn = DateTime.Now;
                task.CreatedBy = model.CreatedBy;
                task.EditedOn = DateTime.Now;
                task.EditedBy = model.EditedBy;
                var errors = Validate<Task>(task, new TaskValidator());
                if (errors.Count() > 0)
                {
                    BaseResponse<Task> errResponse = new BaseResponse<Task>(task, errors);
                    errResponse.IsSuccess = false;
                    return errResponse;
                }

                var addTaskResponse = _taskRepository.Add(task);

                if (addTaskResponse == null)
                {
                    response.Message = "Không thể tạo công việc. Xin vui lòng thử lại.";
                    return response;
                }
                else
                {
                    // listUserRelate.Add(model.CreatedBy);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "CREATE", "Task", task.Id.ToString(), "", "", task, "", HttpContext.Current.Request.UserHostAddress, task.CreatedBy);
                    }
                    catch { }
                    //Update SystemConfigDepartment
                    SystemConfigDepartment taskCodeNumberSystemConfigDepartment = new SystemConfigDepartment();
                    taskCodeNumberSystemConfigDepartment = _systemConfigDepartmentRepository.GetByTitle("TASKNUMBER", model.DepartmentId);
                    if (taskCodeNumberSystemConfigDepartment != null)
                    {
                        taskCodeNumberSystemConfigDepartment.Value = taskCodeNumber;
                        _systemConfigDepartmentRepository.Edit(taskCodeNumberSystemConfigDepartment);
                    }
                    //Response setting
                    response.Value = addTaskResponse;
                }
                //Done add Task
                //Loop list TaskAssignee -> add
                List<TaskAssignee> listTaskAssignee = model.TaskAssignees.ToList();
                if (listTaskAssignee.Count > 0)
                {
                    foreach (var assignee in listTaskAssignee)
                    {
                        try
                        {
                            assignee.TaskId = addTaskResponse.Id;
                            assignee.CreatedOn = DateTime.Now;
                            _taskAssigneeRepository.Add(assignee);
                            if (assignee.Assignee == false)
                            {
                                listUserRelate.Add(assignee.UserId);
                            }
                            else
                            {
                                mainAssignee = assignee.UserId;

                            }
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskAssignee", assignee.Id.ToString(), "", "", assignee, "", HttpContext.Current.Request.UserHostAddress, assignee.CreatedBy);
                            }
                            catch { }
                        }
                        catch
                        {
                            response.IsSuccess = false;
                            response.Message = "Không thể tạo người xử lý công việc. Xin vui lòng thử lại.";
                        }
                    }
                    try
                    {
                        TaskActivity createActivity = new TaskActivity();
                        createActivity.RecordId = task.Id;
                        createActivity.Action = "ADD";
                        createActivity.Type = 1;
                        createActivity.Display = true;
                        createActivity.FlowDescription = "tạo công việc";
                        createActivity.Description = "tạo công việc: " + task.Title;
                        createActivity.TaskField = "";
                        try
                        {
                            createActivity.OldValue = _staffRepository.GetStaffByUserId(mainAssignee).FullName;
                        }
                        catch
                        { }
                        createActivity.NewValue = "";
                        createActivity.CreatedOn = DateTime.Now;
                        createActivity.CreatedBy = task.CreatedBy;
                        createActivity.EditedBy = task.EditedBy;
                        createActivity.EditedOn = DateTime.Now;
                        _taskActivityRepository.Add(createActivity);
                    }
                    catch { }
                    //SendEmail
                    var userProfile = _departmentRepository.GetUserMainOrganization(model.CreatedBy);
                    #region Send email
                    try
                    {
                        //mainAssignee: Người xử lý chính
                        //listUserRelate: Người tạo, đồng xử lý, giám sát
                        string emailTo = _userRepository.GetUserByUserId(mainAssignee).Email;
                        string cc = "";
                        foreach (string u in listUserRelate)
                        {
                            cc += _userRepository.GetUserByUserId(u).Email + ",";
                        }

                        string content = "";
                        string subject = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = model.DepartmentId, Title = "TASKNOTICEEMAILSUBJECT", DefaultValue = "[VOffice] - Thông báo - Quản lý công việc" });

                        string domain = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = model.DepartmentId, Title = "APP_DOMAIN", DefaultValue = "http://congvan.veph.vn/" });

                        string taskDetailUrl = domain + "#!/chi-tiet-cong-viec/" + task.Id.ToString() + "/0////1/true/true";
                        subject += " - Quản lý công việc: " + task.Code;
                        string temp = @"<div style='border: 3px solid #15c; padding: 10px; overflow: hidden;'>
                              <table style='width:100%'>
                                        <tr>
                                            <td><a style='text-decoration:none;' href='" + taskDetailUrl + @"'>[" + task.Code + "] - " + task.Title + @"</a></td>
                                        </tr>
                                        <tr>
                                            <td><div style='border-bottom:1px dotted #ddd; margin:10px 0; width:100%'></div></td>
                                        </tr>
                                        <tr>
                                            <td>{0}</td>
                                        </tr>
                                        <tr>
                                            <td>{1}</td>
                                        </tr>
                                            </table></div> ";

                        string actionNotice = "";
                        if (userProfile != null)
                        {
                            actionNotice = "<b>" + userProfile.FullName + "</b>" + " đã tạo công việc: " + model.Code + " - " + model.Title;
                            actionNotice += "<br/><span style = 'font-style:italic; font-size:10px; color:#888888' >" + DateTime.Now.ToString("dd/MM/yyyy | HH:mm") + "</ span>";
                        }
                        string contentNotice = model.Description;
                        content = string.Format(temp, actionNotice, contentNotice);
                        UtilityProvider.SendEmail(model.DepartmentId, emailTo, cc, "", subject, content);
                    }
                    catch
                    { }
                    #endregion Send email

                    // Send notification
                    #region send notification
                    var listUserSendNotifi = new List<SPGetUserNotification_Result>();
                    listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByUserId(mainAssignee));
                    var listUserRelateDistinct = listUserRelate.Distinct().ToList();
                    foreach (var u in listUserRelateDistinct)
                    {
                        var exist = listUserSendNotifi.Where(x => x.UserId == u).FirstOrDefault();
                        if (exist == null) listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByUserId(u));
                    }
                    NotificationCenter resultNotifiCenter = null;
                    NotificationCenter notifiFirst = null;
                    foreach (var item in listUserSendNotifi)
                    {
                        FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                        fcmNotificationCenter.Avatar = userProfile.Avatar;
                        fcmNotificationCenter.FullName = userProfile.FullName;
                        fcmNotificationCenter.Content = "đã tạo công việc mới: \n" + model.Title;
                        fcmNotificationCenter.Title = "vOffice";
                        fcmNotificationCenter.RecordNumber = model.Code;
                        fcmNotificationCenter.RecordId = addTaskResponse.Id;
                        fcmNotificationCenter.Type = (int)NotificationCode.CreateTask;
                        fcmNotificationCenter.CreatedBy = model.CreatedBy;
                        fcmNotificationCenter.CreatedOn = DateTime.Now;
                        fcmNotificationCenter.HaveSeen = false;
                        fcmNotificationCenter.ReceivedUserId = item.UserId;
                        fcmNotificationCenter.DeviceId = item.DeviceId;
                        if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                        {
                            fcmNotificationCenter.GroupId = notifiFirst.Id;
                        }
                        var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                        resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                        fcmNotificationCenter.Id = resultNotifiCenter.Id;
                        if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                        FCMPushNotification fcmPushNotification = new FCMPushNotification();
                        fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
                    }
                    #endregion

                }
                //Done add TaskAssignee
                //Loop list TaskDocument -> add
                List<TaskDocument> listTaskDocument = model.TaskDocuments.ToList();
                if (listTaskDocument.Count > 0)
                {
                    TaskDocument modelTaskDocument = null;
                    foreach (var taskDocument in listTaskDocument)
                    {
                        try
                        {
                            modelTaskDocument = new TaskDocument();
                            modelTaskDocument.TaskId = addTaskResponse.Id;

                            //modelTaskDocument.DocumentId = taskDocument.Id;
                            if (taskDocument.DocumentId != 0)
                            {
                                modelTaskDocument.DocumentId = taskDocument.DocumentId;
                            }
                            else
                            {
                                modelTaskDocument.DocumentId = taskDocument.Id;
                            }
                            modelTaskDocument.ReceivedDocument = taskDocument.ReceivedDocument;
                            modelTaskDocument.CreatedOn = DateTime.Now;
                            modelTaskDocument.CreatedBy = model.CreatedBy;

                            _taskDocumentRepository.Add(modelTaskDocument);
                            try
                            {
                                _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskDocument", modelTaskDocument.Id.ToString(), "", "", modelTaskDocument, "", HttpContext.Current.Request.UserHostAddress, modelTaskDocument.CreatedBy);
                            }
                            catch { }
                        }
                        catch
                        {
                            response.IsSuccess = false;
                            response.Message = "Không thể tạo người xử lý công việc. Xin vui lòng thử lại.";
                        }
                        //Done add TaskDocument
                        //Check one of document with one of TaskAssignee and and DocumentRecipent
                        foreach (var assignee in listTaskAssignee)
                        {
                            bool readable = _documentReceivedRepository.CheckUserDocumentReadable(assignee.UserId, taskDocument.Id, taskDocument.ReceivedDocument);
                            if (!readable)
                            {
                                try
                                {
                                    DocumentRecipent recipent = new DocumentRecipent();
                                    recipent.DocumentId = modelTaskDocument.DocumentId;
                                    recipent.ReceivedDocument = modelTaskDocument.ReceivedDocument;
                                    recipent.UserId = assignee.UserId;
                                    recipent.ForSending = false;
                                    recipent.Forwarded = false;
                                    recipent.Assigned = true;
                                    recipent.AddedDocumentBook = true;
                                    recipent.CreatedBy = model.CreatedBy;
                                    recipent.CreatedOn = DateTime.Now;
                                    _documentRecipentRepository.Add(recipent);
                                    try
                                    {
                                        _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentRecipent", recipent.Id.ToString(), "", "", recipent, "", HttpContext.Current.Request.UserHostAddress, recipent.CreatedBy);
                                    }
                                    catch { }
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
                string abc = response.Message;
                return response;
            });
        }
        public System.Threading.Tasks.Task<BaseResponse<VOffice.Model.Task>> UpdateSetOfTask(ComplexTask model)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                var response = new BaseResponse<VOffice.Model.Task>();
                VOffice.Model.Task task = new Model.Task();
                task = _taskRepository.GetById(model.Id);

                task.Title = model.Title;
                task.Description = !string.IsNullOrEmpty(model.Description) ? model.Description : "";
                task.ProjectId = model.ProjectId;
                task.TaskTypeId = model.TaskTypeId;
                task.Order = model.Order;
                task.Priority = model.Priority;
                if (model.StartDate != null)
                {
                    task.StartDate = model.StartDate;
                }
                else
                {
                    task.StartDate = DateTime.Now;
                }
                task.DueDate = model.DueDate;

                task.Estimated = model.Estimated;
                task.TimeSpent = model.TimeSpent;

                task.CustomerId = model.CustomerId;
                task.ContactInformation = model.ContactInformation;
                task.Rating = model.Rating;
                task.Result = model.Result;

                task.EditedOn = DateTime.Now;
                task.EditedBy = model.EditedBy;

                var addTaskResponse = _taskRepository.Edit(task);

                if (addTaskResponse == null)
                {
                    response.Message = "Không thể cập nhật công việc. Xin vui lòng thử lại.";
                    return response;
                }
                else
                {
                    string mainAssignee = "";
                    List<string> listUserRelate = new List<string>();
                    listUserRelate.Add(task.CreatedBy);
                    listUserRelate.Add(task.EditedBy);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "UPDATE", "Task", task.Id.ToString(), "", "", task, "", HttpContext.Current.Request.UserHostAddress, task.CreatedBy);
                    }
                    catch { }
                    List<TaskAssignee> listTaskAssignee = model.TaskAssignees.ToList();
                    if (listTaskAssignee.Count > 0)
                    {
                        List<SPGetTaskAssigneeByTaskId_Result> listAssigned = new List<SPGetTaskAssigneeByTaskId_Result>();
                        listAssigned = _taskAssigneeRepository.GetTaskAssigneeByTaskId(addTaskResponse.Id);
                        //Delete all assigned
                        foreach (var assigned in listAssigned)
                        {
                            foreach (var newAssignee in listTaskAssignee)
                            {
                                if (newAssignee.UserId == assigned.UserId)
                                {
                                    newAssignee.ViewDetail = assigned.ViewDetail;
                                    newAssignee.ViewOn = assigned.ViewOn;
                                }
                            }
                            TaskAssignee taskAssigneeModel = _taskAssigneeRepository.GetById(assigned.Id);
                            _taskAssigneeRepository.Delete(taskAssigneeModel);
                        }
                        foreach (var assignee in listTaskAssignee)
                        {
                            if (assignee.Assignee)
                            {
                                mainAssignee = assignee.UserId;
                            }
                            else
                            {
                                listUserRelate.Add(assignee.UserId);
                            }
                            try
                            {
                                assignee.TaskId = addTaskResponse.Id;
                                _taskAssigneeRepository.Add(assignee);
                                try
                                {
                                    _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskAssignee", assignee.Id.ToString(), "", "", assignee, "", HttpContext.Current.Request.UserHostAddress, assignee.CreatedBy);
                                }
                                catch { }
                            }
                            catch
                            {
                                response.IsSuccess = false;
                                response.Message = "Không thể tạo người xử lý công việc. Xin vui lòng thử lại.";
                            }
                        }
                        //SendEmail
                        #region Send email
                        var userProfile = _departmentRepository.GetUserMainOrganization(model.EditedBy);
                        try
                        {
                            //mainAssignee: Người xử lý chính
                            //listUserRelate: Người tạo, đồng xử lý, giám sát
                            string emailTo = _userRepository.GetUserByUserId(mainAssignee).Email;
                            string cc = "";
                            foreach (string u in listUserRelate)
                            {
                                cc += _userRepository.GetUserByUserId(u).Email + ",";
                            }
                            string content = "";
                            string subject = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = model.DepartmentId, Title = "TASKNOTICEEMAILSUBJECT", DefaultValue = "[VOffice] - Thông báo" });

                            string domain = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = model.DepartmentId, Title = "APP_DOMAIN", DefaultValue = "http://congvan.veph.vn/" });

                            string taskDetailUrl = domain + "#!/chi-tiet-cong-viec/" + task.Id.ToString() + "/0////1/true/true";
                            subject += " - Quản lý công việc: " + task.Code;
                            string temp = @"<div style='border: 3px solid #15c; padding: 10px; overflow: hidden;'>
                             <table style='width:100%'>
                                        <tr>
                                            <td><a style='text-decoration:none;' href='" + taskDetailUrl + @"'>[" + task.Code + "] - " + task.Title + @" </a></td>
                                        </tr>
                                        <tr>
                                            <td>
                                            <div style='border-bottom:1px dotted #ddd; margin:10px 0; width:100%'></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>{0}</td>
                                        </tr>
                                        <tr>
                                            <td>{1}</td>
                                        </tr>
                                        
                                            </table></div> ";

                            string actionNotice = "";
                            if (userProfile != null)
                            {
                                actionNotice = "<b>" + userProfile.FullName + "</b>" + " đã cập nhật công việc: " + model.Code + " - " + model.Title;
                                actionNotice += "<br/><span style = 'font-style:italic; font-size:10px; color:#888888' >" + model.CreatedOn.ToString("dd/MM/yyyy | HH:mm") + "</ span>";
                            }
                            string contentNotice = model.Description + "<br/>";
                            var taskDetail = _taskRepository.SPGetTaskDetailById(model.Id);
                            if (taskDetail != null)
                            {
                                contentNotice += "Người tạo: " + taskDetail.FullName + "<br/>";
                                contentNotice += "Trạng thái: " + taskDetail.StatusTitle + "<br/>";
                                contentNotice += "Ngày bắt đầu: " + (taskDetail.StartDate != null ? taskDetail.StartDate.Value.ToString("dd/MM/yyyy") : "Chưa xác định") + "<br/>";
                                if (taskDetail.DueDate != null)
                                {
                                    contentNotice += "Ngày hoàn thành dự kiến: " + (taskDetail.DueDate != null ? taskDetail.DueDate.Value.ToString("dd/MM/yyyy") : "Chưa xác định") + "<br/>";
                                }
                                contentNotice += "Khách hàng: " + (string.IsNullOrEmpty(taskDetail.CustomerName) != true ? taskDetail.CustomerName : "Chưa xác định") + "<br/>";
                                contentNotice += "Mảng công việc: " + (string.IsNullOrEmpty(taskDetail.TaskTypeTitle) != true ? taskDetail.TaskTypeTitle : "Chưa xác định") + "<br/>";
                                contentNotice += "Dự án: " + (string.IsNullOrEmpty(taskDetail.ProjectTitle) != true ? taskDetail.ProjectTitle : "Chưa xác định") + "<br/>";
                            }
                            content = string.Format(temp, actionNotice, contentNotice);
                            UtilityProvider.SendEmail(model.DepartmentId, emailTo, cc, "", subject, content);
                        }
                        catch
                        { }
                        #endregion Send email
                        #region Send notification
                        var listUserSendNotifi = new List<SPGetUserNotification_Result>();
                        var listUserRelateDistinct = listUserRelate.Distinct().ToList();
                        foreach (var u in listUserRelateDistinct)
                        {
                            if (u != model.EditedBy)
                            {
                                listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByUserId(u));
                    }
                        }
                        NotificationCenter resultNotifiCenter = null;
                        NotificationCenter notifiFirst = null;
                        foreach (var item in listUserSendNotifi)
                        {
                            FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                            fcmNotificationCenter.Avatar = userProfile.Avatar;
                            fcmNotificationCenter.FullName = userProfile.FullName;
                            fcmNotificationCenter.Title = "vOffice";
                            fcmNotificationCenter.RecordNumber = model.Code;
                            fcmNotificationCenter.RecordId = model.Id;
                            fcmNotificationCenter.CreatedBy = model.CreatedBy;
                            fcmNotificationCenter.CreatedOn = DateTime.Now;
                            fcmNotificationCenter.HaveSeen = false;
                            fcmNotificationCenter.ReceivedUserId = item.UserId;
                            fcmNotificationCenter.DeviceId = item.DeviceId;
                            fcmNotificationCenter.Content = "đã cập nhật công việc: \n" + model.Title;                            
                            fcmNotificationCenter.Type = (int)NotificationCode.CreateTask;
                            if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                            {
                                fcmNotificationCenter.GroupId = notifiFirst.Id;
                            }
                            var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                            resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                            fcmNotificationCenter.Id = resultNotifiCenter.Id;
                            if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                            FCMPushNotification fcmPushNotification = new FCMPushNotification();
                            fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
                        }
                        #endregion
                    }
                    //Loop Document
                    List<TaskDocument> listTaskDocument = model.TaskDocuments.ToList();

                    //Delete old list TaskDocuments
                    List<SPGetTaskDocumentByTaskId_Result> listAddedTaskDocument = new List<SPGetTaskDocumentByTaskId_Result>();
                    listAddedTaskDocument = _taskDocumentRepository.GetTaskDocumentByTaskId(addTaskResponse.Id);
                    //Delete all assigned
                    foreach (var document in listAddedTaskDocument)
                    {
                        TaskDocument taskDocumentModel = _taskDocumentRepository.GetById(document.TaskDocumentId);
                        _taskDocumentRepository.Delete(taskDocumentModel);
                    }
                    if (listTaskDocument.Count > 0)
                    {
                        TaskDocument modelTaskDocument = null;
                        foreach (var taskDocument in listTaskDocument)
                        {
                            try
                            {
                                modelTaskDocument = new TaskDocument();
                                modelTaskDocument.TaskId = addTaskResponse.Id;
                                modelTaskDocument.DocumentId = taskDocument.Id;
                                modelTaskDocument.ReceivedDocument = taskDocument.ReceivedDocument;
                                modelTaskDocument.CreatedOn = DateTime.Now;
                                modelTaskDocument.CreatedBy = model.CreatedBy;

                                _taskDocumentRepository.Add(modelTaskDocument);
                                try
                                {
                                    _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskDocument", modelTaskDocument.Id.ToString(), "", "", modelTaskDocument, "", HttpContext.Current.Request.UserHostAddress, modelTaskDocument.CreatedBy);
                                }
                                catch { }
                            }
                            catch
                            {
                                response.IsSuccess = false;
                                response.Message = "Không thể tạo người xử lý công việc. Xin vui lòng thử lại.";
                            }
                            //Done add TaskDocument
                            //Check one of document with one of TaskAssignee and and DocumentRecipent
                            foreach (var assignee in listTaskAssignee)
                            {
                                bool readable = _documentReceivedRepository.CheckUserDocumentReadable(assignee.UserId, taskDocument.Id, taskDocument.ReceivedDocument);
                                if (!readable)
                                {
                                    try
                                    {
                                        DocumentRecipent recipent = new DocumentRecipent();
                                        recipent.DocumentId = modelTaskDocument.DocumentId;
                                        recipent.ReceivedDocument = modelTaskDocument.ReceivedDocument;
                                        recipent.UserId = assignee.UserId;
                                        recipent.ForSending = false;
                                        recipent.Forwarded = false;
                                        recipent.Assigned = true;
                                        recipent.AddedDocumentBook = true;
                                        recipent.CreatedBy = model.CreatedBy;
                                        recipent.CreatedOn = DateTime.Now;
                                        _documentRecipentRepository.Add(recipent);
                                        try
                                        {
                                            _applicationLoggingRepository.Log("EVENT", "CREATE", "DocumentRecipent", recipent.Id.ToString(), "", "", recipent, "", HttpContext.Current.Request.UserHostAddress, recipent.CreatedBy);
                                        }
                                        catch { }
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                    }
                }
                response.Value = addTaskResponse;
                return response;
            });
        }
        public BaseListResponse<SPGetTask_Result> FilterTask(TaskQuery query)
        {
            var response = new BaseListResponse<SPGetTask_Result>();
            int count = 0;
            try
            {
                response.Data = _taskRepository.Filter(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<string> GetTaskCode(int departmentId)
        {
            var response = new BaseResponse<string>();
            try
            {
                response.Value = _taskRepository.GenerateTaskCode(departmentId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalTask(int id)
        {
            BaseResponse response = new BaseResponse();
            Task model = _taskRepository.GetById(id);
            try
            {
                model.Deleted = true;
                _taskRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "DELETE", "Task", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetTaskByDocumentId_Result> GetTaskByDocumentId(int docId, bool receivedDoc, string userId)
        {
            var response = new BaseListResponse<SPGetTaskByDocumentId_Result>();
            try
            {
                int count = 0;
                response.Data = _taskRepository.SPGetTaskByDocumentId(docId, receivedDoc, userId, out count);
                response.TotalItems = count;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<int> CountNewTask(TaskQuery query)
        {
            var response = new BaseResponse<int>();
            try
            {
                var defaultTaskStatus = _statusRepository.GetStatusByCode(new StatusQuery { Type = "TASK", Code = "DEFAULT" });
                query.StatusId = defaultTaskStatus.FirstOrDefault() != null ? defaultTaskStatus.FirstOrDefault().Id : 0;
                response.Value = _taskRepository.CountDefaultTask(query);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseResponse<UserTaskAnalystic> CountUserTask(TaskQuery query)
        {
            var response = new BaseResponse<UserTaskAnalystic>();
            UserTaskAnalystic userTaskAnalystic = new UserTaskAnalystic();
            if (query.TimeType == "WEEK")
            {
                var monday = DateTime.Now.GetFirstDayOfWeek();
                query.FromDate = new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0);
                query.ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
            }
            else if (query.TimeType == "MONTH")
            {
                query.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                query.ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
            }
            else if (query.TimeType == "YEAR")
            {
                query.FromDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
                query.ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);
            }
            try
            {
                var defaultTaskStatus = _statusRepository.GetStatusByCode(new StatusQuery { Type = "TASK", Code = "DEFAULT" });
                query.StatusId = defaultTaskStatus.FirstOrDefault().Id;
                userTaskAnalystic.DefaultNumber = _taskRepository.CountTaskInStatus(query);

                var inprocessStatus = _statusRepository.GetStatusByCode(new StatusQuery { Type = "TASK", Code = "INPROCESS" });
                query.StatusId = inprocessStatus.FirstOrDefault().Id;
                userTaskAnalystic.InprocessNumber = _taskRepository.CountTaskInStatus(query);

                var resolvedStatus = _statusRepository.GetStatusByCode(new StatusQuery { Type = "TASK", Code = "RESOLVED" });
                query.StatusId = resolvedStatus.FirstOrDefault().Id;
                userTaskAnalystic.ResolvedNumber = _taskRepository.CountTaskInStatus(query);

                var closedStatus = _statusRepository.GetStatusByCode(new StatusQuery { Type = "TASK", Code = "CLOSED" });
                query.StatusId = closedStatus.FirstOrDefault().Id;
                userTaskAnalystic.ClosedNumber = _taskRepository.CountTaskInStatus(query);

                var reopenStatus = _statusRepository.GetStatusByCode(new StatusQuery { Type = "TASK", Code = "REOPEN" });
                query.StatusId = reopenStatus.FirstOrDefault().Id;
                userTaskAnalystic.ReopenNumber = _taskRepository.CountTaskInStatus(query);


                userTaskAnalystic.ExpriedNumber = _taskRepository.CountExpiredTask(query);

                response.Value = userTaskAnalystic;
            }
            catch
            {

            }

            return response;
        }

        public BaseListResponse<SPGetTaskAdvance_Result> GetTaskAdvance(TaskQuery query)
        {
            var response = new BaseListResponse<SPGetTaskAdvance_Result>();
            int count = 0;
            try
            {
                response.Data = _taskRepository.FilterAdvance(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<string> DownloadTaskAdvance(TaskQuery query)
        {
            var source = _taskRepository.GetTaskReport(query);
            var staff = _staffRepository.GetStaffByUserId(query.UserId);
            BaseListResponse<Status> lstStatus = _categoryService.GetStatusByType("TASK");
            DepartmentRepository deparmentRepository = new DepartmentRepository();
            StringBuilder content = new StringBuilder();
            var response = new BaseResponse<string>();
            var rootDeparment = deparmentRepository.FindBy(x => x.ParentId == 0).FirstOrDefault();
            var deparment = deparmentRepository.FindBy(x => x.Id == query.DepartmentId).FirstOrDefault();
            content.Append("<html>");
            content.Append("<body>");
            content.Append("<table style='width:100%'>");
            content.Append("<tr>").Append("<td style='width: 65.5424 %;'>");
            content.Append(rootDeparment == null ? "" : rootDeparment.Name.ToUpper());
            content.Append("</td>");
            content.Append("<td style='width: 32.4576 %'  align=center>");
            content.Append("<span style='font-size:10pt; font-weight:bold'>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</span");
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td style='vertical-align: top; width: 65.5424 %;'>");
            content.Append(deparment == null ? "" : "<b>" + deparment.Name.ToUpper() + "</b>");
            content.Append("</td>");
            content.Append("<td align=center style='vertical-align: top; width:32.4576 %;'> ");
            content.Append("<span style='font-size:10pt; font-weight:bold'>Độc lập - Tự do - Hạnh phúc<br/>___***___ </span>");
            content.Append("</td>").Append("</tr>");
            content.Append("<tr>").Append("<td colspan='2' style='padding-top:20px; font-size:20pt;' align=center>");
            content.Append("<span style='font-weight:bold;font-size:16pt;'>THỐNG KÊ CÔNG VIỆC</span><br/>");
            if (query.FromDate != null && query.ToDate != null)
            {
                content.Append("<i style='font-size:10pt'>").Append("(" + string.Format("{0:dd/MM/yyyy}", query.FromDate) + " - " + string.Format("{0:dd/MM/yyyy}", query.ToDate)).Append(")</i>");
            }
            content.Append("</td>").Append("</tr>");
            content.Append("</table>");
            content.Append("<br/>");
            content.Append("<table style='width:100%; border-collapse: collapse;' border='1' cellspacing='0' cellpadding='0'>");
            content.Append("<tr>").Append("<td align=center style='padding:5px 0px;width:37px;'>");
            content.Append("<b>STT</b>");
            content.Append("</td>");
            content.Append("<td align=center style=' padding:5px 0px'>");
            content.Append("<span style='font-weight:bold; font-size:10pt'>HẠNG MỤC</span>");
            content.Append("</td>");
            content.Append("<td align=center style='padding:5px 0px;'>");
            content.Append("<span style='font-weight:bold; font-size:10pt'>NGÀY TẠO</span>");
            content.Append("</td>");
            content.Append("<td align=center style='padding:5px 0px;'>");
            content.Append("<span style='font-weight:bold; font-size:10pt'>NGÀY BẮT ĐẦU</span>");
            content.Append("</td>");
            content.Append("<td align=center style='padding:5px 0px;'>");
            content.Append("<span style='font-weight:bold; font-size:10pt'>NGÀY HẾT HẠN</span>");
            content.Append("</td>");
            content.Append("<td align=center style='padding:5px 0px; width:30%'>");
            content.Append("<span style='font-weight:bold; font-size:10pt'>NGƯỜI XỬ LÝ</span>");
            content.Append("</td>").Append("</tr>");
            if (lstStatus.Data.Count() > 0)
            {
                foreach (var obj in lstStatus.Data)
                {
                    content.Append("<tr>");
                    content.Append("<td colspan=6 style='padding:5px'>");
                    content.Append("<b>" + obj.Title + "</b>");
                    content.Append("</td>");
                    content.Append("</tr>");
                    int count = 1;
                    foreach (var item in source)
                    {
                        if (item.StatusId == obj.Id)
                        {
                            content.Append("<tr>").Append("<td align=center>");
                            content.Append("<span style='font-size:10pt'>" + count + "</span>");
                            content.Append("</td>");
                            content.Append("<td style='padding:5px '>");
                            content.Append("<span style='font-size:10pt'>" + item.Code + " - " + item.Title + "</span>");
                            content.Append("</td>");
                            content.Append("<td style='padding:5px;'  align=center>");
                            content.Append("<span style='font-size:10pt'>" + string.Format("{0:dd/MM/yyyy}", item.CreatedOn) + "</span>");
                            content.Append("</td>");
                            content.Append("<td style='padding:5px ;'  align=center>");
                            content.Append("<span style='font-size:10pt'>" + string.Format("{0:dd/MM/yyyy}", item.StartDate));
                            content.Append("</td>");
                            content.Append("<td style='padding:5px;'  align=center>");
                            content.Append("<span style='font-size:10pt'>" + string.Format("{0:dd/MM/yyyy}", item.DueDate) + "</span>");
                            content.Append("</td>");
                            content.Append("<td style='padding:5px'>");
                            content.Append("<span style='font-size:10pt'>" + item.Processor + "</span>");
                            content.Append("</td>").Append("</tr>");
                            count += 1;
                        }
                    }
                }
            }
            content.Append("</table><br/>");
            content.Append("<table style='width:100%'>");
            content.Append("<tr>");
            content.Append("<td style='width:80%'>");
            content.Append("</td>");
            content.Append("<td  align=center style=' padding-bottom:5px; width:200px'>");
            content.Append("Người lập biểu");
            content.Append("</td>");
            content.Append("</tr>");
            content.Append("<tr>");
            content.Append("<td  style='width:80%'>");
            content.Append("</td>");
            content.Append("<td style='height:70px'>");
            content.Append("</td>");
            content.Append("</tr>");
            content.Append("<tr>");
            content.Append("<td  style='width:80%'>");
            content.Append("</td>");
            content.Append("<td align=center>");
            content.Append("<b>" + staff.FullName + "</b>");
            content.Append("</td>");
            content.Append("</tr>");
            content.Append("</table>");
            content.Append("</body>");
            content.Append("</html>");
            response.Value = Util.CreateContent(content.ToString(), "task", "Demo1");
            return response;
        }
        public BaseListResponse<SPGetTaskMobile_Result> FilterTaskMobile(TaskQuery query)
        {
            var response = new BaseListResponse<SPGetTaskMobile_Result>();
            int count = 0;
            try
            {
                response.Data = _taskRepository.FilterMobile(query, out count);
                response.TotalItems = count;
                response.PageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
                response.PageSize = query.PageSize;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        #endregion Task

        #region TaskAssignee
        public BaseListResponse<TaskAssignee> AddTaskAssignee(List<TaskAssignee> models)
        {
            var response = new BaseListResponse<TaskAssignee>();
            List<TaskAssignee> resultSet = new List<TaskAssignee>();
            foreach (var model in models)
            {
                var errors = Validate<TaskAssignee>(model, new TaskAssigneeValidator());
                if (errors.Count() > 0)
                {
                    BaseListResponse<TaskAssignee> errResponse = new BaseListResponse<TaskAssignee>();
                    errResponse.IsSuccess = false;
                    return errResponse;
                }
            }
            foreach (var model in models)
            {
                model.CreatedOn = DateTime.Now;
                resultSet.Add(_taskAssigneeRepository.Add(model));
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskAssignee", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            response.Data = resultSet;
            return response;
        }
        public BaseListResponse<SPGetTaskAssigneeByTaskId_Result> GetTaskAssigneeByTaskId(int taskId)
        {
            var response = new BaseListResponse<SPGetTaskAssigneeByTaskId_Result>();
            try
            {
                response.Data = _taskAssigneeRepository.GetTaskAssigneeByTaskId(taskId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetTaskAssigneeByTaskId_Result> GetTaskAssigneeViewDetail(int taskId)
        {
            var response = new BaseListResponse<SPGetTaskAssigneeByTaskId_Result>();
            try
            {
                response.Data = _taskAssigneeRepository.GetTaskAssigneeViewDetailByTaskId(taskId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<TaskAssignee> UpdateTaskAssignee(TaskAssigneeQuery model)
        {
            BaseResponse<TaskAssignee> result = new BaseResponse<TaskAssignee>();

            //model: taskId, userId người xử lý chính mới
            List<SPGetTaskAssigneeByTaskId_Result> listTaskAssignee = new List<SPGetTaskAssigneeByTaskId_Result>();
            listTaskAssignee = _taskAssigneeRepository.GetTaskAssigneeByTaskId(model.TaskId);

            // Kiểm tra người xử chính lý mới có phải đã là đồng xử lý hay không?
            TaskAssignee nguoixulychinhcu = null;
            TaskAssignee nguoixulychinhcuUpdateThanhDongXuLy = new TaskAssignee();
            TaskAssignee nguoixulychinhmoi = new TaskAssignee();
            TaskAssignee nguoixulychinhmoiDangLaDongXuLy = null;


            foreach (var item in listTaskAssignee)
            {
                if (item.Assignee)
                {
                    //Lấy ra người xử lý chính cũ
                    nguoixulychinhcu = _taskAssigneeRepository.GetById(item.Id);
                }
                if (item.Coprocessor && item.UserId == model.UserId)
                {
                    nguoixulychinhmoiDangLaDongXuLy = _taskAssigneeRepository.GetById(item.Id);
                }
            }
            if (nguoixulychinhcu != null)
            {
                nguoixulychinhmoi.TaskId = model.TaskId;
                nguoixulychinhmoi.UserId = model.UserId;
                nguoixulychinhmoi.Order = 0;
                nguoixulychinhmoi.Supervisor = false;
                nguoixulychinhmoi.Assignee = true;
                nguoixulychinhmoi.Coprocessor = false;
                nguoixulychinhmoi.CreatedBy = nguoixulychinhcu.CreatedBy;
                nguoixulychinhmoi.CreatedOn = DateTime.Now;
                if (nguoixulychinhmoiDangLaDongXuLy != null)
                {
                    nguoixulychinhmoi.ViewDetail = nguoixulychinhmoiDangLaDongXuLy.ViewDetail;
                    nguoixulychinhmoi.ViewOn = nguoixulychinhmoiDangLaDongXuLy.ViewOn;
                }
                else
                {
                    nguoixulychinhmoi.ViewDetail = false;
                    nguoixulychinhmoi.ViewOn = null;
                }
                _taskAssigneeRepository.Add(nguoixulychinhmoi);
                if (nguoixulychinhmoiDangLaDongXuLy != null)
                    _taskAssigneeRepository.Delete(nguoixulychinhmoiDangLaDongXuLy);


                nguoixulychinhcuUpdateThanhDongXuLy.TaskId = model.TaskId;
                nguoixulychinhcuUpdateThanhDongXuLy.UserId = nguoixulychinhcu.UserId;
                nguoixulychinhcuUpdateThanhDongXuLy.Order = 0;
                nguoixulychinhcuUpdateThanhDongXuLy.Supervisor = false;
                nguoixulychinhcuUpdateThanhDongXuLy.Assignee = false;
                nguoixulychinhcuUpdateThanhDongXuLy.Coprocessor = true;
                nguoixulychinhcuUpdateThanhDongXuLy.CreatedBy = nguoixulychinhcu.CreatedBy;
                nguoixulychinhcuUpdateThanhDongXuLy.CreatedOn = DateTime.Now;
                nguoixulychinhcuUpdateThanhDongXuLy.ViewDetail = nguoixulychinhcu.ViewDetail;
                nguoixulychinhcuUpdateThanhDongXuLy.ViewOn = nguoixulychinhcu.ViewOn;

                foreach (var item in listTaskAssignee)
                {
                    if (item.UserId == nguoixulychinhcu.UserId)
                    {
                        if (!item.Supervisor)
                        {
                            //Xóa các bản ghi liên quan đến người xử lý chính cũ
                            TaskAssignee nguoixulychinhcuDeleteModel = new TaskAssignee();
                            nguoixulychinhcuDeleteModel = _taskAssigneeRepository.GetById(item.Id);
                            if (nguoixulychinhcuDeleteModel != null)
                            {
                                _taskAssigneeRepository.Delete(nguoixulychinhcuDeleteModel);
                            }
                        }
                    }
                }
                //Add nguoi xử lý chính cũ thành đồng xử lý
                _taskAssigneeRepository.Add(nguoixulychinhcuUpdateThanhDongXuLy);
                result.Value = nguoixulychinhmoi;
            }
            return result;
        }
        public BaseResponse<TaskAssignee> ViewTaskDetail(string userId, int taskId)
        {
            BaseResponse<TaskAssignee> result = new BaseResponse<TaskAssignee>();
            List<SPGetTaskAssigneeByTaskId_Result> listTaskAssignee = new List<SPGetTaskAssigneeByTaskId_Result>();
            listTaskAssignee = _taskAssigneeRepository.GetTaskAssigneeByTaskId(taskId);
            List<SPGetTaskAssigneeByTaskId_Result> taskAssignees = listTaskAssignee.Where(x => x.UserId == userId).ToList();
            bool createdActivity = false;
            if (taskAssignees.Count > 0)
            {
                foreach (var taskAssignee in taskAssignees)
                {
                    TaskAssignee model = new TaskAssignee();
                    model = _taskAssigneeRepository.GetById(taskAssignee.Id);
                    if (!model.ViewDetail)
                    {
                        model.ViewDetail = true;
                        model.ViewOn = DateTime.Now;
                        _taskAssigneeRepository.Edit(model);
                        result.Value = model;
                    }
                    else
                    {
                        createdActivity = true;
                    }
                }
                try
                {
                    if (!createdActivity)
                    {
                        TaskActivity taskActivity = new TaskActivity();
                        taskActivity.RecordId = taskId;
                        taskActivity.Action = "VIEW";
                        taskActivity.Type = 1;
                        taskActivity.Display = false;
                        taskActivity.FlowDescription = "xem chi tiết công việc";
                        taskActivity.Description = "xem chi tiết công việc";
                        taskActivity.TaskField = "";
                        taskActivity.OldValue = "";
                        taskActivity.NewValue = "";
                        taskActivity.CreatedOn = DateTime.Now;
                        taskActivity.CreatedBy = userId;
                        taskActivity.EditedOn = DateTime.Now;
                        taskActivity.EditedBy = userId;
                        _taskActivityRepository.Add(taskActivity);
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = ex.Message + ex.StackTrace;
                }
            }
            return result;
        }
        public BaseListResponse<SPGetTaskDocumentHistory_Result> GetTaskAssigneeDocumentHistory(int taskId, int documentId, string documentReceived)
        {
            var response = new BaseListResponse<SPGetTaskDocumentHistory_Result>();
            try
            {
                response.Data = _taskAssigneeRepository.GetTaskAssigneeDocumentHistory(taskId, documentId, documentReceived);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;

        }
        public System.Threading.Tasks.Task<BaseListResponse<TaskAssignee>> AddMoreTaskAssignee(List<TaskAssignee> models)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {


                BaseListResponse<TaskAssignee> result = new BaseListResponse<TaskAssignee>();

                int taskId = 0;
                try
                {
                    taskId = models[0].TaskId;
                }
                catch
                { }
                if (taskId != 0)
                {
                    List<SPGetTaskAssigneeByTaskId_Result> listTaskAssignee = new List<SPGetTaskAssigneeByTaskId_Result>();
                    listTaskAssignee = _taskAssigneeRepository.GetTaskAssigneeByTaskId(taskId);
                    int countSuccess = 0;

                    foreach (var model in models)
                    {
                        bool exist = false;
                        foreach (var ta in listTaskAssignee)
                        {
                            if (!string.IsNullOrEmpty(model.UserId))
                            {
                                if (ta.UserId == model.UserId)
                                {
                                    exist = true;
                                }
                            }
                        }
                        if (!exist)
                        {
                            if (!string.IsNullOrEmpty(model.UserId))
                            {
                                model.Order = 0;
                                model.CreatedOn = DateTime.Now;
                                _taskAssigneeRepository.Add(model);
                                countSuccess++;
                            }
                        }
                    }
                    if (countSuccess == 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "Chọn người phối hợp";
                    }
                    else
                    {
                        if (taskId != 0)
                        {
                            Task task = _taskRepository.GetById(taskId);
                            if (task != null)
                            {
                                string createdBy = task.CreatedBy;
                                string mailto = _userRepository.GetUserByUserId(createdBy).Email;
                                List<SPGetTaskAssigneeByTaskId_Result> listXuLy = new List<SPGetTaskAssigneeByTaskId_Result>();
                                listXuLy = _taskAssigneeRepository.GetTaskAssigneeByTaskId(task.Id);


                                #region Send email
                                var userProfile = _departmentRepository.GetUserMainOrganization(models[0].CreatedBy);
                                try
                                {
                                    //mainAssignee: Người xử lý chính
                                    //listUserRelate: Người tạo, đồng xử lý, giám sát

                                    string cc = "";
                                    if (listXuLy.Count > 0)
                                    {
                                        foreach (var item in listXuLy)
                                        {
                                            cc += _userRepository.GetUserByUserId(item.UserId).Email + ",";
                                        }
                                    }
                                    string content = "";
                                    string subject = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = task.DepartmentId, Title = "TASKNOTICEEMAILSUBJECT", DefaultValue = "[VOffice] - Thông báo" });

                                    string domain = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = task.DepartmentId, Title = "APP_DOMAIN", DefaultValue = "http://congvan.veph.vn/" });

                                    string taskDetailUrl = domain + "#!/chi-tiet-cong-viec/" + task.Id.ToString() + "/0////1/true/true";
                                    subject += " - Quản lý công việc: " + task.Code;
                                    string temp = @"<div style='border: 3px solid #15c; padding: 10px; overflow: hidden;'>
                                        <table style='width:100%'>
                                        <tr>
                                            <td><a style='text-decoration:none;' href='" + taskDetailUrl + @"'>[" + task.Code + "] - " + task.Title + @"</a></td>
                                        </tr>
                                        <tr>
                                            <td>
                                            <div style='border-bottom:1px dotted #ddd; margin:10px 0; width:100%'></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>{0}</td>
                                        </tr>
                                        <tr>
                                            <td>{1}</td>
                                        </tr>
                                            </table></div> ";

                                    string actionNotice = "";
                                    if (userProfile != null)
                                    {
                                        actionNotice = "<b>" + userProfile.FullName + "</b>" + " đã bổ sung người xử lý công việc: " + task.Code + " - " + task.Title;
                                        actionNotice += "<br/><span style = 'font-style:italic; font-size:10px; color:#888888' >" + task.CreatedOn.ToString("dd/MM/yyyy | HH:mm") + "</ span>";

                                    }
                                    string contentNotice = task.Description;
                                    content = string.Format(temp, actionNotice, contentNotice);
                                    UtilityProvider.SendEmail(task.DepartmentId, mailto, cc, "", subject, content);
                                }
                                catch
                                { }
                                #endregion Send email
                                #region Send notification
                                var listUserSendNotifi = new List<SPGetUserNotification_Result>();
                                var listUserDistinct = listXuLy.Distinct().ToList();
                                foreach (var u in listUserDistinct)
                                {
                                    if (u.UserId != models[0].CreatedBy)
                                    {
                                        listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByUserId(u.UserId));
                            }
                        }
                                NotificationCenter resultNotifiCenter = null;
                                NotificationCenter notifiFirst = null;
                                foreach (var item in listUserSendNotifi)
                                {
                                    FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                                    fcmNotificationCenter.Avatar = userProfile.Avatar;
                                    fcmNotificationCenter.FullName = userProfile.FullName;
                                    fcmNotificationCenter.Title = "vOffice";
                                    fcmNotificationCenter.RecordNumber = task.Code;
                                    fcmNotificationCenter.RecordId = task.Id;
                                    fcmNotificationCenter.CreatedBy = task.CreatedBy;
                                    fcmNotificationCenter.CreatedOn = DateTime.Now;
                                    fcmNotificationCenter.HaveSeen = false;
                                    fcmNotificationCenter.ReceivedUserId = item.UserId;
                                    fcmNotificationCenter.DeviceId = item.DeviceId;
                                    fcmNotificationCenter.Content = "đã bổ sung người xử lý công việc: \n" + task.Title;
                                    fcmNotificationCenter.Type = (int)NotificationCode.CreateTask;
                                    if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                                    {
                                        fcmNotificationCenter.GroupId = notifiFirst.Id;
                    }
                                    var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                                    resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                                    fcmNotificationCenter.Id = resultNotifiCenter.Id;
                                    if (notifiFirst == null) notifiFirst = resultNotifiCenter;

                                    FCMPushNotification fcmPushNotification = new FCMPushNotification();
                                    fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
                }
                                #endregion
                            }
                        }
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Chọn người phối hợp";
                }
                return result;
            });
        }
        public BaseResponse<bool> CheckPermissionUserTask(int taskId, string userId)
        {
            BaseResponse<bool> result = new BaseResponse<bool>();
            bool IsApprove = false;
            IsApprove = _taskRepository.CheckPermissionUserTask(taskId, userId);
            result.Value = IsApprove;
            return result;
        }
        #endregion TaskAssignee

        #region TaskDocuments
        public BaseResponse<TaskDocument> AddTaskDocument(TaskDocument model)
        {
            var response = new BaseResponse<TaskDocument>();
            var errors = Validate<TaskDocument>(model, new TaskDocumentsValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<TaskDocument> errResponse = new BaseResponse<TaskDocument>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.CreatedOn = DateTime.Now;
                response.Value = _taskDocumentRepository.Add(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskDocument", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<SPGetTaskDocumentByTaskId_Result> GetTaskDocumentByTaskId(int taskId)
        {
            var response = new BaseListResponse<SPGetTaskDocumentByTaskId_Result>();
            try
            {
                response.Data = _taskDocumentRepository.GetTaskDocumentByTaskId(taskId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        #endregion TaskDocuments

        #region Task Project
        public BaseResponse<Project> GetProjectById(int id)
        {
            var response = new BaseResponse<Project>();
            try
            {
                response.Value = _projectRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetProjectByDepartmentId_Result> FilterProjectByDepartmentId(int departmentId, string keyword)
        {
            var response = new BaseListResponse<SPGetProjectByDepartmentId_Result>();
            try
            {
                var result = _projectRepository.FilterByDepartmentId(departmentId, keyword);
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
        public BaseListResponse<SPGetProject_Result> Filter(ProjectQuery query)
        {
            var response = new BaseListResponse<SPGetProject_Result>();
            int count = 0;
            try
            {
                response.Data = _projectRepository.Filter(query, out count);
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
        public BaseResponse<Project> AddProject(Project model)
        {
            var response = new BaseResponse<Project>();
            var errors = Validate<Project>(model, new ProjectValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Project> errResponse = new BaseResponse<Project>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }

            try
            {
                var listProject = _projectRepository.GetAll().Where(x => x.Code.ToLower() == model.Code.ToLower() && x.DepartmentId == model.DepartmentId).ToList();
                if (listProject.Count > 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã dự án đã tồn tại";
                }
                else
                {
                    model.CreatedOn = DateTime.Now;
                    response.Value = _projectRepository.Add(model);
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "Project", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);

                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<Project> UpdateProject(Project model)
        {
            BaseResponse<Project> response = new BaseResponse<Project>();
            var errors = Validate<Project>(model, new ProjectValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<Project> errResponse = new BaseResponse<Project>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _projectRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "Project", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalProject(int id)
        {
            BaseResponse response = new BaseResponse();
            Project model = _projectRepository.GetById(id);
            BaseListResponse<Task> lstTasks = new BaseListResponse<Task>();
            lstTasks.Data = _taskRepository.GetAll().Where(n => n.Active == true && n.Deleted == false && n.ProjectId == model.Id).ToList();
            try
            {
                if (lstTasks.Data.Count() == 0)
                {
                    model.Deleted = true;
                    _projectRepository.Edit(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "DELETE", "Project", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    response.Message = "Không thể xóa được dự án vì có công việc liên quan";
                    response.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        #endregion Task project

        #region TaskType
        public BaseResponse<TaskType> GetTaskTypeById(int id)
        {
            var response = new BaseResponse<TaskType>();
            try
            {
                response.Value = _taskTypeRepository.GetById(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                response.IsSuccess = false;
            }
            return response;
        }
        public BaseListResponse<SPGetTaskTypeByDepartmentId_Result> FilterTaskTypeByDepartmentId(int departmentId, string keyword)
        {
            var response = new BaseListResponse<SPGetTaskTypeByDepartmentId_Result>();
            try
            {
                var result = _taskTypeRepository.FilterByDepartmentId(departmentId, keyword);
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
        public BaseListResponse<SPGetTaskType_Result> FilterTaskType(TaskTypeQuery query)
        {
            var response = new BaseListResponse<SPGetTaskType_Result>();
            int count = 0;
            try
            {
                response.Data = _taskTypeRepository.Filter(query, out count);
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
        public BaseResponse<TaskType> AddTaskType(TaskType model)
        {
            var response = new BaseResponse<TaskType>();
            var errors = Validate<TaskType>(model, new TaskTypeValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<TaskType> errResponse = new BaseResponse<TaskType>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                var listTaskType = _taskTypeRepository.GetAll().Where(x => x.Code.ToLower() == model.Code.ToLower() && x.DepartmentId == model.DepartmentId).ToList();
                if (listTaskType.Count > 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Mã mảng công việc đã tồn tại";
                }
                else
                {
                    model.CreatedOn = DateTime.Now;
                    response.Value = _taskTypeRepository.Add(model);
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskType", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<TaskType> UpdateTaskType(TaskType model)
        {
            BaseResponse<TaskType> response = new BaseResponse<TaskType>();
            var errors = Validate<TaskType>(model, new TaskTypeValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<TaskType> errResponse = new BaseResponse<TaskType>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _taskTypeRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "TaskType", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse DeleteLogicalTaskType(int id)
        {
            BaseResponse response = new BaseResponse();
            TaskType model = _taskTypeRepository.GetById(id);
            BaseListResponse<Task> lstTasks = new BaseListResponse<Task>();
            lstTasks.Data = _taskRepository.GetAll().Where(n => n.Active == true && n.Deleted == false && n.TaskTypeId == model.Id).ToList();
            try
            {
                if (lstTasks.Data.Count() == 0)
                {
                    model.Deleted = true;
                    _taskTypeRepository.Edit(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "DELETE", "TaskType", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    response.Message = "Không thể xóa được mảng công việc vì có công việc liên quan";
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        #endregion TaskType

        #region TaskAttachment
        public BaseListResponse<TaskAttachment> AddTaskAttachment(List<TaskAttachment> models)
        {
            bool remove = false;
            int recordId = 0;
            int taskAttachmentType = 1; // opinionAttachmentType = 2: Ý kiến xử lý
            foreach (var item in models)
            {
                if (item.Type == taskAttachmentType)
                {
                    remove = true;
                    recordId = item.RecordId;
                    break;
                }
            }
            if (remove)
            {
                //Xóa tất cả Task Attachment của Opinion có id = recordId
                List<SPGetTaskAttachmentByRecordId_Result> listUploadedAttachment = _taskAttachmentRepository.GetTaskAttachmenByRecordId(recordId, taskAttachmentType.ToString());
                foreach (var item in listUploadedAttachment)
                {
                    TaskAttachment uploadedTaskAttachment = _taskAttachmentRepository.GetById(item.Id);
                    if (uploadedTaskAttachment != null)
                    {
                        _taskAttachmentRepository.Delete(uploadedTaskAttachment);
                    }
                }
            }
            var response = new BaseListResponse<TaskAttachment>();
            List<TaskAttachment> resultSet = new List<TaskAttachment>();
            foreach (var model in models)
            {
                var errors = Validate<TaskAttachment>(model, new TaskAttachmentValidator());
                if (errors.Count() > 0)
                {
                    BaseListResponse<TaskAttachment> errResponse = new BaseListResponse<TaskAttachment>();
                    errResponse.IsSuccess = false;
                    return errResponse;
                }
            }
            foreach (var model in models)
            {
                model.CreatedOn = DateTime.Now;
                resultSet.Add(_taskAttachmentRepository.Add(model));
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskAttachment", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            response.Data = resultSet;
            return response;
        }
        public BaseListResponse<TaskAttachment> UpdateTaskAttachment(List<TaskAttachment> models)
        {
            BaseListResponse<TaskAttachment> errResponse = new BaseListResponse<TaskAttachment>();
            var response = new BaseListResponse<TaskAttachment>();
            var taskId = 0;
            if (models.Count > 0)
            {
                taskId = models[0].RecordId;
            }
            else
            {
                errResponse.IsSuccess = false;
                return errResponse;
            }
            List<SPGetTaskAttachmentByRecordId_Result> listTaskAttachmentUploaded = new List<SPGetTaskAttachmentByRecordId_Result>();
            listTaskAttachmentUploaded = _taskAttachmentRepository.GetTaskAttachmenByRecordId(taskId, "1");
            if (listTaskAttachmentUploaded.Count > 0)
            {
                foreach (var taskAttachmentUploadedModel in listTaskAttachmentUploaded)
                {
                    TaskAttachment taskAttachment = new TaskAttachment();
                    taskAttachment = _taskAttachmentRepository.GetById(taskAttachmentUploadedModel.Id);
                    if (taskAttachment != null)
                    {
                        _taskAttachmentRepository.Delete(taskAttachment);
                    }
                }
            }
            List<TaskAttachment> resultSet = new List<TaskAttachment>();
            foreach (var model in models)
            {
                var errors = Validate<TaskAttachment>(model, new TaskAttachmentValidator());
                if (errors.Count() > 0)
                {
                    errResponse.IsSuccess = false;
                    return errResponse;
                }
            }
            foreach (var model in models)
            {
                model.CreatedOn = DateTime.Now;
                resultSet.Add(_taskAttachmentRepository.Add(model));
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskAttachment", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            response.Data = resultSet;
            return response;
        }
        public BaseListResponse<SPGetTaskAttachmentByRecordId_Result> GetTaskAttachmentByRecordId(string type, int recordId)
        {
            var response = new BaseListResponse<SPGetTaskAttachmentByRecordId_Result>();
            try
            {
                response.Data = _taskAttachmentRepository.GetTaskAttachmenByRecordId(recordId, type);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        #endregion TaskAttachment

        #region TaskOpinion
        public System.Threading.Tasks.Task<BaseResponse<TaskOpinion>> AddTaskOpinion(TaskOpinion model)
        {
            return System.Threading.Tasks.Task.Run(() =>
             {
                 var response = new BaseResponse<TaskOpinion>();
                 var errors = Validate<TaskOpinion>(model, new TaskOpinionValidator());
                 if (errors.Count() > 0)
                 {
                     BaseResponse<TaskOpinion> errResponse = new BaseResponse<TaskOpinion>(model, errors);
                     errResponse.IsSuccess = false;
                     return errResponse;
                 }
                 try
                 {
                     model.CreatedOn = DateTime.Now;
                     response.Value = _taskOpinionRepository.Add(model);
                     try
                     {
                         _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskOpinion", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                     }
                     catch { }
                     string mailTo = "";// _userRepository.GetUserByUserId(_taskRepository.GetById(model.TaskId).CreatedBy).Email;
                     SPGetTaskDetailById_Result taskDetail = new SPGetTaskDetailById_Result();
                     List<string> listUserRelate = new List<string>();
                     try
                     {
                         if (model.TaskId.HasValue)
                         {
                             taskDetail = _taskRepository.SPGetTaskDetailById(model.TaskId.Value);
                             if (taskDetail != null)
                             {
                                 //listUserRelate.Add(taskDetail.CreatedBy);
                                 listUserRelate.Add(taskDetail.EditedBy);
                                 mailTo = _userRepository.GetUserByUserId(taskDetail.CreatedBy).Email;
                             }
                             List<SPGetTaskAssigneeByTaskId_Result> listTaskAssignee = new List<SPGetTaskAssigneeByTaskId_Result>();
                             listTaskAssignee = _taskAssigneeRepository.GetTaskAssigneeByTaskId(taskDetail.Id);
                             if (listTaskAssignee.Count > 0)
                             {
                                 foreach (var assignee in listTaskAssignee)
                                 {
                                     if (!string.IsNullOrEmpty(assignee.UserId))
                                     {
                                         listUserRelate.Add(assignee.UserId);
                                     }
                                 }
                             }
                         }
                         #region Send email
                         var userProfile = _departmentRepository.GetUserMainOrganization(model.CreatedBy);
                         try
                         {
                             //mainAssignee: Người xử lý chính
                             //listUserRelate: Người tạo, đồng xử lý, giám sát

                             string cc = "";
                             foreach (string u in listUserRelate)
                             {
                                 if (u != model.CreatedBy)
                                 {
                                     cc += _userRepository.GetUserByUserId(u).Email + ",";
                                 }
                             }
                             string content = "";
                             string subject = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = taskDetail.DepartmentId, Title = "TASKNOTICEEMAILSUBJECT", DefaultValue = "[VOffice] - Thông báo" });

                             string domain = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = taskDetail.DepartmentId, Title = "APP_DOMAIN", DefaultValue = "http://congvan.veph.vn/" });

                             string taskDetailUrl = domain + "#!/chi-tiet-cong-viec/" + model.TaskId.ToString() + "/0////1/true/true";
                             subject += " - Quản lý công việc: " + taskDetail.Code;
                             string temp = @"<div style='border: 3px solid #15c; padding: 10px; overflow: hidden;'>
                             <table style='width:100%'>
                                        <tr>
                                            <td><a style='text-decoration:none;' href='" + taskDetailUrl + @"'>[" + taskDetail.Code + "] - " + taskDetail.Title + @"</a></td>
                                        </tr>
                                        <tr>
                                         <td><div style='border-bottom: 1px dotted #ddd; width:100%; margin:10px 0px'></div></td>
                                        </tr>
                                        <tr>
                                            <td>{0}</td>
                                        </tr>
                                        <tr>
                                            <td><div style='border: 1px solid #e7eaec;box-shadow: none;margin-top: 10px;margin-bottom: 5px;padding: 10px;line-height:20px;border-radius: 4px;min-height:20px;background-color: #f5f5f5;'>{1}</div></td>
                                        </tr>
                                            </table> </div>";

                             string actionNotice = "";
                             if (userProfile != null)
                             {
                                 actionNotice = "<b>" + userProfile.FullName + "</b>" + " đã gửi ý kiến xử lý công việc: " + taskDetail.Code + " - " + taskDetail.Title;
                                 actionNotice += "<br/><span style = 'font-style:italic; font-size:10px; color:#888888' >" + taskDetail.CreatedOn.ToString("dd/MM/yyyy | HH:mm") + "</ span>";
                             }
                             string contentNotice = model.Content;
                             content = string.Format(temp, actionNotice, contentNotice);
                             // not send to created 
                             UtilityProvider.SendEmail(taskDetail.DepartmentId, mailTo, cc, "", subject, content);
                         }
                         catch
                         { }
                         #endregion Send email
                         #region Send notification
                         var listUserSendNotifi = new List<SPGetUserNotification_Result>();
                         var listUserRelateDistinct = listUserRelate.Distinct().ToList();
                         foreach (var u in listUserRelateDistinct)
                         {
                             if (u != model.CreatedBy)
                             {
                                 listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByUserId(u));
                             }
                         }
                         NotificationCenter resultNotifiCenter = null;
                         NotificationCenter notifiFirst = null;
                         foreach (var item in listUserSendNotifi)
                         {
                             FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                             fcmNotificationCenter.Avatar = userProfile.Avatar;
                             fcmNotificationCenter.FullName = userProfile.FullName;
                             fcmNotificationCenter.Title = "vOffice";
                             fcmNotificationCenter.RecordNumber = taskDetail.Code;
                             fcmNotificationCenter.RecordId = taskDetail.Id;
                             fcmNotificationCenter.CreatedBy = model.CreatedBy;
                             fcmNotificationCenter.CreatedOn = DateTime.Now;
                             fcmNotificationCenter.HaveSeen = false;
                             fcmNotificationCenter.ReceivedUserId = item.UserId;
                             fcmNotificationCenter.DeviceId = item.DeviceId;
                             if (model.ParentId != 0)
                             {
                                 fcmNotificationCenter.Content = "đã trả lời ý kiến: \n" + model.Content;
                                 fcmNotificationCenter.RelateRecordId = model.ParentId;
                                 fcmNotificationCenter.SubRelateRecordId = response.Value.Id;
                                 fcmNotificationCenter.Type = (int)NotificationCode.ReplyOpinion;
                             }
                             else
                             {
                                 fcmNotificationCenter.Type = (int)NotificationCode.CreateOpinion;
                                 fcmNotificationCenter.Content = "đã gửi một ý kiến mới: \n" + model.Content;
                                 fcmNotificationCenter.RelateRecordId = response.Value.Id;
                             }
                             if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                             {
                                 fcmNotificationCenter.GroupId = notifiFirst.Id;
                             }
                             var notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                             resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                             if (notifiFirst == null) notifiFirst = resultNotifiCenter;
                             fcmNotificationCenter.Id = resultNotifiCenter.Id;

                             FCMPushNotification fcmPushNotification = new FCMPushNotification();
                             fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
                         }
                         #endregion
                     }
                     catch { }
                 }
                 catch (Exception ex)
                 {
                     response.IsSuccess = false;
                     response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                 }
                 return response;
             });
        }
        public BaseListResponse<SPGetTaskOpinionByTaskId_Result> GetTaskOpinionByTaskId(int taskId)
        {
            var response = new BaseListResponse<SPGetTaskOpinionByTaskId_Result>();
            try
            {
                response.Data = _taskOpinionRepository.GetTaskOpnionByTaskId(taskId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<TaskOpinion> UpdateTaskOpinion(TaskOpinion model)
        {
            BaseResponse<TaskOpinion> response = new BaseResponse<TaskOpinion>();
            var errors = Validate<TaskOpinion>(model, new TaskOpinionValidator());
            if (errors.Count() > 0)
            {
                BaseResponse<TaskOpinion> errResponse = new BaseResponse<TaskOpinion>(model, errors);
                errResponse.IsSuccess = false;
                return errResponse;
            }
            try
            {
                model.EditedOn = DateTime.Now;
                response.Value = _taskOpinionRepository.Edit(model);
                try
                {
                    _applicationLoggingRepository.Log("EVENT", "UPDATE", "TaskOpinion", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);
                }
                catch { }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseResponse<TaskOpinionComplex> GetTaskOpinionByOpinionId(int id)
        {
            BaseResponse<TaskOpinionComplex> response = new BaseResponse<TaskOpinionComplex>();
            response.Value = _taskOpinionRepository.GetTaskOpinionByOpinionId(id);
            return response;
        }
        #endregion TaskOpinion
        #region TaskActivity
        public System.Threading.Tasks.Task<BaseResponse<TaskActivity>> AddTaskActivity(TaskActivity model)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                var response = new BaseResponse<TaskActivity>();
                var errors = Validate<TaskActivity>(model, new TaskActivityValidator());
                if (errors.Count() > 0)
                {
                    BaseResponse<TaskActivity> errResponse = new BaseResponse<TaskActivity>(model, errors);
                    errResponse.IsSuccess = false;
                    return errResponse;
                }
                try
                {
                    model.CreatedOn = DateTime.Now;
                    response.Value = _taskActivityRepository.Add(model);
                    try
                    {
                        _applicationLoggingRepository.Log("EVENT", "CREATE", "TaskActivity", model.Id.ToString(), "", "", model, "", HttpContext.Current.Request.UserHostAddress, model.CreatedBy);


                    }
                    catch { }

                    string mailTo = _userRepository.GetUserByUserId(model.CreatedBy).Email;
                    SPGetTaskDetailById_Result taskDetail = new SPGetTaskDetailById_Result();
                    List<string> listUserRelate = new List<string>();
                    try
                    {
                        if (model.RecordId > 0)
                        {
                            taskDetail = _taskRepository.SPGetTaskDetailById(model.RecordId);
                            if (taskDetail != null)
                            {
                                listUserRelate.Add(taskDetail.CreatedBy);
                                listUserRelate.Add(taskDetail.EditedBy);
                            }
                            List<SPGetTaskAssigneeByTaskId_Result> listTaskAssignee = new List<SPGetTaskAssigneeByTaskId_Result>();
                            listTaskAssignee = _taskAssigneeRepository.GetTaskAssigneeByTaskId(taskDetail.Id);
                            if (listTaskAssignee.Count > 0)
                            {
                                foreach (var assignee in listTaskAssignee)
                                {
                                    if (!string.IsNullOrEmpty(assignee.UserId))
                                    {
                                        listUserRelate.Add(assignee.UserId);
                                    }
                                }
                            }
                        }
                        #region Send email
                        var userProfile = _departmentRepository.GetUserMainOrganization(model.CreatedBy);
                        try
                        {
                            //mainAssignee: Người xử lý chính
                            //listUserRelate: Người tạo, đồng xử lý, giám sát
                            string cc = "";
                            foreach (string u in listUserRelate)
                            {
                                cc += _userRepository.GetUserByUserId(u).Email + ",";
                            }
                            string content = "";
                            string subject = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = taskDetail.DepartmentId, Title = "TASKNOTICEEMAILSUBJECT", DefaultValue = "[VOffice] - Thông báo" });

                            string domain = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(new SystemConfigDepartmentQuery { DepartmentId = taskDetail.DepartmentId, Title = "APP_DOMAIN", DefaultValue = "http://congvan.veph.vn/" });

                            string taskDetailUrl = domain + "#!/chi-tiet-cong-viec/" + model.RecordId.ToString() + "/0////1/true/true";
                            subject += " - Quản lý công việc: " + taskDetail.Code;
                            string temp = @"<div style='border: 3px solid #15c; padding: 10px; overflow: hidden;'>
                                         <table style='width:100%'><tr>
                                            <td><a style='text-decoration:none;' href='" + taskDetailUrl + @"'>[" + taskDetail.Code + "] - " + taskDetail.Title + @"</a></td>
                                        </tr>
                                        <tr>
                                            <td><div style='border-bottom: 1px dotted #ddd; width:100%; margin:10px 0px'></div></td>
                                        </tr>
                                        <tr>
                                            <td>{0}</td>
                                        </tr>
                                        <tr>
                                            <td>{1}</td>
                                        </tr>
                                       <tr><td style='font-style:italic; font-size:10px; color:#888888'>{2}</td></tr>
                                            </table></div>";

                            string actionNotice = "";
                            if (userProfile != null)
                            {
                                actionNotice = "<b>" + userProfile.FullName + "</b>" + " " + model.Description;
                            }
                            string contentNotice = "";
                            if (!string.IsNullOrEmpty(model.NewValue))
                            {
                                contentNotice = model.NewValue;
                            }
                            content = string.Format(temp, actionNotice, contentNotice, model.CreatedOn.ToString("dd/MM/yyyy | HH:mm"));
                            UtilityProvider.SendEmail(taskDetail.DepartmentId, mailTo, cc, "", subject, content);
                        }
                        catch
                        { }
                        #endregion Send email
                        #region Send notification
                        var listUserSendNotifi = new List<SPGetUserNotification_Result>();
                        var listUserRelateDistinct = listUserRelate.Distinct().ToList();
                        foreach (var u in listUserRelateDistinct)
                        {
                            if (u != model.CreatedBy)
                            {
                                listUserSendNotifi.AddRange(_userNotificationRepository.GetUserNotificationByUserId(u));
                            }
                        }
                        NotificationCenter resultNotifiCenter = null;
                        NotificationCenter notificenter = null;
                        NotificationCenter notifiFirst = null;
                        FCMPushNotification fcmPushNotification = new FCMPushNotification();
                        foreach (var item in listUserSendNotifi)
                        {
                            FCMNotificationCenter fcmNotificationCenter = new FCMNotificationCenter();
                            fcmNotificationCenter.Avatar = userProfile.Avatar;
                            fcmNotificationCenter.FullName = userProfile.FullName;
                            fcmNotificationCenter.Title = "vOffice";
                            fcmNotificationCenter.RecordNumber = taskDetail.Code;
                            fcmNotificationCenter.RecordId = taskDetail.Id;
                            fcmNotificationCenter.CreatedBy = model.CreatedBy;
                            fcmNotificationCenter.CreatedOn = DateTime.Now;
                            fcmNotificationCenter.HaveSeen = false;
                            fcmNotificationCenter.ReceivedUserId = item.UserId;
                            fcmNotificationCenter.DeviceId = item.DeviceId;
                            switch (model.Action)
                            {
                                case TaskActivityAction.ADD:
                                    break;
                                case TaskActivityAction.FORWARD:
                                    fcmNotificationCenter.Content = model.Description;
                                    fcmNotificationCenter.SubRelateRecordId = response.Value.Id;
                                    fcmNotificationCenter.Type = (int)NotificationCode.CreateTask;
                                    if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                                    {
                                        fcmNotificationCenter.GroupId = notifiFirst.Id;
                                    }
                                    notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                                    resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                                    if (notifiFirst == null) notifiFirst = resultNotifiCenter;
                                    fcmNotificationCenter.Id = resultNotifiCenter.Id;

                                    fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
                                    break;
                                case TaskActivityAction.RESOLVE:
                                    fcmNotificationCenter.Content = model.Description;
                                    fcmNotificationCenter.SubRelateRecordId = response.Value.Id;
                                    fcmNotificationCenter.Type = (int)NotificationCode.CreateTask;
                                    if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                                    {
                                        fcmNotificationCenter.GroupId = notifiFirst.Id;
                                    }
                                    notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                                    resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                                    if (notifiFirst == null) notifiFirst = resultNotifiCenter;
                                    fcmNotificationCenter.Id = resultNotifiCenter.Id;

                                    fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
                                    break;
                                case TaskActivityAction.REOPEN:
                                    fcmNotificationCenter.Content = model.Description;
                                    fcmNotificationCenter.SubRelateRecordId = response.Value.Id;
                                    fcmNotificationCenter.Type = (int)NotificationCode.CreateTask;
                                    if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                                    {
                                        fcmNotificationCenter.GroupId = notifiFirst.Id;
                                    }
                                    notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                                    resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                                    if (notifiFirst == null) notifiFirst = resultNotifiCenter;
                                    fcmNotificationCenter.Id = resultNotifiCenter.Id;

                                    fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
                                    break;
                                case TaskActivityAction.CLOSE:
                                    fcmNotificationCenter.Content = model.Description;
                                    fcmNotificationCenter.SubRelateRecordId = response.Value.Id;
                                    fcmNotificationCenter.Type = (int)NotificationCode.CreateTask;
                                    if (notifiFirst != null && notifiFirst.ReceivedUserId == item.UserId)
                                    {
                                        fcmNotificationCenter.GroupId = notifiFirst.Id;
                                    }
                                    notificenter = _notificationCenterRepository.ConvertFromCustomNotificationToOrigin(fcmNotificationCenter);
                                    resultNotifiCenter = _notificationCenterRepository.Add(notificenter);
                                    if (notifiFirst == null) notifiFirst = resultNotifiCenter;
                                    fcmNotificationCenter.Id = resultNotifiCenter.Id;

                                    fcmPushNotification.SendNotification(fcmNotificationCenter, item.ClientId);
                                    break;
                            }
                            

                        }
                        #endregion
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
                }
                return response;
            });
        }
        public BaseListResponse<SPGetTaskActivityByTaskId_Result> GetTaskActivityByTaskId(int taskId)
        {
            var response = new BaseListResponse<SPGetTaskActivityByTaskId_Result>();
            try
            {
                response.Data = _taskActivityRepository.GetTaskActivityByTaskId(taskId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }
        public BaseListResponse<TaskOpinionComplex> GetTaskOpinionAndActivityByTaskId(int taskId)
        {
            var response = new BaseListResponse<TaskOpinionComplex>();
            try
            {
                response.Data = _taskActivityRepository.GetTaskOpinionAndActivityByTaskId(taskId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        public BaseListResponse<SPGetTaskOpinionAndActivityByUserId_Result> GetTaskOpinionAndActivityByUserId(string userId, int count)
        {
            var response = new BaseListResponse<SPGetTaskOpinionAndActivityByUserId_Result>();
            try
            {
                response.Data = _taskActivityRepository.GetTaskOpinionAndActivityByUserId(userId, count);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message + " StackTrace: " + ex.StackTrace;
            }
            return response;
        }

        #endregion TaskActivity
    }
}
