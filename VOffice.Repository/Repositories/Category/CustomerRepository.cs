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
    public partial class CustomerRepository : BaseRepository<Customer>
    {
        public CustomerRepository()
        {

        }
        public List<SPGetCustomer_Result> Filter(CustomerQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetCustomer_Result> result = new List<SPGetCustomer_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetCustomer(query.DepartmentId,Util.DetecVowel(keyword), start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public List<SPGetCustomerByDepartmentId_Result> FilterByDepartmentId(int departmentId, string keyword)
        {
            List<SPGetCustomerByDepartmentId_Result> result = new List<SPGetCustomerByDepartmentId_Result>();
            result = _entities.SPGetCustomerByDepartmentId(departmentId, Util.DetecVowel(keyword)).ToList();
            return result;
        }
    }

}
