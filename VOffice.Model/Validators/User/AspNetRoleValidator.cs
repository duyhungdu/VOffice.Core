using VOffice.Core.Validations;
using FluentValidation;
using System;

namespace VOffice.Model.Validators
{
   public class AspNetRoleValidator: AbstractValidator<AspNetRole>
    {
        public AspNetRoleValidator()
        {
            RuleFor(x => x.Code).NotNull().WithMessage("Mã quyền không được để trống");
            RuleFor(x => x.Name).NotNull().WithMessage("Mã quyền không được để trống");
            RuleFor(x => x.GroupBy).NotNull().WithMessage("Nhóm quyền không được để trống");
            RuleFor(x => x.AllowClientAccess).NotNull().WithMessage("Allow access client được để trống");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.Order).NotNull().WithMessage("Thứ tự không được để trống");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }

        
    }
}
