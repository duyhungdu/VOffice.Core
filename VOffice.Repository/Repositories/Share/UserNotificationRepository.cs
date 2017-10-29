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
    public partial class UserNotificationRepository : BaseRepository<UserNotification>
    {
        public UserNotificationRepository()
        {

        }

        public List<SPGetUserNotification_Result> GetUserNotificationByUserId(string userId)
        {
            return _entities.SPGetUserNotification(userId).ToList();            
        }
        public List<SPGetUserNotificationForDocumentAndEvent_Result> GetUserNotificationByDepartmentId(int deparmentId)
        {
            return _entities.SPGetUserNotificationForDocumentAndEvent(deparmentId).ToList();
        }

        public List<SPGetUserNotificationForDocumentAndEvent_Result> ConvertToNotificationDocumentAndEvent(List<SPGetUserNotification_Result> listOject)
        {
            var listReturn = new List<SPGetUserNotificationForDocumentAndEvent_Result>();
            foreach (var item in listOject)
            {
                var newObj = new SPGetUserNotificationForDocumentAndEvent_Result
                {
                    AttempOn = item.AttempOn,
                    Avatar = item.Avatar,
                    ClientId = item.ClientId,
                    FullName = item.FullName,
                    Id = item.Id,
                    Type = item.Type,
                    UserId = item.UserId
                };
                listReturn.Add(newObj);
                
            }
            return listReturn;
        }
    }
}
