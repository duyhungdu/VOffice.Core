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
    public class NoticeValidator : AbstractValidator<Notice>
    {
        public NoticeValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Nội dung không để trống.");
        }
    }
}
