using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;

namespace VOffice.API.Controllers.Api.Document
{
    /// <summary>
    /// TaskType service
    /// </summary>
    [Authorize]
    public class TaskActivityController : ApiController
    {
        ITaskService taskService;
        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_taskService"></param>
        public TaskActivityController(ITaskService _taskService)
        {
            taskService = _taskService;
        }
        /// <summary>
        /// Insert a TaskActivity to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async System.Threading.Tasks.Task<BaseResponse<TaskActivity>> Add(TaskActivity model)
        {

            return await taskService.AddTaskActivity(model);

        }

        /// <summary>
        /// Query list TaskActivity by TaskId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskActivityByTaskId_Result> GetByTaskId(int id)
        {
            return taskService.GetTaskActivityByTaskId(id);
        }
        /// <summary>
        /// Query list TaskActivity and Opinion by TaskId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<TaskOpinionComplex> GetTaskOpinionAndActivityByTaskId(int id)
        {
            return taskService.GetTaskOpinionAndActivityByTaskId(id);
        }

        /// <summary>
        /// Query list TaskActivity and Opinion by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskOpinionAndActivityByUserId_Result> GetTaskOpinionAndActivityByUserId(string userId, int count)
        {
            return taskService.GetTaskOpinionAndActivityByUserId(userId, count);
        }
    }
}
