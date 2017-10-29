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
    public class TaskDocumentsController : ApiController
    {
        ITaskService taskService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_taskService"></param>
        public TaskDocumentsController(ITaskService _taskService)
        {
            taskService = _taskService;
        }
        /// <summary>
        /// Insert a TaskDocuments to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<TaskDocument> Add(TaskDocument model)
        {
            return taskService.AddTaskDocument(model);
        }

        /// <summary>
        /// Fetch records by taskId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskDocumentByTaskId_Result> GetByTask(int taskId)
        {
            return taskService.GetTaskDocumentByTaskId(taskId);
        }
    }

}
