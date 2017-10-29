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
    public partial class TaskOpinionRepository : BaseRepository<TaskOpinion>
    {
        public TaskOpinionRepository()
        {
        }
        public List<SPGetTaskOpinionByTaskId_Result> GetTaskOpnionByTaskId(int taskId)
        {
            return _entities.SPGetTaskOpinionByTaskId(taskId).ToList();
        }
        public TaskOpinionComplex GetTaskOpinionByOpinionId(int id)
        {
            TaskOpinionComplex result = new TaskOpinionComplex();
            SPGetTaskOpinionAndActivityByOpinionId_Result item = new SPGetTaskOpinionAndActivityByOpinionId_Result();
            item = _entities.SPGetTaskOpinionAndActivityByOpinionId(id).FirstOrDefault();
            List<SPGetSubTaskOpinionByTaskOpinionId_Result> lstSub = new List<SPGetSubTaskOpinionByTaskOpinionId_Result>();
            result = new TaskOpinionComplex();
            if (item != null)
            {
                lstSub = _entities.SPGetSubTaskOpinionByTaskOpinionId(item.Id).ToList();
            }
            else
                lstSub = null;
            result.Id = item.Id;
            result.Model = item.Model;
            result.NewValue = item.NewValue;
            result.OpiniOnContent = item.OpiniOnContent;
            result.OpinionFileName = item.OpinionFileName;
            result.OpinionFilePath = item.OpinionFilePath;
            result.ActivityContent = item.ActivityContent;
            result.Avatar = item.Avatar;
            result.CreatedBy = item.CreatedBy;
            result.CreatedOn = item.CreatedOn;
            result.DESCRIPTION = item.DESCRIPTION;
            result.EditedBy = item.EditedBy;
            result.EditedOn = item.EditedOn;
            result.FullName = item.FullName;
            result.SubComent = lstSub;
            return result;
        }
    }
}
