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
    public partial class NoticeRepository : BaseRepository<Notice>
    {
        public List<SPGetNotice_Result> Filter(NoticeQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var departmentId = query.DepartmentId;
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetNotice_Result> result = new List<SPGetNotice_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetNotice(departmentId,Util.DetecVowel(keyword), start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
    }
}
