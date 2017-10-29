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
    public partial class TaskActivityRepository : BaseRepository<TaskActivity>
    {
        public TaskActivityRepository()
        {

        }
        public List<SPGetTaskActivityByTaskId_Result> GetTaskActivityByTaskId(int taskId)
        {
            List<SPGetTaskActivityByTaskId_Result> result = new List<SPGetTaskActivityByTaskId_Result>();
            result = _entities.SPGetTaskActivityByTaskId(taskId).ToList();
            return result;
        }
        public List<TaskOpinionComplex> GetTaskOpinionAndActivityByTaskId(int taskId)
        {
            List<SPGetTaskOpinionAndActivityByTaskId_Result> lstParents = new List<SPGetTaskOpinionAndActivityByTaskId_Result>();
            List<TaskOpinionComplex> result = new List<TaskOpinionComplex>();
            List<SPGetSubTaskOpinionByTaskOpinionId_Result> lstSub = new List<SPGetSubTaskOpinionByTaskOpinionId_Result>();
            TaskOpinionComplex child;
            lstParents = _entities.SPGetTaskOpinionAndActivityByTaskId(taskId).ToList();
            foreach (var item in lstParents)
            {
                child = new TaskOpinionComplex();
                lstSub = _entities.SPGetSubTaskOpinionByTaskOpinionId(item.Id).ToList();
                child.Id = item.Id;
                child.Model = item.Model;
                child.NewValue = item.NewValue;
                child.OpiniOnContent = item.OpiniOnContent;
                child.OpinionFileName = item.OpinionFileName;
                child.OpinionFilePath = item.OpinionFilePath;
                child.ActivityContent = item.ActivityContent;
                child.Avatar = item.Avatar;
                child.CreatedBy = item.CreatedBy;
                child.CreatedOn = item.CreatedOn;
                child.DESCRIPTION = item.DESCRIPTION;
                child.EditedBy = item.EditedBy;
                child.EditedOn = item.EditedOn;
                child.FullName = item.FullName;
                child.SubComent = lstSub;
                result.Add(child);
            }
            return result;
        }
        public List<SPGetTaskOpinionAndActivityByUserId_Result> GetTaskOpinionAndActivityByUserId(string userId, int count)
        {
            List<SPGetTaskOpinionAndActivityByUserId_Result> result = new List<SPGetTaskOpinionAndActivityByUserId_Result>();
            result = _entities.SPGetTaskOpinionAndActivityByUserId(userId).Take(count).ToList();
            return result;
        }
    }
}
