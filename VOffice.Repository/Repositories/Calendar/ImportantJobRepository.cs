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
    public partial class ImportantJobRepository : BaseRepository<ImportantJob>
    {
        public ImportantJobRepository()
        {

        }
        public SPGetImportantJob_Result GetImportantJob(int departmentId, DateTime startDate,DateTime endDate, bool note)
        {
            return _entities.SPGetImportantJob(departmentId, startDate, endDate, note).FirstOrDefault();
        }
    }
}
