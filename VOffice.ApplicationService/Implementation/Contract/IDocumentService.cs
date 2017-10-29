using VOffice.Core.Messages;
using VOffice.Model;
using VOffice.Repository.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.ApplicationService.Implementation.Contract
{
    public interface IDocumentService : IService
    {
        #region DocumentType
        BaseResponse<DocumentType> GetDocumentTypeById(int id);
        BaseListResponse<DocumentType> GetAllDocumentType();
        BaseListResponse<SPGetDocumentType_Result> FilterDocumentType(DocumentTypeQuery query);
        BaseResponse<DocumentType> AddDocumentType(DocumentType model);
        BaseResponse<DocumentType> UpdateDocumentType(DocumentType model);
        BaseResponse DeleteLogicalDocumentType(int id);
        #endregion DocumentType

        #region DocumentSignedBy
        BaseResponse<DocumentSignedBy> GetDocumentSignedByById(int id);
        BaseListResponse<DocumentSignedBy> GetAllDocumentSignedBy();
        BaseListResponse<SPGetDocumentSignedBy_Result> FilterDocumentSignedBy(DocumentSignedByQuery query);
        BaseListResponse<DocumentSignedBy> FilterDocumentSignedByByDepartmentId(int departmentId);
        BaseResponse<DocumentSignedBy> AddDocumentSignedBy(DocumentSignedBy model);
        BaseResponse<DocumentSignedBy> UpdateDocumentSignedBy(DocumentSignedBy model);
        BaseResponse DeleteLogicalDocumentSignedBy(int id);
        #endregion DocumentSignedBy

        #region ExternalSendReceiveDivision
        BaseResponse<ExternalSendReceiveDivision> GetExternalSendReceiveDivisionById(int id);
        BaseListResponse<ExternalSendReceiveDivision> GetAllExternalSendReceiveDivision();
        BaseListResponse<SPGetExternalSendReceiveDivision_Result> FilterExternalSendReceiveDivision(ExternalSendReceiveDivisionQuery query);
        BaseListResponse<ExternalSendReceiveDivision> FilterExternalSendReceiveDivisionByDepartmentId(int departmentId);
        BaseResponse<ExternalSendReceiveDivision> AddExternalSendReceiveDivision(ExternalSendReceiveDivision model);
        BaseResponse<ExternalSendReceiveDivision> UpdateExternalSendReceiveDivision(ExternalSendReceiveDivision model);
        BaseResponse DeleteLogicalExternalSendReceiveDivision(int id);
        #endregion ExternalSendReceiveDivision

        #region Document
        #region DocumentReceived
        BaseListResponse<SPGetDocument_Result> FilterDocument(DocumentReceivedQuery query);
        BaseResponse<int> CountNewDocument(DocumentReceivedQuery query);
        //BaseResponse<int> CountNewDocument(DocumentReceivedQuery query);
        BaseListResponse<SPSearchListDocument_Result> SearchListDocument(DocumentReceivedQuery query);
        BaseResponse<DocumentReceived> GetDocumentReceivedById(int id);
        BaseResponse<DocumentReceived> AddDocumentReceived(DocumentReceived model);
        BaseListResponse<DocumentReceived> AddSetOfDocumentReceived(List<ComplexDocumentReceived> model);
        BaseResponse<DocumentReceived> UpdateDocumentReceived(DocumentReceived model);
        BaseResponse DeleteLogicalDocument(int id, bool receivedDocument,string retrievedText);
        BaseResponse RetrieveDocument(int id, string retrieveText, bool receivedDocument);
        BaseListResponse<DocumentRecipent> ForwardDocument(List<DocumentRecipent> listRecipent);
        bool CheckReceivedNumber(int departmentId, string receivedNumber);
        BaseResponse<SPGetDetailDocument_Result> GetDetailDocumentReceived(string type, int id);
        BaseListResponse<SPGetDocument_Result> GetDocumentUnRead(DocumentReceivedQuery query);
        #endregion DocumentReceived

        #region DocumentDelivered
        BaseResponse<DocumentDelivered> GetDocumentDeliveredById(int id);
        BaseResponse<DocumentDelivered> AddDocumentDelivered(DocumentDelivered model);
        BaseResponse<DocumentDelivered> AddComplexDocumentDelivered(ComplexDocumentDelivered model);
        BaseResponse<DocumentDelivered> UpdateDocumentDelivered(DocumentDelivered model);
        BaseResponse<DocumentDelivered> UpdateComplexDocumentDelivered(ComplexDocumentDelivered model);
        #endregion DocumentDelivered

        BaseListResponse<SPGetDocumentAdvance_Result> SearchDocument(DocumentReceivedQuery info);
        BaseListResponse<SPGetTotalDocumentReportList_Result> DownloadTotalDocumentList(DocumentReceivedQuery query);
        BaseResponse<SPGetComplexCount_Result> GetComplexCount(string userId, int departmentId);

        BaseResponse<UserDocumentsAnalystic> CountUserDocument(int NumberOfMonths, DocumentReceivedQuery query);

        BaseListResponse<SPGetDocumentDeliveredStatistics_Result> GetDocumentDeliveredStatisticsList(DocumentDeliveredStatisticsQuery info);
        BaseResponse<string> GetDocumentDeliveredStatisticsDownload(DocumentDeliveredStatisticsQuery query);
        #endregion

        #region DocumentHistory
        BaseResponse<DocumentHistory> AddDocumentHistory(DocumentHistory model);
        #endregion DocumentHistory

        #region DocumentField
        BaseResponse<DocumentField> GetDocumentFieldById(int id);
        BaseListResponse<DocumentField> GetAllDocumentField();
        BaseListResponse<SPGetDocumentField_Result> FilterDocumentField(DocumentFieldQuery query);
        BaseResponse<DocumentField> AddDocumentField(DocumentField model);
        BaseResponse<DocumentField> AddDocumentFieldSystem(DocumentField model);
        BaseListResponse<SPCopyDocumentField_Result> GetDocumentFieldaNotInDepartment(int departmentId);
        BaseResponse CloneDocumentFieldSystem(string listDepartmentId);
        BaseResponse<DocumentField> UpdateDocumentField(DocumentField model);
        BaseResponse DeleteLogicalDocumentField(int id);
        #endregion DocumentField

        #region DocumentFieldDepartment
        BaseResponse<DocumentFieldDepartment> GetDocumentFieldDepartmentById(int id);
        BaseListResponse<DocumentFieldDepartment> GetAllDocumentFieldDepartment();
        BaseListResponse<SPGetDocumentFieldDepartment_Result> FilterDocumentFieldDepartment(int departmentID);
        BaseListResponse<SPGetListDocumentFieldDepartment_Result> GetListDocumentFieldDepartment(DocumentFieldDepartmentQuery query);
        BaseResponse<DocumentFieldDepartment> AddDocumentFieldDepartment(DocumentFieldDepartment model);
        BaseResponse<DocumentFieldDepartment> UpdateDocumentFieldDepartment(DocumentFieldDepartment model);
        BaseResponse DeleteLogicalDocumentFieldDepartment(int id);
        BaseListResponse<CustomDocumentField> GetListDocFieldDepartmentFromSystem(List<CustomDocumentField> listDocumentFieldDepartment);
        BaseListResponse<SPGetDocFieldDepartmentByDocIdAndReceivedDoc_Result> GetDocFieldDeaprtmentByDocIdAndReceivedDoc(int docId, bool receivedDoc);

        #endregion DocumentField

        #region DocumentDocumentField
        bool GetDocDocFieldByDocIdDocFieldDepartmentId(int docId, int docFieldDepartmentId, bool receivedDocument);
        BaseListResponse<DocumentDocumentField> GetAllDocumentDocumentField();
        BaseListResponse<SPGetDocumentDocumentField_Result> FilterDocumentDocumentField(DocumentDocumentFieldQuery query);
        BaseResponse<DocumentDocumentField> AddDocumentDocumentField(DocumentDocumentField model);
        BaseListResponse<DocumentDocumentField> AddDocumentDocumentFields(List<DocumentDocumentField> models);
        BaseResponse<DocumentDocumentField> UpdateDocumentDocumentField(DocumentDocumentField model);
        BaseResponse DeleteLogicalDocumentDocumentField(int id);
        BaseResponse DeleteDocumentDocumentFields(List<DocumentDocumentField> models);
        BaseResponse DeleteDocDocumentFieldsByDocIdAndReceivedDoc(CustomDocumentRecipent model);
        #endregion DocumentField

        #region DocumentRecipent
        BaseResponse<DocumentRecipent> AddDocumentRecipent(DocumentRecipent model);
        BaseResponse<DocumentRecipent> UpdateDocumentRecipentByDocIdAndReceivedDoc(DocumentRecipent model);
        BaseListResponse<DocumentRecipent> AddDocumentRecipents(List<DocumentRecipent> models);
        bool CheckUserDocumentReadable(DocumentCheckReadableQuery query);
        BaseResponse DeleteDocumentRecipents(List<DocumentRecipent> models);
        BaseListResponse<SPGetDocumentRecipentByDocIdAndRecivedDoc_Result> GetDocRecipentsByDocIdAndReceived(int documentId, bool receivedDocument);
        BaseResponse<string> DownloadDocumentBook(DocumentReceivedQuery query);
        BaseResponse DeleteRecipentsByDocIdAndReceivedDoc(CustomDocumentRecipent model);
        #endregion

        #region AddedDocumentBook
        BaseListResponse<SPGetAddedDocumentBook_Result> FilterAddedDocumentBook(DocumentReceivedQuery query);
        BaseResponse<string> DownloadTotalDocument(DateTime fromDate, DateTime toDate, string listDepartmentId, int departmentId);
        BaseResponse<bool> CheckHistoryAddedBookDoc(int documentId, bool receivedDocument);

        #endregion

        #region HistoryDocument
        BaseListResponse<SPGetHistoryDocument_Result> GetHistoryDocument(int documentId, bool receivedDocument);
        #endregion

        #region HistoryAddedBook
        BaseListResponse<SPGetHistoryAddedBookDocument_Result> GetHistoryAddedDocument(int documentId, bool receivedDocument);
        #endregion
        BaseResponse<bool> CheckPermissionUserDocument(string userId, int documentId, bool receivedDocument, string listDepartment);
    }
}
