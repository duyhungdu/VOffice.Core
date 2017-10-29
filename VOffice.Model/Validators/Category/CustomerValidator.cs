using VOffice.Core.Validations;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Model;
using System.Text.RegularExpressions;

namespace VOffice.Model.Validators
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Mã khách hàng không để trống.");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Tên khách hàng không để trống.");
            RuleFor(x => x.Order).GreaterThan(0).WithMessage("Thứ tự phải lớn hơn 0.");
            //sửa
            RuleFor(x => x.PhoneNumber).Length(10, 11).WithMessage("Độ dài số điện thoại chưa đúng.");
        }
    }
}
