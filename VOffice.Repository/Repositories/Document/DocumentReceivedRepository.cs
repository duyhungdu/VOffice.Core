using VOffice.Model;
using VOffice.Repository.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Repository.Infrastructure;
using VOffice.Core.Messages;

namespace VOffice.Repository
{
    public partial class DocumentReceivedRepository : BaseRepository<DocumentReceived>
    {
        DepartmentRepository _departmentRepository;
        #region DocumentReceived
        public DocumentReceivedRepository()
        {
            _departmentRepository = new DepartmentRepository();
        }
        public List<SPGetDocument_Result> Filter(DocumentReceivedQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetDocument_Result> result = new List<SPGetDocument_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetDocument(query.Type, Util.DetecVowel(keyword), query.StartDate, query.EndDate, query.UserId, query.ListSubDepartmentId, query.DepartmentId, start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public int CountNewDocument(DocumentReceivedQuery query)
        {
            int count = 0;
            int totalRow = 0;
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            var temp = _entities.SPCountNewDocument("0", "", DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(3), query.UserId, query.ListSubDepartmentId, query.DepartmentId, 0, int.MaxValue, prTotalRow);
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return count;
        }
        public int CountUserDocument(DocumentReceivedQuery query)
        {
            int count = 0;
            int totalRow = 0;
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            var temp = _entities.SPCountUserDocument(query.Type, query.StartDate, query.EndDate, query.UserId, query.ListSubDepartmentId, query.DepartmentId, prTotalRow);
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return count;
        }
        public int CountUserDocumentHaventRead(DocumentReceivedQuery query)
        {
            int count = 0;
            int totalRow = 0;
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            var temp = _entities.SPCountUserDocumentHaventRead(query.Type, query.StartDate, query.EndDate, query.UserId, query.ListSubDepartmentId, query.DepartmentId, prTotalRow);
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return count;
        }
        public List<SPSearchListDocument_Result> SearchListDocument(DocumentReceivedQuery query)
        {
            List<SPSearchListDocument_Result> result = new List<SPSearchListDocument_Result>();
            result = _entities.SPSearchListDocument("", query.StartDate, query.EndDate, query.UserId, query.ListSubDepartmentId, query.DepartmentId).ToList();
            return result;
        }
        public List<SPGetDocumentReceivedByDocumentDeliveredId_Result> GetDocumentReceivedByDocumentDeliveredId(int documentDeliveredId, bool receivedDocument)
        {
            List<SPGetDocumentReceivedByDocumentDeliveredId_Result> result = new List<SPGetDocumentReceivedByDocumentDeliveredId_Result>();
            result = _entities.SPGetDocumentReceivedByDocumentDeliveredId(documentDeliveredId).ToList();
            return result;
        }
        public bool CheckUserDocumentReadable(string userId, int documentId, bool receivedDocument)
        {
            var userDepartment = new UserDepartment();
            userDepartment = _departmentRepository.GetUserMainOrganization(userId);
            if (userDepartment != null)
            {
                string listSubDepartmentId = userDepartment.ListSubDepartmentId;
                int departmentId = userDepartment.DepartmentId;
                List<SPCheckUserDocumentReadable_Result> listCheckUserDocumentReadable = new List<SPCheckUserDocumentReadable_Result>();
                listCheckUserDocumentReadable = _entities.SPCheckUserDocumentReadable(userId, documentId, receivedDocument, departmentId, listSubDepartmentId).ToList();
                if (listCheckUserDocumentReadable.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool CheckReceivedNumber(int departmentId, string receivedNumber)
        {
            if (_entities.SPCheckReceivedNumber(departmentId, receivedNumber).FirstOrDefault().Value > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public SPGetDetailDocument_Result GetDetailDocumentReceived(string type, int documentId)
        {
            SPGetDetailDocument_Result result = new SPGetDetailDocument_Result();
            result = _entities.SPGetDetailDocument(type, documentId).FirstOrDefault();
            return result;
        }
        #endregion
        #region AddedDocumentBook
        public List<SPGetAddedDocumentBook_Result> FilterAddedDocumentBook(DocumentReceivedQuery query, out int count)
        {
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetAddedDocumentBook_Result> result = new List<SPGetAddedDocumentBook_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetAddedDocumentBook(query.DepartmentId, Util.DetecVowel(keyword), query.StartDate, query.EndDate, start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public List<SPGetHistoryAddedBookDocument_Result> GetHistoryAddedBookDocument(int documentId, bool receivedDocument)
        {
            List<SPGetHistoryAddedBookDocument_Result> result = new List<SPGetHistoryAddedBookDocument_Result>();
            result = _entities.SPGetHistoryAddedBookDocument(documentId, receivedDocument).ToList();
            return result;
        }
        public bool CheckHistoryAddedBookDoc(int documentId, bool receivedDocument)
        {
            List<SPGetHistoryAddedBookDocument_Result> result = _entities.SPGetHistoryAddedBookDocument(documentId, receivedDocument).ToList();
            if (result.Count() > 0)
                return true;
            else
                return false;
        }
        public SPGetComplexCount_Result GetComplexCount(string userId, int departmentId)
        {
            SPGetComplexCount_Result result = new SPGetComplexCount_Result();
            result = _entities.SPGetComplexCount(userId, departmentId).ToList().FirstOrDefault();
            return result;
        }
        public List<SPGetDocumentAdvance_Result> SearchDocument(DocumentReceivedQuery query, out int count, int pageSize = 10)
        {
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var start = 0;
            var limit = pageSize == 10 ? query.PageSize : pageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetDocumentAdvance_Result> result = new List<SPGetDocumentAdvance_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("Total", totalRow);
            result = _entities.SPGetDocumentAdvance(
                query.DocumentReceived,
                query.DocumentDelivered,
                query.LegalDocument,
                Util.DetecVowel(keyword),
                query.DocumentDateStart,
                query.DocumentDateEnd,
                query.DocumentDateRDStart,
                query.DocumentDateRDEnd,
                query.DocumentSigns,
                query.DocumentSignsDelivered,
                query.DocumentFields,
                query.DocumentTypes,
                query.DocumentSecretLevels,
                query.DocumentUrgencyLevels,
                query.DepartmentId,
                query.UserId,
                query.ListSubDepartmentId,
                start,
                limit,
                prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public List<SPGetTotalDocumentReport_Result> DownloadTotalDocument(DateTime fromDate, DateTime toDate, string listDepartmentId)
        {
            return _entities.SPGetTotalDocumentReport(fromDate, toDate, listDepartmentId).ToList();
        }
        public List<SPGetTotalDocumentReportList_Result> DownloadTotalDocumentList(DocumentReceivedQuery query, out int count)
        {
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetTotalDocumentReportList_Result> result = new List<SPGetTotalDocumentReportList_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("Total", totalRow);
            result = _entities.SPGetTotalDocumentReportList(
                query.StartDate,
                query.EndDate,
                query.ListSubDepartmentId,
                start,
                limit,
                prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        #endregion

        public bool CheckPermissionUserDocument(string userId, int documentID, bool receivedDocument, string listDepartmentId)
        {
            bool IsApprove = false;
            var check = _entities.SPCheckPermissionUserDocument(userId, documentID, receivedDocument, listDepartmentId).FirstOrDefault();
            if (check != null)
            {
                IsApprove = check.Value;
            }
            return IsApprove;
        }
    }
}
