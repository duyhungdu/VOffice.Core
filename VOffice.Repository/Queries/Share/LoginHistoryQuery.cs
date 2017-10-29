using VOffice.Core.Queries;
using VOffice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Repository.Queries
{
    public class LoginHistoryQuery : BaseQuery<LoginHistory>
    {
        public string UserId { get; set; }
        public string OS { get; set; }
        public string DeviceType { get; set; }
        public string DeviceSerial { get; set; }
        public string BrowserType { get; set; }
        public string UserHostAddress { get; set; }
        public string UserLocation { get; set; }
    }
}
