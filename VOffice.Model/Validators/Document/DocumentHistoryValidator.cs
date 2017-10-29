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
    public class DocumentHistoryValidator : AbstractValidator<DocumentHistory>
    {
        public DocumentHistoryValidator()
        {
            RuleFor(x => x.DocumentId).NotNull().WithMessage("Văn bản không được trống.");
            RuleFor(x => x.AttempOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.AttempOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
