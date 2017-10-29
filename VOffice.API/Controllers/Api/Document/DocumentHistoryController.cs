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
    /// DocumentHistory API. An element of DocumentService
    /// </summary>
    public class DocumentHistoryController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_documentService"></param>
        public DocumentHistoryController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }
        /// <summary>
        /// Insert a DocumentHistory to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<DocumentHistory> Add(DocumentHistory model)
        {
            return documentService.AddDocumentHistory(model);
        }

        /// <summary>
        /// Get DocumentHistories from Database
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="receivedDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetHistoryDocument_Result> GetHistoryDocument(int documentId, bool receivedDocument)
        {
            return documentService.GetHistoryDocument(documentId,receivedDocument);
        }
    }

}
