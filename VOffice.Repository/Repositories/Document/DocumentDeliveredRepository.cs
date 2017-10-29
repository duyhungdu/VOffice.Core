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

namespace VOffice.Repository
{
    public partial class DocumentDeliveredRepository : BaseRepository<DocumentDelivered>
    {
        public DocumentDeliveredRepository()
        {
        }
        public List<SPGetDocumentDeliveredByDocumentReceivedId_Result> GetDocumentDeliveredByDocumentReceivedId(int documentReceivedId, bool receivedDocument)
        {
            List<SPGetDocumentDeliveredByDocumentReceivedId_Result> result = new List<SPGetDocumentDeliveredByDocumentReceivedId_Result>();
            result = _entities.SPGetDocumentDeliveredByDocumentReceivedId(documentReceivedId).ToList();
            return result;
        }
        public List<SPGetDocumentDeliveredStatistics_Result> GetDocumentDeliveredStatisticsList(DocumentDeliveredStatisticsQuery query, out int count)
        {
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetDocumentDeliveredStatistics_Result> result = new List<SPGetDocumentDeliveredStatistics_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("Total", totalRow);
            result = _entities.SPGetDocumentDeliveredStatistics(query.Keyword, 
                query.StartDate, 
                query.EndDate, 
                query.DepartmentId, 
                query.ListSignById, 
                query.ListDocumentFieldId, 
                query.ListDocumentTypeId, 
                start, 
                limit,
                prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public List<SPGetDocumentDeliveredStatistics_Result> GetDocumentDeliveredStatisticsDownload(DocumentDeliveredStatisticsQuery query, out int count)
        {
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var userId= string.IsNullOrEmpty(query.UserId) != true ? query.UserId : "";
            var start = 0;
            var limit = 1000;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetDocumentDeliveredStatistics_Result> result = new List<SPGetDocumentDeliveredStatistics_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("Total", totalRow);
            result = _entities.SPGetDocumentDeliveredStatistics(query.Keyword,
                query.StartDate,
                query.EndDate,
                query.DepartmentId,
                query.ListSignById,
                query.ListDocumentFieldId,
                query.ListDocumentTypeId,
                start,
                limit,
                prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
    }
}
