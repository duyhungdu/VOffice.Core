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
    /// DocumentField API. An element of DocumentService
    /// </summary>
    public class DocumentDocumentFieldController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_documentService"></param>
        public DocumentDocumentFieldController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }
        /// <summary>
        /// Get a DocumentDocumentField by DocumentId and DocmentFieldDepartmentId
        /// </summary>
        /// <param name="DocId"></param>
        /// <param name="DocumentFileDepartmentId"></param>
        /// <param name="ReceivedDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public bool Get(int DocId, int DocumentFileDepartmentId, bool ReceivedDocument)
        {
            return documentService.GetDocDocFieldByDocIdDocFieldDepartmentId(DocId, DocumentFileDepartmentId, ReceivedDocument);
        }
        /// <summary>
        /// Get a list of DocumentDocumentField via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDocumentDocumentField_Result> Search([FromUri] DocumentDocumentFieldQuery query)
        {
            return documentService.FilterDocumentDocumentField(query);
        }
        /// <summary>
        /// Get All DocumentDocumentField
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<DocumentDocumentField> GetAll()
        {
            return documentService.GetAllDocumentDocumentField();
        }

        /// <summary>
        /// Insert a DocumentDocumentField to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<DocumentDocumentField> Add(DocumentDocumentField model)
        {
            return documentService.AddDocumentDocumentField(model);
        }
        /// <summary>
        /// Insert List DocumentDocumentField to Database
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseListResponse<DocumentDocumentField> AddDocumentDocumentFields(List<DocumentDocumentField> models)
        {
            return documentService.AddDocumentDocumentFields(models);
        }
        /// <summary>
        /// Update a DocumentDocumentField
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<DocumentDocumentField> Update(DocumentDocumentField model)
        {
            return documentService.UpdateDocumentDocumentField(model);
        }
        /// <summary>
        /// Mark a DocumentDocumentField as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogical(int id)
        {
            return documentService.DeleteLogicalDocumentDocumentField(id);
        }

        /// <summary>
        /// Delete list DocumentRecipent
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse DeleteDocumentDocumentFields(List<DocumentDocumentField> models)
        {
            return documentService.DeleteDocumentDocumentFields(models);
        }
        /// <summary>
        /// delete list document document field by document id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse DeleteDocDocumentFieldsByDocIdAndReceivedDoc(CustomDocumentRecipent model)
        {
            return documentService.DeleteDocDocumentFieldsByDocIdAndReceivedDoc(model);
        }
    }

}
