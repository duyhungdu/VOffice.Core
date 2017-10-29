using VOffice.Model;
using VOffice.Repository.Queries;
using System;
using System.Collections.Generic;
using System.Data;

using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Repository.Infrastructure;
using System.Data.Entity.Core.Objects.DataClasses;

namespace VOffice.Repository
{
    public partial class NotificationCenterRepository : BaseRepository<NotificationCenter>
    {
        public NotificationCenterRepository()
        {

        }
        public List<SPGetNotificationCenter_Result> Filter(NotificationCenterQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var userId = query.UserId;
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetNotificationCenter_Result> result = new List<SPGetNotificationCenter_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetNotificationCenter(userId, query.DeviceId, start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        
        public int GetCountNotificationUnread(string userId, string deveiceId)
        {
            return _entities.SPGetCountNotificationUnread(userId, deveiceId).FirstOrDefault().Value;
        }
        public NotificationCenter ConvertFromCustomNotificationToOrigin(FCMNotificationCenter fcm)
        {
            NotificationCenter result = new NotificationCenter
            {
                GroupId = fcm.GroupId,
                DeviceId = fcm.DeviceId,
                Content = fcm.Content,
                CreatedBy = fcm.CreatedBy, 
                CreatedOn = fcm.CreatedOn,
                HaveSeen = fcm.HaveSeen,
                ReceivedUserId = fcm.ReceivedUserId,
                RecordId = fcm.RecordId,
                RecordNumber = fcm.RecordNumber,
                RelateRecordId= fcm.RelateRecordId,
                SubRelateRecordId = fcm.SubRelateRecordId,
                Title = fcm.Title,
                Type = fcm.Type,
                Id = fcm.Id                
            };
            return result;
        }
    }
}
