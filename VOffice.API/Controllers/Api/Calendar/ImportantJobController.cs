using System.Web.Http;
using VOffice.ApplicationService.Implementation.Contract;
using VOffice.Core.Messages;
using VOffice.Model;

namespace VOffice.API.Controllers.Api.Calendar
{
    /// <summary>
    /// CRUD important job
    /// </summary>
    public class ImportantJobController : ApiController
    {
        private ICalendarService calendarService;

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="_calendarService"></param>
        public ImportantJobController(ICalendarService _calendarService)
        {
            calendarService = _calendarService;
        }

        /// <summary>
        /// Get important job
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse<ImportantJob> Get(int id)
        {
            return calendarService.GetImportantJobById(id);
        }

        /// <summary>
        /// Add ImportantJob
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse<ImportantJob> Add(ImportantJob model)
        {
            return calendarService.AddImportantJob(model);
        }

        /// <summary>
        /// Edit ImportantJob
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse<ImportantJob> Edit(ImportantJob model)
        {
            return calendarService.EditImportantJob(model);
        }

        /// <summary>
        /// Delete ImportantJob
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse DeleteLogicalImportantJob(int id)
        {
            return calendarService.DeleteLogicalImportantJob(id);
        }
    }
}