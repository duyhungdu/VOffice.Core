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
    public class DocumentDeliveredController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_documentService"></param>
        public DocumentDeliveredController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }
        /// <summary>
        /// Get a DocumentReceived by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "viewdocumentdetail")]
        public BaseResponse<DocumentDelivered> Get(int id)
        {
            return documentService.GetDocumentDeliveredById(id);
        }
        /// <summary>
        /// Get a list of Document via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "doclist")]
        public BaseListResponse<SPGetDocument_Result> Search([FromUri] DocumentReceivedQuery query)
        {
            return documentService.FilterDocument(query);
        }
        /// <summary>
        /// Get All DocumentReceived
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "doclist")]
        public BaseListResponse<DocumentDelivered> GetAll()
        {
            return null;
        }
        /// <summary>
        /// Insert a DocumentReceived to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "adddelivereddocument")]
        public BaseResponse<DocumentDelivered> Add(DocumentDelivered model)
        {
            return documentService.AddDocumentDelivered (model);
        }
        /// <summary>
        /// Add complex document delivered
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "adddelivereddocument")]
        public  BaseResponse<DocumentDelivered> AddComplexDocumentDelivered(ComplexDocumentDelivered model)
        {
            return documentService.AddComplexDocumentDelivered(model);
        }
        /// <summary>
        /// Update a DocumentReceived
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "editdelivereddocument")]
        public BaseResponse<DocumentDelivered> Update(DocumentDelivered model)
        {
            return documentService.UpdateDocumentDelivered(model);
        }
        /// <summary>
        /// Update complex document delivered
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "editdelivereddocument")]
        public BaseResponse<DocumentDelivered> UpdateComplexDocumentDelivered(ComplexDocumentDelivered model)
        {
            return documentService.UpdateComplexDocumentDelivered(model);
        }
        /// <summary>
        /// Mark a DocumentReceived as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="receivedDocument"></param>
        /// <param name="retrievedText"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "deletedelivereddocument")]
        public BaseResponse DeleteLogical(int id, bool receivedDocument,string retrievedText)
        {
            return documentService.DeleteLogicalDocument(id, receivedDocument, retrievedText);
        }
        /// <summary>
        /// get document delivered query list
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDocumentDeliveredStatistics_Result> DocumentDeliveredStatisticsList([FromUri]DocumentDeliveredStatisticsQuery query)
        {
            return documentService.GetDocumentDeliveredStatisticsList(query);
        }
        /// <summary>
        /// down load file report
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<string> DocumentDeliveredStatisticsListDownLoadFile([FromUri]DocumentDeliveredStatisticsQuery query)
        {
            return documentService.GetDocumentDeliveredStatisticsDownload(query);
        }
    }
}
