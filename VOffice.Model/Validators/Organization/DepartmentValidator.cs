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
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Mã đơn vị không được trống.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Tên đơn vị không được trống.");
            RuleFor(x => x.ParentId).NotNull().WithMessage("Đơn vị cha không được trống.");
            RuleFor(x => x.Order).NotNull().WithMessage("Thứ tự không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
