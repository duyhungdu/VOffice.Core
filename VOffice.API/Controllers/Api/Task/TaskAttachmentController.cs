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
    public class TaskAttachmentController : ApiController
    {
        ITaskService taskService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_taskService"></param>
        public TaskAttachmentController(ITaskService _taskService)
        {
            taskService = _taskService;
        }
        /// <summary>
        /// Insert a List Attachment to Database
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseListResponse<TaskAttachment> AddListAttachment(List<TaskAttachment> models)
        {
            return taskService.AddTaskAttachment(models);
        }
        /// <summary>
        /// Insert a List Attachment to Database
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseListResponse<TaskAttachment> UpdateListAttachment(List<TaskAttachment> models)
        {
            return taskService.UpdateTaskAttachment(models);
        }
        /// <summary>
        /// Fetch records by taskId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskAttachmentByRecordId_Result> GetByTask(string type, int recordId)
        {
            return taskService.GetTaskAttachmentByRecordId(type, recordId);
        }
    }

}
