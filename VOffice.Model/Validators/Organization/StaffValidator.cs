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
    public class StaffValidator : AbstractValidator<Staff>
    {
        public StaffValidator()
        {
            RuleFor(x => x.StaffCode).NotEmpty().WithMessage("Mã cán bộ không được trống.");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Đệm, tên không được trống.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Họ không được trống.");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Họ tên không được trống.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email không được trống.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Số điện thoại không được trống.");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
