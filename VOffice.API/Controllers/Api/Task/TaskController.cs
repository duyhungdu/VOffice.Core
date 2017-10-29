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
    public class TaskController : ApiController
    {
        ITaskService taskService;
        ICategoryService categoryService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_taskService"></param>
        /// <param name="_categoryService"></param>
        public TaskController(ITaskService _taskService, ICategoryService _categoryService)
        {
            taskService = _taskService;
            categoryService = _categoryService;
        }
        /// <summary>
        /// Get a Task by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "tasklist")]
        public BaseResponse<VOffice.Model.Task> Get(int id)
        {
            return taskService.GetTaskById(id);
        }
        /// <summary>
        /// Get a Task by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "tasklist")]
        public BaseResponse<SPGetTaskDetailById_Result> GetTaskDeTail(int id)
        {
            return taskService.GetTaskDetailById(id);
        }
        /// <summary>
        /// Get a Task by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "tasklist")]
        public BaseResponse<ComplexTaskResponse> GetComplexTask(int id)
        {
            return taskService.GetComplexTaskDetailById(id);
        }
        /// <summary>
        /// Get a list of Task via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "tasklist")]
        public BaseListResponse<SPGetTask_Result> Search([FromUri] TaskQuery query)
        {
            return taskService.FilterTask(query);
        }
        /// <summary>
        /// Get All Task
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "tasklist")]
        public BaseListResponse<VOffice.Model.Task> GetAll()
        {
            return null;
        }
        /// <summary>
        /// Insert a Task to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "addtask")]
        public BaseResponse<VOffice.Model.Task> Add(VOffice.Model.Task model)
        {
            return taskService.AddTask(model);
        }

        /// <summary>
        /// Update a Task
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "edittask")]
        public BaseResponse<VOffice.Model.Task> Update(VOffice.Model.Task model)
        {
            return taskService.UpdateTask(model);
        }
        /// <summary>
        /// Mark a Task as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "deletetask")]
        public BaseResponse DeleteLogical(int id)
        {
            return taskService.DeleteLogicalTask(id);
        }

        /// <summary>
        /// Get a TaskCode
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "tasklist")]
        public BaseResponse<string> GetTaskCode(int departmentId)
        {
            return taskService.GetTaskCode(departmentId);
        }

        /// <summary>
        /// Add set of Task - TaskAssignee - TaskDocument
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "addtask")]
        public async System.Threading.Tasks.Task<BaseResponse<VOffice.Model.Task>> AddSetOfTask(ComplexTask model)
        {
            return await taskService.AddSetOfTask(model);
        }
        /// <summary>
        /// Update set of Task - TaskAssignee - TaskDocument
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "edittask")]
        public async System.Threading.Tasks.Task<BaseResponse<VOffice.Model.Task>> UpdateSetOfTask(ComplexTask model)
        {
            return await taskService.UpdateSetOfTask(model);
        }

        /// <summary>
        /// Get list Task of Document
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="receivedDoc"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "tasklist")]
        public BaseListResponse<SPGetTaskByDocumentId_Result> GetTaskByDocumentId(int docId, bool receivedDoc, string userId)
        {
            return taskService.GetTaskByDocumentId(docId, receivedDoc, userId);
        }
        /// <summary>
        /// Count number of default Task via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "tasklist")]
        public BaseResponse<int> CountNewTask([FromUri] TaskQuery query)
        {
            return taskService.CountNewTask(query);
        }
        /// <summary>
        /// Count number of default Task via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskAdvance_Result> GetTaskAdvance([FromUri] TaskQuery query)
        {
            return taskService.GetTaskAdvance(query);
        }
        /// <summary>
        /// Count number of default Task via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<string> DownloadTaskAdvance([FromUri]TaskQuery query)
        {
            return taskService.DownloadTaskAdvance(query);
        }
        /// <summary>
        /// Count user task
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<UserTaskAnalystic> CountUserTask([FromUri]TaskQuery query)
        {
            return taskService.CountUserTask(query);
        }

        /// <summary>
        /// Get a list of Task via SQL Store (Mobile)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTaskMobile_Result> SearchMobile([FromUri] TaskQuery query)
        {
            return taskService.FilterTaskMobile(query);
        }
    }

}
