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
    /// Document Recipent
    /// </summary>
    public class DocumentRecipentController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_documentService"></param>
        public DocumentRecipentController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }
        /// <summary>
        /// Insert a DocumentRecipent to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<DocumentRecipent> Add(DocumentRecipent model)
        {
            return documentService.AddDocumentRecipent(model);
        }
        /// <summary>
        /// Insert a List Recipent to Database
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseListResponse<DocumentRecipent> AddDocumentRecipents(List<DocumentRecipent> models)
        {
            return documentService.AddDocumentRecipents(models);
        }
        /// <summary>
        /// Delete list DocumentRecipent
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse DeleteDocumentRecipents(List<DocumentRecipent> models)
        {
            return documentService.DeleteDocumentRecipents(models);
        }
        /// <summary>
        /// Delete list DocumentRecipent by DocumentId and ReceivedDocument
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse DeleteRecipentsByDocIdAndReceivedDoc(CustomDocumentRecipent model)
        {
            return documentService.DeleteRecipentsByDocIdAndReceivedDoc(model);
        }
        /// <summary>
        /// Get a list of DocumentRecipents by DocumentId and ReceivedDocument and DepartmentId
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="receivedDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDocumentRecipentByDocIdAndRecivedDoc_Result> GetListRecipentsByDocIdAndReceivedDoc(int documentId, bool receivedDocument)
        {
            return documentService.GetDocRecipentsByDocIdAndReceived(documentId, receivedDocument);
        }
        /// <summary>
        /// Update a DocumentRecipent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<DocumentRecipent> UpdateDocumentRecipentByDocIdAndReceivedDoc(DocumentRecipent model)
        {
            return documentService.UpdateDocumentRecipentByDocIdAndReceivedDoc(model);
        }
    }

}