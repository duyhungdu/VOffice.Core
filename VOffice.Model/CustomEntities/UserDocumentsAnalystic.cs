using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Model
{
    public partial class UserDocumentsAnalystic
    {
        public List<int> NumberDocumentReceived { get; set; }
        public List<int> NumberDocumentDelivered { get; set; }
        public List<int> NumberDocumentHaveNotRead { get; set; }
        public List<int> NumberDocumentReceivedHaveNotRead { get; set; }
        public List<int> NumberDocumentDeliveredHaveNotRead { get; set; }
        public List<int> NumberDocumentHaventAddedDocumentBook { get; set; }
    }
}
