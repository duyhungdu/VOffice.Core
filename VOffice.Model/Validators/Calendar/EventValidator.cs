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
    public class EventValidator : AbstractValidator<Event>
    {
        public EventValidator()
        {
            
            RuleFor(x => x.DepartmentId).NotNull().WithMessage("Đơn vị không được trống.");
            RuleFor(x => x.OccurDate).NotNull().WithMessage("Ngày thực hiện không được trống.");
            RuleFor(x => x.OccurDate).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày thực hiện không hợp lệ.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Nội dung không được trống.");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
