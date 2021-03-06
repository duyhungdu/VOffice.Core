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
    public partial class DocumentFieldDepartmentRepository : BaseRepository<DocumentFieldDepartment>
    {
        public DocumentFieldDepartmentRepository()
        {
        }
        public List<SPGetDocumentFieldDepartment_Result> Filter(int departmnetId)
        {
            /*Lấy danh sách lĩnh vực phục vụ cho form danh sách */
            List<SPGetDocumentFieldDepartment_Result> result = new List<SPGetDocumentFieldDepartment_Result>();
            result = _entities.SPGetDocumentFieldDepartment(departmnetId).ToList();
            return result;
        }
        public List<SPGetListDocumentFieldDepartment_Result> GetList(DocumentFieldDepartmentQuery query, out int count)
        {
            /*Lấy danh sách lĩnh vực đơn vị khi thực hiện thêm mới văn bản*/
            count = 0;
            int pageNumber = query.PageNumber != 0 ? query.PageNumber : 1;
            var keyword = string.IsNullOrEmpty(query.Keyword) != true ? query.Keyword : "";
            var departmnetId = query.DepartmentId;
            var start = 0;
            var limit = query.PageSize;
            start = (pageNumber - 1) * limit;
            int totalRow = 0;
            List<SPGetListDocumentFieldDepartment_Result> result = new List<SPGetListDocumentFieldDepartment_Result>();
            ObjectParameter prTotalRow = new ObjectParameter("total", totalRow);
            result = _entities.SPGetListDocumentFieldDepartment(departmnetId, Util.DetecVowel(keyword), start, limit, prTotalRow).ToList();
            count = (prTotalRow.Value == null) ? 0 : Convert.ToInt32(prTotalRow.Value);
            return result;
        }
        public List<SPGetDocFieldDepartmentByDocIdAndReceivedDoc_Result> GetListByDocIdAndReceivedDoc(int docId, bool receivedDoc)
        {
            /*Lấy danh sách lĩnh vực theo DocumentId và ReceivedDocument */
            List<SPGetDocFieldDepartmentByDocIdAndReceivedDoc_Result> result = new List<SPGetDocFieldDepartmentByDocIdAndReceivedDoc_Result>();
            result = _entities.SPGetDocFieldDepartmentByDocIdAndReceivedDoc(docId, receivedDoc).ToList();
            return result;
        }
    }
}
