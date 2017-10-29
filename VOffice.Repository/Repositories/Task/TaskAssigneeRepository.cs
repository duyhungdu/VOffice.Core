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
    public partial class TaskAssigneeRepository : BaseRepository<TaskAssignee>
    {
        DepartmentRepository _departmentRepository;
        public TaskAssigneeRepository()
        {
            _departmentRepository = new DepartmentRepository();
        }
        public List<SPGetTaskAssigneeByTaskId_Result> GetTaskAssigneeByTaskId(int taskId)
        {
            List<SPGetTaskAssigneeByTaskId_Result> result = new List<SPGetTaskAssigneeByTaskId_Result>();
            result = _entities.SPGetTaskAssigneeByTaskId(taskId).ToList();
            return result;
        }
        public List<SPGetTaskAssigneeByTaskId_Result> GetTaskAssigneeViewDetailByTaskId(int taskId)
        {
            List<SPGetTaskAssigneeByTaskId_Result> result = new List<SPGetTaskAssigneeByTaskId_Result>();
            List<SPGetTaskAssigneeByTaskId_Result> listTaskAssignee = _entities.SPGetTaskAssigneeByTaskId(taskId).ToList();
            foreach (var item in listTaskAssignee)
            {
                if (item.UserId != item.CreatedBy)
                {
                    bool exist = false;
                    foreach (var model in result)
                    {
                        if (model.UserId == item.UserId)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        result.Add(item);
                    }
                }
            }
            return result.OrderByDescending(x => x.ViewOn).ToList();
        }
        public List<SPGetTaskDocumentHistory_Result> GetTaskAssigneeDocumentHistory(int taskId, int documentId, string documentReceived)
        {
            List<SPGetTaskDocumentHistory_Result> result = new List<SPGetTaskDocumentHistory_Result>();

            List<SPGetTaskDocumentHistory_Result> listAssignee = _entities.SPGetTaskDocumentHistory(taskId, documentId, documentReceived).ToList();
            foreach (var item in listAssignee)
            {
                bool exist = false;
                foreach (var r in result)
                {
                    if (r.UserId == item.UserId)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    result.Add(item);
                }
            }
            string userId = "";
            int count = 0;
            foreach (var item in listAssignee)
            {
                if (item.UserId == item.CreatedBy)
                {
                    count++;
                }
                userId = item.CreatedBy;
            }
            if (count == 0)
            {
                bool checkReceived = documentReceived == "1" ? true : false;
                DocumentHistory history = _entities.DocumentHistories.FirstOrDefault(n => n.UserId == userId && n.DocumentId == documentId && n.ReceivedDocument == checkReceived);
                UserDepartment userDepartment = _departmentRepository.GetUserMainOrganization(userId);
                SPGetTaskDocumentHistory_Result createdBy = new SPGetTaskDocumentHistory_Result();
               
                if (history != null)
                {
                    createdBy.HistoryId = history.Id;
                    createdBy.ViewDetail = true;
                    //createdBy.ViewOn = history.AttempOn;

                    createdBy.AttempOn = history.AttempOn;
                    createdBy.Coprocessor = false;
                    createdBy.CreatedBy = userId;
                    createdBy.CreatedOn = DateTime.Now;
                    createdBy.Avatar = userDepartment.Avatar;
                    createdBy.FullName = userDepartment.FullName;
                    createdBy.Name = userDepartment.FirstName;
                    createdBy.Order = 1;
                    createdBy.Position = userDepartment.Position;
                    createdBy.Assignee = false;
                    createdBy.Supervisor = false;
                    createdBy.TaskId = taskId;
                    createdBy.UserId = userId;
                }
                else
                {
                    createdBy.HistoryId = null;
                    createdBy.ViewDetail = false;
                    // createdBy.ViewOn = null;

                    createdBy.AttempOn = null;
                    createdBy.Coprocessor = false;
                    createdBy.CreatedBy = userId;
                    createdBy.CreatedOn = DateTime.Now;
                    createdBy.Avatar = userDepartment.Avatar;
                    createdBy.FullName = userDepartment.FullName;
                    createdBy.Name = userDepartment.FirstName;
                    createdBy.Order = 1;
                    createdBy.Position = userDepartment.Position;
                    createdBy.Assignee = false;
                    createdBy.Supervisor = false;
                    createdBy.TaskId = taskId;
                    createdBy.UserId = userId;
                }
                result.Add(createdBy);
            }
            return result;
        }
    }
}
