using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.ApplicationService.Implementation.Contract
{
    public interface ITaskService : IService
    {
        #region Task
        BaseResponse<VOffice.Model.Task> GetTaskById(int id);
        BaseResponse<SPGetTaskDetailById_Result> GetTaskDetailById(int id);
        BaseResponse<ComplexTaskResponse> GetComplexTaskDetailById(int id);
        BaseResponse<VOffice.Model.Task> AddTask(VOffice.Model.Task model);
        BaseResponse<VOffice.Model.Task> UpdateTask(VOffice.Model.Task model);
        BaseListResponse<SPGetTask_Result> FilterTask(TaskQuery query);
        System.Threading.Tasks.Task<BaseResponse<VOffice.Model.Task>> AddSetOfTask(ComplexTask model);
        System.Threading.Tasks.Task<BaseResponse<VOffice.Model.Task>> UpdateSetOfTask(ComplexTask model);
        BaseResponse<string> GetTaskCode(int departmentId);
        BaseListResponse<SPGetTaskByDocumentId_Result> GetTaskByDocumentId(int docId, bool receivedDoc, string userId);
        BaseResponse<int> CountNewTask(TaskQuery query);
        BaseListResponse<SPGetTaskAdvance_Result> GetTaskAdvance(TaskQuery query);
        BaseResponse<string> DownloadTaskAdvance(TaskQuery query);
        BaseResponse<UserTaskAnalystic> CountUserTask(TaskQuery query);
        BaseListResponse<SPGetTaskMobile_Result> FilterTaskMobile(TaskQuery query);
        #endregion Task
        #region TaskDocuments
        BaseResponse<TaskDocument> AddTaskDocument(TaskDocument model);
        BaseListResponse<SPGetTaskDocumentByTaskId_Result> GetTaskDocumentByTaskId(int taskId);
        #endregion TaskDocuments
        #region TaskAssignee
        BaseListResponse<TaskAssignee> AddTaskAssignee(List<TaskAssignee> models);
        BaseListResponse<SPGetTaskAssigneeByTaskId_Result> GetTaskAssigneeByTaskId(int taskId);
        BaseListResponse<SPGetTaskAssigneeByTaskId_Result> GetTaskAssigneeViewDetail(int taskId);
        BaseResponse DeleteLogicalTask(int id);
        BaseResponse<TaskAssignee> UpdateTaskAssignee(TaskAssigneeQuery model);

        BaseResponse<TaskAssignee> ViewTaskDetail(string userId,int taskId);
        BaseListResponse<SPGetTaskDocumentHistory_Result> GetTaskAssigneeDocumentHistory(int taskId, int documentId, string documentReceived);
        System.Threading.Tasks.Task<BaseListResponse<TaskAssignee>> AddMoreTaskAssignee(List<TaskAssignee> models);
        #endregion TaskAssignee
        #region Project
        BaseResponse<Project> GetProjectById(int id);
        BaseListResponse<SPGetProjectByDepartmentId_Result> FilterProjectByDepartmentId(int departmentId, string keyword);
        BaseListResponse<SPGetProject_Result> Filter(ProjectQuery query);
        BaseResponse<Project> AddProject(Project model);
        BaseResponse<Project> UpdateProject(Project model);
        BaseResponse DeleteLogicalProject(int id);
        #endregion Project
        #region TaskType
        BaseResponse<TaskType> GetTaskTypeById(int id);
        BaseListResponse<SPGetTaskTypeByDepartmentId_Result> FilterTaskTypeByDepartmentId(int departmentId, string keyword);
        BaseListResponse<SPGetTaskType_Result> FilterTaskType(TaskTypeQuery query);
        BaseResponse<TaskType> AddTaskType(TaskType model);
        BaseResponse<TaskType> UpdateTaskType(TaskType model);
        BaseResponse DeleteLogicalTaskType(int id);
        #endregion TaskType
        #region TaskAttachment
        BaseListResponse<TaskAttachment> AddTaskAttachment(List<TaskAttachment> models);
        BaseListResponse<TaskAttachment> UpdateTaskAttachment(List<TaskAttachment> models);
        BaseListResponse<SPGetTaskAttachmentByRecordId_Result> GetTaskAttachmentByRecordId(string type, int recordId);
        #endregion TaskAttachment
        #region TaskOpinion
        System.Threading.Tasks.Task<BaseResponse<TaskOpinion>> AddTaskOpinion(TaskOpinion model);
        BaseListResponse<SPGetTaskOpinionByTaskId_Result> GetTaskOpinionByTaskId(int taskId);
        BaseResponse<TaskOpinion> UpdateTaskOpinion(TaskOpinion model);

        BaseResponse<TaskOpinionComplex> GetTaskOpinionByOpinionId(int id);
        #endregion TaskOpinion
        #region TaskActivity
        System.Threading.Tasks.Task<BaseResponse<TaskActivity>> AddTaskActivity(TaskActivity model);
        BaseListResponse<SPGetTaskActivityByTaskId_Result> GetTaskActivityByTaskId(int taskId);
        BaseListResponse<TaskOpinionComplex> GetTaskOpinionAndActivityByTaskId(int taskId);
        BaseListResponse<SPGetTaskOpinionAndActivityByUserId_Result> GetTaskOpinionAndActivityByUserId(string userId, int count);
        #endregion TaskActivity
        BaseResponse<bool> CheckPermissionUserTask(int taskId, string userId);
    }
}
