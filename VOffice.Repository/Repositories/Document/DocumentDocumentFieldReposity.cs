using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using VOffice.Repository.Infrastructure;
using VOffice.Model;
using VOffice.Repository.Queries;

namespace VOffice.Repository
{
    public partial class DocumentDocumentFieldReposity : BaseRepository<DocumentDocumentField>
    {
        public DocumentDocumentFieldReposity()
        {

        }
        public List<SPGetDocumentDocumentField_Result> Filter(DocumentDocumentFieldQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var documentID = query.DocumentID;
            var documentFieldDepartmnetID = query.DocumentFieldDepartmentId;
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetDocumentDocumentField_Result> result = new List<SPGetDocumentDocumentField_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetDocumentDocumentField(documentID, documentFieldDepartmnetID, Util.DetecVowel(keyword), start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
    }
}
