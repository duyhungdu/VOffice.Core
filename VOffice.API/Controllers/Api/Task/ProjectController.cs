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
    /// Task.Project service
    /// </summary>
    [Authorize]
    public class ProjectController : ApiController
    {
        ITaskService taskService;
        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_taskService"></param>
        public ProjectController(ITaskService _taskService)
        {
            taskService = _taskService;
        }
        /// <summary>
        /// Get a Project by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public BaseResponse<Project> Get(int id)
        {
            return taskService.GetProjectById(id);
        }
        /// <summary>
        /// Fetch records by departmentId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public BaseListResponse<SPGetProjectByDepartmentId_Result> GetByDepartment(int departmentId, string keyword)
        {
            return taskService.FilterProjectByDepartmentId(departmentId, keyword);
        }

        /// <summary>
        /// Get a list of Project via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public BaseListResponse<SPGetProject_Result> Search([FromUri] ProjectQuery query)
        {
            return taskService.Filter(query);
        }
        /// <summary>
        /// Add Project
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public BaseResponse<Project> Add(Project model)
        {
            return taskService.AddProject(model);
        }
        /// <summary>
        /// Update Project
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public BaseResponse<Project> Update(Project model)
        {
            return taskService.UpdateProject(model);
        }
        /// <summary>
        /// Delete Project
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public BaseResponse DeleteLogical(int id)
        {
            return taskService.DeleteLogicalProject(id);
        }

    }
}
