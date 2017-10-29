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
    public class TaskTypeController : ApiController
    {
        ITaskService taskService;
        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_taskService"></param>
        public TaskTypeController(ITaskService _taskService)
        {
            taskService = _taskService;
        }
        /// <summary>
        /// Get TaskType by Id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        public BaseResponse<TaskType> Get(int id)
        {
            return taskService.GetTaskTypeById(id);
        }
        /// <summary>
        /// Fetch records by departmentId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskTypeByDepartmentId_Result> GetByDepartment(int departmentId, string keyword)
        {
            return taskService.FilterTaskTypeByDepartmentId(departmentId, keyword);
        }

        /// <summary>
        /// Get a list TaskType via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskType_Result> Search([FromUri] TaskTypeQuery query)
        {
            return taskService.FilterTaskType(query);
        }
        /// <summary>
        /// Add TaskType
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<TaskType> Add(TaskType model)
        {
            return taskService.AddTaskType(model);
        }
        /// <summary>
        /// Update TaskType
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<TaskType> Update(TaskType model)
        {
            return taskService.UpdateTaskType(model);
        }
        /// <summary>
        /// Delete Task Type
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return taskService.DeleteLogicalTaskType(id);
        }
    }
}
