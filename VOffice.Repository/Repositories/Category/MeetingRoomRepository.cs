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
    public partial class MeetingRoomRepository : BaseRepository<MeetingRoom>
    {
        public MeetingRoomRepository()
        {

        }
        public List<SPGetMeetingRoom_Result> Filter(MeetingRoomQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var listDepartmentID = string.IsNullOrEmpty(query.ListDepartmentId) != true ? query.ListDepartmentId : "";
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetMeetingRoom_Result> result = new List<SPGetMeetingRoom_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetMeetingRoom(Util.DetecVowel(keyword), listDepartmentID, start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public List<SPGetMeetingRoomByDepartmentId_Result> GetMeetingRoomByDepartmentId(int departmentId)
        {
            List<SPGetMeetingRoomByDepartmentId_Result> result = new List<SPGetMeetingRoomByDepartmentId_Result>();
            result = _entities.SPGetMeetingRoomByDepartmentId(departmentId).ToList();
            return result;
        }
    }
}
