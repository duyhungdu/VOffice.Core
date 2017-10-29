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
    public partial class DocumentHistoryRepository : BaseRepository<DocumentHistory>
    {
        public DocumentHistoryRepository()
        {
        }
        public List<SPGetHistoryDocument_Result> GetHistoryDocument(int documentId, bool receivedDocument, int departmentId, string userId)
        {
            List<SPGetHistoryDocument_Result> result = new List<SPGetHistoryDocument_Result>();
            result = _entities.SPGetHistoryDocument(documentId, receivedDocument, departmentId, userId).ToList();
            return result;
        }
    }
}
