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
    public partial class TaskDocumentsRepository : BaseRepository<TaskDocument>
    {
        public TaskDocumentsRepository()
        {

        }
        public List<SPGetTaskDocumentByTaskId_Result> GetTaskDocumentByTaskId(int taskId)
        {
            List<SPGetTaskDocumentByTaskId_Result> result = new List<SPGetTaskDocumentByTaskId_Result>();
            result = _entities.SPGetTaskDocumentByTaskId(taskId).ToList();
            return result;
        }
    }
}
