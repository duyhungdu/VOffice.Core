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
    public class DocumentReceivedController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_documentService"></param>
        public DocumentReceivedController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }

        /// <summary>
        /// Check current user is or isn't DocumentRecipent
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "viewdocumentdetail")]
        public bool CheckUserDocumentReadable([FromUri] DocumentCheckReadableQuery query)
        {
            return documentService.CheckUserDocumentReadable(query);
        }

        /// <summary>
        /// Get a DocumentReceived by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "viewdocumentdetail")]
        public BaseResponse<DocumentReceived> Get(int id)
        {
            return documentService.GetDocumentReceivedById(id);
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
        /// Count number of Document via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "doclist")]
        public BaseResponse<int> CountNewDocument([FromUri] DocumentReceivedQuery query)
        {
            return documentService.CountNewDocument(query);
        }
        /// <summary>
        /// Search a small list with full document info field
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "doclist")]
        public BaseListResponse<SPSearchListDocument_Result> SearchListDocument([FromUri] DocumentReceivedQuery query)
        {
            return documentService.SearchListDocument(query);
        }
        /// <summary>
        /// Get All DocumentReceived
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "doclist")]
        public BaseListResponse<DocumentReceived> GetAll()
        {
            return null;
        }
        /// <summary>
        /// Insert a DocumentReceived to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "adddocumentreceived")]
        public BaseResponse<DocumentReceived> Add(DocumentReceived model)
        {
            return documentService.AddDocumentReceived(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "adddocumentreceived")]
        public BaseListResponse<DocumentReceived> AddSetOfDocumentReceived(List<ComplexDocumentReceived> model)
        {
            return documentService.AddSetOfDocumentReceived(model);
        }
        /// <summary>
        /// Update a DocumentReceived
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "editdocumentreceived")]
        public BaseResponse<DocumentReceived> Update(DocumentReceived model)
        {
            return documentService.UpdateDocumentReceived(model);
        }
        /// <summary>
        /// Mark a DocumentReceived as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="receivedDocument"></param>
        /// <param name="retrievedText"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "deletedocumentreceived")]
        public BaseResponse DeleteLogical(int id, bool receivedDocument, string retrievedText)
        {
            return documentService.DeleteLogicalDocument(id, receivedDocument, retrievedText);
        }
        /// <summary>
        /// Retrieve all Document
        /// </summary>
        /// <param name="id"></param>
        ///  <param name="retrieveText"></param>
        /// <param name="receivedDocument"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "retrievedocument")]
        public BaseResponse RetrieveDocument(int id,string retrieveText, bool receivedDocument)
        {
            return documentService.RetrieveDocument(id, retrieveText, receivedDocument);
        }
        /// <summary>
        /// Forward Current Document
        /// </summary>
        /// <param name="listRecipent"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "forwarddocument")]
        public BaseResponse ForwardDocument(List<DocumentRecipent> listRecipent)
        {
            return documentService.ForwardDocument(listRecipent);
        }
        /// <summary>
        /// Get a Document Detail Received by Id and Type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "viewdocumentdetail")]
        public BaseResponse<SPGetDetailDocument_Result> GetDocumentDetail(string type, int id)
        {
            return documentService.GetDetailDocumentReceived(type, id);
        }

        /// <summary>
        /// Get a list of Document via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "docadded")]
        public BaseListResponse<SPGetAddedDocumentBook_Result> SearchAddedDocumentBook([FromUri] DocumentReceivedQuery query)
        {
            return documentService.FilterAddedDocumentBook(query);
        }
        /// <summary>
        /// Get a list of Departments with status addedBookDocument via SQL Store
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="receivedDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetHistoryAddedBookDocument_Result> GetHistoryAddedBookDocument(int documentId, bool receivedDocument)
        {
            return documentService.GetHistoryAddedDocument(documentId, receivedDocument);
        }
        /// <summary>
        /// Search document info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDocumentAdvance_Result> SearchDocument([FromUri] DocumentReceivedQuery info)
        {
            return documentService.SearchDocument(info);
        }
        /// <summary>
        /// download document book
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]

        public BaseResponse<string> DownloadDocumentBook([FromUri] DocumentReceivedQuery query)
        {
            return documentService.DownloadDocumentBook(query);
        }

        /// <summary>
        /// count total document by 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="listDepartmentId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<string> DownloadTotalDocument(DateTime fromDate, DateTime toDate, string listDepartmentId, int departmentId)
        {
            return documentService.DownloadTotalDocument(fromDate, toDate, listDepartmentId, departmentId);
        }
      /// <summary>
      /// get document query
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetTotalDocumentReportList_Result> DownloadTotalDocumentList([FromUri]DocumentReceivedQuery query)
        {
            return documentService.DownloadTotalDocumentList(query);
        }
        /// <summary>
        /// Get complex count 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<SPGetComplexCount_Result> GetComplexCount(string userId, int departmentId)
        {
            return documentService.GetComplexCount(userId, departmentId); 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<UserDocumentsAnalystic> CountUserDocument([FromUri] DocumentReceivedQuery query)
        {
            return documentService.CountUserDocument(query.NumberOfMonthsOrWeeks, query);
        }


        /// <summary>
        /// Check document has send out
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="receivedDocument"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<bool> CheckHistoryAddedBookDoc(int documentId, bool receivedDocument)
        {
            return documentService.CheckHistoryAddedBookDoc(documentId, receivedDocument);
        }
        /// <summary>
        /// Get document UnRead
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseListResponse<SPGetDocument_Result> GetDocumentUnRead([FromUri]DocumentReceivedQuery query)
        {
            return documentService.GetDocumentUnRead(query);
        }

        /// <summary>
        /// Check permission user document detail
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="documentId"></param>
        /// <param name="receivedDocument"></param>
        /// <param name="listDepartment"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<bool> CheckPermissionUserDocument(string userId, int documentId, bool receivedDocument, string listDepartment)
        {
            return documentService.CheckPermissionUserDocument(userId, documentId, receivedDocument, listDepartment);
        }
    }
}
