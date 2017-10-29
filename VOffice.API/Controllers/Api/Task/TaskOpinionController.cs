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
    public class TaskOpinionController : ApiController
    {
        ITaskService taskService;
        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_taskService"></param>
        public TaskOpinionController(ITaskService _taskService)
        {
            taskService = _taskService;
        }
        /// <summary>
        /// Insert a TaskOpinion to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "addtaskopinion")]
        public async System.Threading.Tasks.Task<BaseResponse<TaskOpinion>> Add(TaskOpinion model)
        {
            BaseResponse<TaskOpinion> result = new BaseResponse<TaskOpinion>();
            result = await taskService.AddTaskOpinion(model);
            return result;
        }
        /// <summary>
        /// Update TaskOpinion 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "addtaskopinion")]
        public BaseResponse<TaskOpinion> Update(TaskOpinion model)
        {
            return taskService.UpdateTaskOpinion(model);
        }
        /// <summary>
        /// Query list Opinion by TaskId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "taskopinion")]
        public BaseListResponse<SPGetTaskOpinionByTaskId_Result> GetByTaskId(int id)
        {
            return taskService.GetTaskOpinionByTaskId(id);
        }
        /// <summary>
        /// Get TaskOpinion by OpinionId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseResponse<TaskOpinionComplex> GetTaskOpinionByOpinionId(int id)
        {
            return taskService.GetTaskOpinionByOpinionId(id);
        }

    }
}
