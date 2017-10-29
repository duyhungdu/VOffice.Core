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
    public partial class TaskAttachmentRepository : BaseRepository<TaskAttachment>
    {
        public TaskAttachmentRepository()
        {

        }
        public List<SPGetTaskAttachmentByRecordId_Result> GetTaskAttachmenByRecordId(int recordId, string type)
        {
            List<SPGetTaskAttachmentByRecordId_Result> result = new List<SPGetTaskAttachmentByRecordId_Result>();
            result = _entities.SPGetTaskAttachmentByRecordId(type, recordId).ToList();
            return result;
        }
    }
}
