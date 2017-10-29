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
    public partial class TaskRepository : BaseRepository<VOffice.Model.Task>
    {
        SystemConfigDepartmentRepository _systemConfigDepartmentRepository;
        TaskAssigneeRepository _taskAssigneeRepository;
        StatusRepository _statusRepository;
        public TaskRepository()
        {
            _systemConfigDepartmentRepository = new SystemConfigDepartmentRepository();
            _taskAssigneeRepository = new TaskAssigneeRepository();
            _statusRepository = new StatusRepository();
        }
        public List<SPGetTask_Result> Filter(TaskQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            string userId = query.UserId;
            DateTime? startDate = query.FromDate;
            DateTime? endDate = query.ToDate;
            int? statusId = query.StatusId;
            bool? assignToMe = query.AssignToMe;
            bool? relativeToMe = query.RelativeToMe;
            List<SPGetTask_Result> result = new List<SPGetTask_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            bool? documentReceived = null;
            if (query.DocumentReceived.HasValue)
            {
                documentReceived = query.DocumentReceived.Value != 1 ? false : true;
            }
            if (query.StatusId != 0)
            {
                Status status = _statusRepository.GetById(query.StatusId);
                if (status.Code == "HH")
                {
                    result = _entities.SPGetTask(userId, 0, startDate, endDate, Util.DetecVowel(keyword), query.DocumentId, documentReceived, assignToMe, relativeToMe, start, limit, prTotalRow).Where(n => n.Expired == true).ToList();
                }
                else
                {
                    result = _entities.SPGetTask(userId, statusId, startDate, endDate, Util.DetecVowel(keyword), query.DocumentId, documentReceived, assignToMe, relativeToMe, start, limit, prTotalRow).ToList();
                }
            }
            else
            {
                result = _entities.SPGetTask(userId, statusId, startDate, endDate, Util.DetecVowel(keyword), query.DocumentId, documentReceived, assignToMe, relativeToMe, start, limit, prTotalRow).ToList();
            }
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public string GenerateTaskCode(int departmentId)
        {
            #region Generate TaskCode
            string taskCode = "";
            string taskCodeText = "";
            string taskCodeNumber = "";
            try
            {
                var systemConfigDepartmentQuery = new SystemConfigDepartmentQuery();
                systemConfigDepartmentQuery.DepartmentId = departmentId;
                systemConfigDepartmentQuery.DefaultValue = "TASK";
                systemConfigDepartmentQuery.Title = "TASKCODE";
                taskCodeText = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(systemConfigDepartmentQuery);
                systemConfigDepartmentQuery.DefaultValue = "0";
                systemConfigDepartmentQuery.Title = "TASKNUMBER";
                string taskCodeNumberConfig = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(systemConfigDepartmentQuery);
                taskCodeNumber = (int.Parse(taskCodeNumberConfig) + 1).ToString("D4");
                taskCode = taskCodeText + taskCodeNumber;
            }
            catch
            {

            }
            return taskCode;
            #endregion Generate TaskCode
        }
        public SPGetTaskDetailById_Result SPGetTaskDetailById(int taskId)
        {
            SPGetTaskDetailById_Result result = new SPGetTaskDetailById_Result();
            result = _entities.SPGetTaskDetailById(taskId).FirstOrDefault();
            return result;
        }
        public List<SPGetTaskByDocumentId_Result> SPGetTaskByDocumentId(int docId, bool receivedDoc, string userId, out int count)
        {
            List<SPGetTaskByDocumentId_Result> result = new List<SPGetTaskByDocumentId_Result>();
            int totalRow = 0;
            count = 0;
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetTaskByDocumentId(docId, receivedDoc, userId, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public int CountDefaultTask(TaskQuery query)
        {
            int count = 0;
            int totalRow = 0;
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            var temp = _entities.CountNewTask(query.UserId, query.StatusId, DateTime.Now.AddMonths(-3), DateTime.Now, "", 0, int.MaxValue, prTotalRow);
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return count;
        }
        public int CountTaskInStatus(TaskQuery query)
        {
            int count = 0;
            int totalRow = 0;
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            var temp = _entities.CountTaskInStatus(query.UserId, query.Type, query.StatusId, query.FromDate, query.ToDate, "", 0, int.MaxValue, prTotalRow);
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return count;
        }

        public int CountExpiredTask(TaskQuery query)
        {
            int count = 0;
            int totalRow = 0;
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            var temp = _entities.CountExpiredTask(query.UserId, query.Type, query.FromDate, query.ToDate, prTotalRow);
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return count;
        }
        public List<SPGetTaskAdvance_Result> FilterAdvance(TaskQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            string userId = query.UserId;
            DateTime? fromDate = query.FromDate;
            DateTime? toDate = query.ToDate;
            DateTime? startFromDate = query.StartFromDate;
            DateTime? startToDate = query.StartToDate;
            string status = string.IsNullOrEmpty(query.Status) == true ? "" : query.Status;
            DateTime? dueFromDate = query.DueFromDate;
            DateTime? dueToDate = query.DueToDate;
            string taskType = string.IsNullOrEmpty(query.TaskType) == true ? "" : query.TaskType;
            string project = string.IsNullOrEmpty(query.Project) == true ? "" : query.Project;
            string assignee = string.IsNullOrEmpty(query.Assignee) == true ? "" : query.Assignee;
            string coprocessor = string.IsNullOrEmpty(query.Coprocessor) == true ? "" : query.Coprocessor;
            string supervisor = string.IsNullOrEmpty(query.Supervisor) == true ? "" : query.Supervisor;
            string customer = string.IsNullOrEmpty(query.Customer) ? "" : query.Customer;
            int departmentId = query.DepartmentId;
            bool taskAssignee = query.TaskAssignee;
            string keywordDoc = string.IsNullOrEmpty(query.KeywordDoc) == true ? "" : query.KeywordDoc;
            List<SPGetTaskAdvance_Result> result = new List<SPGetTaskAdvance_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetTaskAdvance(departmentId, userId, status, keyword, fromDate, toDate, startFromDate, startToDate, dueFromDate, dueToDate, taskType, project, assignee, coprocessor, supervisor, customer, taskAssignee, keywordDoc, start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public List<SPGetTaskReport_Result> GetTaskReport(TaskQuery query)
        {
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            string userId = query.UserId;
            DateTime? fromDate = query.FromDate;
            DateTime? toDate = query.ToDate;
            DateTime? startFromDate = query.StartFromDate;
            DateTime? startToDate = query.StartToDate;
            string status = string.IsNullOrEmpty(query.Status) == true ? "" : query.Status;
            DateTime? dueFromDate = query.DueFromDate;
            DateTime? dueToDate = query.DueToDate;
            string taskType = string.IsNullOrEmpty(query.TaskType) == true ? "" : query.TaskType;
            string project = string.IsNullOrEmpty(query.Project) == true ? "" : query.Project;
            string assignee = string.IsNullOrEmpty(query.Assignee) == true ? "" : query.Assignee;
            string coprocessor = string.IsNullOrEmpty(query.Coprocessor) == true ? "" : query.Coprocessor;
            string supervisor = string.IsNullOrEmpty(query.Supervisor) == true ? "" : query.Supervisor;
            string customer = string.IsNullOrEmpty(query.Customer) ? "" : query.Customer;
            int departmentId = query.DepartmentId;
            bool taskAssignee = query.TaskAssignee;
            string keywordDoc = string.IsNullOrEmpty(query.KeywordDoc) == true ? "" : query.KeywordDoc;
            List<SPGetTaskReport_Result> result = new List<SPGetTaskReport_Result>();
            List<SPGetTaskAssigneeByTaskId_Result> lstAssign = new List<SPGetTaskAssigneeByTaskId_Result>();
            result = _entities.SPGetTaskReport(departmentId, userId, status, keyword, fromDate, toDate, startFromDate, startToDate, dueFromDate, dueToDate, taskType, project, assignee, coprocessor, supervisor, customer, taskAssignee, keywordDoc).ToList();
            if (result.Count() > 0)
            {
                foreach (var item in result)
                {
                    string processors = "";
                    lstAssign = _taskAssigneeRepository.GetTaskAssigneeByTaskId(item.Id);
                    for (int i = 0; i < lstAssign.Count; i++)
                    {
                        if (i == 0)
                        {
                            processors += lstAssign[i].FullName;
                        }
                        else
                        if (i == 1)
                        {
                            processors += " (" + lstAssign[i].FullName + (lstAssign.Count - 1 == 1 ? ")" : "");
                        }
                        else if (i > 1 && i < lstAssign.Count - 1)
                        {
                            processors += ", " + lstAssign[i].FullName;
                        }
                        else
                            if (i == lstAssign.Count - 1)
                        {
                            processors += " , " + lstAssign[i].FullName + ")";
                        }
                    }
                    item.Processor = processors;
                }
            }
            return result;
        }
        public List<SPGetTaskMobile_Result> FilterMobile(TaskQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            string userId = query.UserId;
            bool? assignee = query.AssignToMe;
            bool? supervisor = query.RelativeToMe;
            bool? coprocessor = query.CoprocessorToMe;
            bool? expired = query.Experied;
            string listStatusId = query.Status;
            if(!string.IsNullOrEmpty(query.Status))
            {
                string[] arr = query.Status.Split(',');
                int countE = 0;
                foreach (var item in arr)
                {
                    if (string.IsNullOrEmpty(item) == false)
                    {
                        countE += 1;
                    }
                }
                if (countE == 1)
                {
                    foreach (var item in arr)
                    {
                        if (string.IsNullOrEmpty(item) == false)
                        {
                            if (_statusRepository.GetById(int.Parse(item)).Code == "HH")
                                expired = true;
                            else expired = false;
                        }
                    }
                }
                else expired = false;
            }
            List<SPGetTaskMobile_Result> result = new List<SPGetTaskMobile_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetTaskMobile(userId, listStatusId, expired, assignee, coprocessor, supervisor, start, limit, prTotalRow).ToList();
            foreach (var item in result)
            {
                var staff = _taskAssigneeRepository.GetTaskAssigneeByTaskId(item.Id.Value).FirstOrDefault(n => n.Assignee == true);
                var supers = _taskAssigneeRepository.GetTaskAssigneeByTaskId(item.Id.Value).Where(n => n.Supervisor == true);
                item.Avatar = staff.Avatar;
                if (supers.Count() > 0)
                {
                    foreach (var st in supers)
                    {
                        item.SupervisorId += st.UserId + ',';
                    }
                }
            }
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public bool CheckPermissionUserTask(int taskId, string userId)
        {
            var countTask = _entities.SPCheckPermissionUserTask(userId, taskId).FirstOrDefault();
            if(countTask!=null)
            {
                if(countTask.Value>0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
