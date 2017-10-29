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
    /// DocumentSignedBy service
    /// </summary>
    [Authorize]
    public class ExternalSendReceiveDivisionController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_documentService"></param>
        public ExternalSendReceiveDivisionController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }
        /// <summary>
        /// Get ExternalSendReceiveDivision by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<ExternalSendReceiveDivision> Get(int id)
        {
            return documentService.GetExternalSendReceiveDivisionById(id);
        }
        /// <summary>
        /// Filter Grid
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetExternalSendReceiveDivision_Result> Search([FromUri] ExternalSendReceiveDivisionQuery query)
        {
            return documentService.FilterExternalSendReceiveDivision(query);
        }
        /// <summary>
        /// Fetch all
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<ExternalSendReceiveDivision> GetAll()
        {
            return documentService.GetAllExternalSendReceiveDivision();
        }
        /// <summary>
        /// Fetch records by departmentId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<ExternalSendReceiveDivision> GetByDepartment(int departmentId)
        {
            return documentService.FilterExternalSendReceiveDivisionByDepartmentId(departmentId);
        }
        /// <summary>
        /// Insert to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<ExternalSendReceiveDivision> Add(ExternalSendReceiveDivision model)
        {
            return documentService.AddExternalSendReceiveDivision(model);
        }
        /// <summary>
        /// Update a record
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<ExternalSendReceiveDivision> Update(ExternalSendReceiveDivision model)
        {
            return documentService.UpdateExternalSendReceiveDivision(model);
        }
        /// <summary>
        /// Mark an item as deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return documentService.DeleteLogicalExternalSendReceiveDivision(id);
        }
    }
}
