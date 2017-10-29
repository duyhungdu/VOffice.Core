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
    public class UserNotificationValidator : AbstractValidator<UserNotification>
    {
        public UserNotificationValidator()
        {
            RuleFor(x => x.Type).NotNull().WithMessage("Loại không được trống.");
            RuleFor(x => x.AttempOn).NotNull().WithMessage("Thời điểm thực hiện không được trống.");
            RuleFor(x => x.AttempOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Thời điểm thực hiện không hợp lệ.");
            RuleFor(x => x.ClientId).NotEmpty().WithMessage("Chọn thiết bị.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Chọn người dùng.");
        }
    }
}
