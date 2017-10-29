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
    public class DocumentSignedByController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_documentService"></param>
        public DocumentSignedByController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }
        /// <summary>
        /// Get DocumentSignedBy by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<DocumentSignedBy> Get(int id)
        {
            return documentService.GetDocumentSignedByById(id);
        }
        /// <summary>
        /// Filter Grid
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDocumentSignedBy_Result> Search([FromUri] DocumentSignedByQuery query)
        {
            return documentService.FilterDocumentSignedBy(query);
        }
        /// <summary>
        /// Fetch all
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<DocumentSignedBy> GetAll()
        {
            return documentService.GetAllDocumentSignedBy();
        }
        /// <summary>
        /// Fetch records by departmentId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<DocumentSignedBy> GetByDepartment(int departmentId)
        {
            return documentService.FilterDocumentSignedByByDepartmentId(departmentId);
        }
        /// <summary>
        /// Insert to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<DocumentSignedBy> Add(DocumentSignedBy model)
        {
            return documentService.AddDocumentSignedBy(model);
        }
        /// <summary>
        /// Update a record
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<DocumentSignedBy> Update(DocumentSignedBy model)
        {
            return documentService.UpdateDocumentSignedBy(model);
        }
        /// <summary>
        /// Mark an item as deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return documentService.DeleteLogicalDocumentSignedBy(id);
        }
    }
}
