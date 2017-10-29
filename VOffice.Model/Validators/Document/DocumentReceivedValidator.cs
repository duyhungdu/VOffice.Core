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
    public class DocumentReceivedValidator : AbstractValidator<DocumentReceived>
    {
        public DocumentReceivedValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Trích yếu không được trống.");
            RuleFor(x => x.AttachmentName).NotEmpty().WithMessage("Tệp đính kèm không được trống.");
            RuleFor(x => x.DocumentDate).NotNull().WithMessage("Ngày văn bản không được trống.");
            RuleFor(x => x.NumberOfPages).NotNull().WithMessage("Số trang không được trống.");
            RuleFor(x => x.NumberOfCopies).NotNull().WithMessage("Số bản không được trống.");
            RuleFor(x => x.SignedBy).NotEmpty().WithMessage("Người ký không được trống.");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.DocumentDate).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo văn bản không hợp lệ.");            
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
