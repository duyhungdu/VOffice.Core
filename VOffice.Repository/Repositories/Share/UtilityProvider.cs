using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Repository.Queries;

namespace VOffice.Repository
{
    public static class UtilityProvider
    {
        static SystemConfigDepartmentRepository _systemConfigDepartmentRepository = new SystemConfigDepartmentRepository();
        public async static void SendEmail(int departmentId, string mailTo, string mailCC, string mailBCC, string subject, string content)
        {
            SystemConfigDepartmentQuery query = new SystemConfigDepartmentQuery();
            query.DefaultValue = "suminshop.net@gmail.com";
            query.DepartmentId = departmentId;
            query.Title = "NOTICEEMAIL";
            string fromEmail = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(query);
            query.DefaultValue = "duyhung@6";
            query.DepartmentId = departmentId;
            query.Title = "NOTICEEMAIL_PASSWORD";
            string fromEmailPassword = _systemConfigDepartmentRepository.GetSystemConfigDepartmentValue(query);
            try
            {
                await EmailHelper.Send_Email(fromEmail, fromEmailPassword, mailTo, mailCC, mailBCC, subject, content);
            }
            catch
            {

            }
        }
    }
}
