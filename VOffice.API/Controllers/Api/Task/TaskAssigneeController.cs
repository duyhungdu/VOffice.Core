using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;


namespace VOffice.API.Controllers.Api.Task
{
    /// <summary>
    /// DocumentType API. An element of DocumentService
    /// </summary>
    [Authorize]
    public class TaskAssigneeController : ApiController
    {
        ITaskService taskService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_taskService"></param>
        public TaskAssigneeController(ITaskService _taskService)
        {
            taskService = _taskService;
        }
        /// <summary>
        /// Insert a List Assignee to Database
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseListResponse<TaskAssignee> Add(List<TaskAssignee> models)
        {
            return taskService.AddTaskAssignee(models);
        }
        /// <summary>
        /// Fetch records by taskId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskAssigneeByTaskId_Result> GetByTask(int taskId)
        {
            return taskService.GetTaskAssigneeByTaskId(taskId);
        }
        /// <summary>
        /// Fetch records viewdetail by taskId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskAssigneeByTaskId_Result> GetTaskAssigneeViewDetail(int taskId)
        {
            return taskService.GetTaskAssigneeViewDetail(taskId);
        }

        /// <summary>
        /// Fetch document history for Task Assignee
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskDocumentHistory_Result> GetTaskDocumentHistory(int taskId, int documentId, string documentReceived)
        {
            return taskService.GetTaskAssigneeDocumentHistory(taskId, documentId, documentReceived);
        }

        /// <summary>
        /// Update TaskAssignee Assignee
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<TaskAssignee> UpdateTaskAssigneeAssignee(TaskAssigneeQuery model)
        {
            return taskService.UpdateTaskAssignee(model);
        }
        /// <summary>
        /// Update TaskAssignee View TaskDetail
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<TaskAssignee> ViewTaskDetail(string userId, int taskId)
        {
            return taskService.ViewTaskDetail(userId, taskId);
        }

        /// <summary>
        /// Update More TaskAssignee
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async System.Threading.Tasks.Task<BaseListResponse<TaskAssignee>> AddMoreTaskAssignee(List<TaskAssignee> models)
        {
            return await taskService.AddMoreTaskAssignee(models);
        }

        /// <summary>
        /// check permission user by taskId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<bool> CheckPermissionUserByTaskId(int taskId, string userId)
        {
            return taskService.CheckPermissionUserTask(taskId, userId);
        }
    }

}
