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
    public class NotificationCenterValidator : AbstractValidator<NotificationCenter>
    {
        public NotificationCenterValidator()
        {
            RuleFor(x => x.Type).NotNull().WithMessage("Loại dữ liệu không được trống.");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Thời điểm thực hiện không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Thời điểm thực hiện không hợp lệ.");
            RuleFor(x => x.ReceivedUserId).NotEmpty().WithMessage("Người nhận thông báo không được trống.");
            RuleFor(x => x.RecordId).NotNull().WithMessage("Đối tượng dữ liệu thông báo không được trống.");
        }
    }
}
