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
    /// DocumentField for Department
    /// </summary>
    [Authorize]
    public class DocumentFieldDepartmentController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_documentService"></param>
        public DocumentFieldDepartmentController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }
        /// <summary>
        /// Get a DocumentField by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        
        public BaseResponse<DocumentFieldDepartment> Get(int id)
        {
            return documentService.GetDocumentFieldDepartmentById(id);
        }
        /// <summary>
        /// Get a list of DocumentFieldDepartment via SQL Store 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
       
        public BaseListResponse<SPGetListDocumentFieldDepartment_Result> Search([FromUri] DocumentFieldDepartmentQuery query)
        {
            return documentService.GetListDocumentFieldDepartment(query);
        }
        /// <summary>
        /// Get a list of DocumentFieldDepartment when add Document....
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        [HttpGet]
       
        public BaseListResponse<SPGetDocumentFieldDepartment_Result> Filter(int departmentID)
        {
            return documentService.FilterDocumentFieldDepartment(departmentID);
        }
        /// <summary>
        /// Get a list of DocumentFieldDepartment when has DocumentId and RecceivedDocument....
        /// </summary>
        /// <param name="documentId"></param> 
        /// <param name="receivedDocument"></param>
        /// <returns></returns>
        [HttpGet]
       
        public BaseListResponse<SPGetDocFieldDepartmentByDocIdAndReceivedDoc_Result> GetDocFieldDeaprtmentByDocIdAndReceivedDoc(int documentId, bool receivedDocument)
        {
            return documentService.GetDocFieldDeaprtmentByDocIdAndReceivedDoc(documentId, receivedDocument);
        }
        /// <summary>
        /// Get All DocumentFieldDepartment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        
        public BaseListResponse<DocumentFieldDepartment> GetAll()
        {
            return documentService.GetAllDocumentFieldDepartment();
        }
        /// <summary>
        /// Insert a DocumentFieldDepartment to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        
        public BaseResponse<DocumentFieldDepartment> Add(DocumentFieldDepartment model)
        {
            return documentService.AddDocumentFieldDepartment(model);
        }
        /// <summary>
        /// Update a DocumentFieldDepartment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
       
        public BaseResponse<DocumentFieldDepartment> Update(DocumentFieldDepartment model)
        {
            return documentService.UpdateDocumentFieldDepartment(model);
        }
        /// <summary>
        /// Get documentFieldDepartment of list Department from list object documentFieldSystem of Send Place
        /// </summary>
        /// <param name="listInputDocFieldDepartment"></param>
        /// <returns></returns>
        [HttpPost]
        
        public BaseListResponse<CustomDocumentField> GetOutputDocFieldDepartment(List<CustomDocumentField> listInputDocFieldDepartment)
        {
            return documentService.GetListDocFieldDepartmentFromSystem(listInputDocFieldDepartment);
        }
        /// <summary>
        /// Mark a DocumentFieldDepartment as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        
        public BaseResponse DeleteLogical(int id)
        {
            return documentService.DeleteLogicalDocumentFieldDepartment(id);
        }
    }
}