using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using VOffice.Model;
using VOffice.Repository.Infrastructure;
using VOffice.Repository.Queries;

namespace VOffice.Repository
{
    public partial class DocumentFieldRepository : BaseRepository<DocumentField>
    {
        public DocumentFieldRepository()
        {
        }
        public List<SPGetDocumentField_Result> Filter(DocumentFieldQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetDocumentField_Result> result = new List<SPGetDocumentField_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetDocumentField(Util.DetecVowel(keyword), start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public List<SPCopyDocumentField_Result> GetDocumentFieldaNotInDepartment(int departmentId)
        {
            List<SPCopyDocumentField_Result> result = new List<SPCopyDocumentField_Result>();
            result = _entities.SPCopyDocumentField(departmentId).ToList();
            return result;
        }
    }
}