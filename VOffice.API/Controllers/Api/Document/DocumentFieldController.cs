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
    [Authorize]
    public class DocumentFieldController : ApiController
    {
        IDocumentService documentService;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="_documentService"></param>
        public DocumentFieldController(IDocumentService _documentService)
        {
            documentService = _documentService;
        }
        /// <summary>
        /// Get a DocumentField by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
       
        public BaseResponse<DocumentField> Get(int id)
        {
            return documentService.GetDocumentFieldById(id);
        }
        /// <summary>
        /// Get a list of DocumentField via SQL Store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
       
        public BaseListResponse<SPGetDocumentField_Result> Search([FromUri] DocumentFieldQuery query)
        {
            return documentService.FilterDocumentField(query);
        }
        /// <summary>
        /// Get All DocumentField
        /// </summary>
        /// <returns></returns>
        [HttpGet]
       
        public BaseListResponse<DocumentField> GetAll()
        {
            return documentService.GetAllDocumentField();
        }
        /// <summary>
        /// Get All DocumentField not in Department
        /// </summary>
        /// <returns></returns>
        [HttpGet]
      
        public BaseListResponse<SPCopyDocumentField_Result> GetDocumentFieldaNotInDepartment(int DepartmentId)
        {
            return documentService.GetDocumentFieldaNotInDepartment(DepartmentId);
        }
        /// <summary>
        /// Insert a DocumentField to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
      
        public BaseResponse<DocumentField> Add(DocumentField model)
        {
            return documentService.AddDocumentField(model);
        }
        /// <summary>
        /// Insert a DocumentField and clone for department to Database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        
        public BaseResponse<DocumentField> AddDocumentFieldSystem(DocumentField model)
        {
            return documentService.AddDocumentFieldSystem(model);
        }
        /// <summary>
        /// Copy DocumentField for Department
        /// </summary>
        /// <param name="ListDepartmentId"></param>
        /// <returns></returns>
        [HttpPut]
       
        public BaseResponse CloneDocumentFieldSystem(string ListDepartmentId)
        {
            return documentService.CloneDocumentFieldSystem(ListDepartmentId);
        }
        /// <summary>
        /// Update a DocumentField
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <summary>
        [HttpPut]
       
        public BaseResponse<DocumentField> Update(DocumentField model)
        {
            return documentService.UpdateDocumentField(model);
        }
        /// <summary>
        /// Mark a DocumentField as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
       
        public BaseResponse DeleteLogical(int id)
        {
            return documentService.DeleteLogicalDocumentField(id);
        }
    }

}
