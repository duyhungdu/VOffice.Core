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
    /// DocumentType API. An element of DocumentService
    /// </summary>
    [Authorize]
    public class DocumentTypeController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_documentService"></param>
        public DocumentTypeController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }
        /// <summary>
        /// Get a DocumentType by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<DocumentType> Get(int id)
        {
            return documentService.GetDocumentTypeById(id);
        }
        /// <summary>
        /// Get a list of DocumentType via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDocumentType_Result> Search([FromUri] DocumentTypeQuery query)
        {
            return documentService.FilterDocumentType(query);
        }
        /// <summary>
        /// Get All DocumentType
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<DocumentType> GetAll()
        {
            return documentService.GetAllDocumentType();
        }
        /// <summary>
        /// Insert a DocumentType to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<DocumentType> Add(DocumentType model)
        {
            return documentService.AddDocumentType(model);
        }
        /// <summary>
        /// Update a DocumentType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<DocumentType> Update(DocumentType model)
        {
            return documentService.UpdateDocumentType(model);
        }
        /// <summary>
        /// Mark a DocumentType as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return documentService.DeleteLogicalDocumentType(id);
        }
    }

}
