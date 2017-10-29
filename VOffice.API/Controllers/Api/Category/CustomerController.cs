using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using VOffice.ApplicationService.Implementation.Contract;

namespace VOffice.API.Controllers.Api.Category
{
    /// <summary>
    /// Customer API. An element of CategoryService
    /// </summary>
    [Authorize]
    public class CustomerController : ApiController
    {
        ICategoryService categoryService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_categoryService"></param>
        public CustomerController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }
        /// <summary>
        /// Get a Customer by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<Customer> Get(int id)
        {
            return categoryService.GetCustomerById(id);
        }
        /// <summary>
        /// Get a list of Customers via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetCustomer_Result> Search([FromUri] CustomerQuery query)
        {
            return categoryService.FilterCustomer(query);
        }
        /// <summary>
        /// Fetch records by departmentId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetCustomerByDepartmentId_Result> GetByDepartment(int departmentId, string keyword)
        {
            return categoryService.FilterCustomerByDepartmentId(departmentId, keyword);
        }
        /// <summary>
        /// Get All Customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<Customer> GetAll()
        {
            return categoryService.GetAllCustomer();
        }
        /// <summary>
        /// Insert a Customer to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<Customer> Add(Customer model)
        {
            return categoryService.AddCustomer(model);
        }
        /// <summary>
        /// Update a Customer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<Customer> Update(Customer model)
        {
            return categoryService.UpdateCustomer(model);
        }
        /// <summary>
        /// Mark a Customer as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return categoryService.DeleteLogicalCustomer(id);
        }
    }
}