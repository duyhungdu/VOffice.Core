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
using Newtonsoft.Json;

namespace VOffice.Repository
{
    public partial class ApplicationLoggingRepository : BaseRepository<ApplicationLogging>
    {
        public ApplicationLoggingRepository()
        {
        }
        public int Log(string type, string action, string modelName, string modelId, string modelProperty, object oldValue, object newValue, string message, string ip, string userId)
        {
            int result = 0;
            ApplicationLogging appLog = new ApplicationLogging();
            appLog.Type = type;
            appLog.Action = action;
            appLog.ModelName = modelName;
            appLog.ModelId = modelId;
            appLog.ModelProperty = modelProperty;
            if (oldValue != null)
            {
                string oldVal = JsonConvert.SerializeObject(oldValue);
                if (string.IsNullOrEmpty(oldVal))
                {
                    appLog.OldValue = oldVal;
                }
            }
            if (newValue != null)
                appLog.NewValue = JsonConvert.SerializeObject(newValue);
            appLog.Message = message;
            appLog.UserHostAddress = ip;
            appLog.CreatedBy = userId;
            appLog.CreatedOn = DateTime.Now;
            try
            {
                result = Add(appLog).Id;
            }
            catch
            {
                result = 0;
            }
            return result;
        }
        public List<SPGetApplicationLogging_Result> Filter(ApplicationLoggingQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var start = 0;
            var limit = query.PageSize;
            DateTime fromDate = query.FromDate;
            DateTime toDate = query.ToDate;
            var modules = !string.IsNullOrEmpty(query.Module)?query.Module:"";
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetApplicationLogging_Result> result = new List<SPGetApplicationLogging_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetApplicationLogging(fromDate, toDate, modules, Util.DetecVowel(keyword), start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
    }
}
