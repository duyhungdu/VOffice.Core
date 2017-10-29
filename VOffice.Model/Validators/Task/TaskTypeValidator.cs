using VOffice.Core.Validations;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Model;

namespace VOffice.Model.Validators
{
    public class TaskTypeValidator : AbstractValidator<TaskType>
    {
        public TaskTypeValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Mã mảng công việc không được trống");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Tên mảng công việc không được trống");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
        }
    }
}
