﻿using VOffice.Model;
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
    public partial class DocumentRecipentRepository : BaseRepository<DocumentRecipent>
    {
        public DocumentRecipentRepository()
        {
        }
        public List<SPGetDocumentRecipentByDocIdAndRecivedDoc_Result> GetRecipentsByDocIdAndReceivedDoc(int documentId, bool receivedDocument)
        {
            List<SPGetDocumentRecipentByDocIdAndRecivedDoc_Result> result = new List<SPGetDocumentRecipentByDocIdAndRecivedDoc_Result>();
            result = _entities.SPGetDocumentRecipentByDocIdAndRecivedDoc(documentId, receivedDocument).ToList();
            return result;
        }

    }
}
