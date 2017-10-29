using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Model
{
    public partial class UserTaskAnalystic
    {
        public int DefaultNumber { get; set; }
        public int InprocessNumber { get; set; }
        public int ResolvedNumber { get; set; }
        public int ClosedNumber { get; set; }
        public int ReopenNumber { get; set; }
        public int ExpriedNumber { get; set; }
    }
}
