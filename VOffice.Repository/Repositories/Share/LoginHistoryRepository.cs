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
using System.Web;
using System.Net;
using System.Globalization;

namespace VOffice.Repository
{
    public partial class LoginHistoryRepository : BaseRepository<LoginHistory>
    {
        public LoginHistoryRepository()
        {

        }

        public LoginHistory AddLoginHistory(LoginHistory model)
        {
            LoginHistory result = new LoginHistory();
            model.UserHostAddress = HttpContext.Current.Request.UserHostAddress;
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://ipinfo.io/" + model.UserHostAddress);
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                model.UserLocation = ipInfo.Region + ", " + ipInfo.City + ", " + myRI1.EnglishName;
            }
            catch (Exception)
            {
                model.UserLocation = "N/A";
            }
            model.AttempOn = DateTime.Now;
            model.EmailSent = true;
            model.NormalAccess = true;
            try
            {
                result = Add(model);
            }
            catch
            { }
            return result;
        }
        public List<SPGetLoginHistory_Result> Filter(LoginHistoryQuery query, out int count)
        {
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var userId = query.UserId;
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetLoginHistory_Result> result = new List<SPGetLoginHistory_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetLoginHistory(userId, start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
    }
    public class IpInfo
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }


        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }
    }

}