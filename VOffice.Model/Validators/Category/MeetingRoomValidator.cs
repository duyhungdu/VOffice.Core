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
    public class MeetingRoomValidator : AbstractValidator<MeetingRoom>
    {
        public MeetingRoomValidator()
        {
            RuleFor(x => x.Code).NotNull().WithMessage("Mã phòng không được trống");
            RuleFor(x => x.Title).NotNull().WithMessage("Tên phòng không được trống");
            RuleFor(x => x.Order).NotNull().WithMessage("Thứ tự không được trống");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
