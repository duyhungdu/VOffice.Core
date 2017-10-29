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
    public class DocumentRecipentValidator : AbstractValidator<DocumentRecipent>
    {
        public DocumentRecipentValidator()
        {
            RuleFor(x => x.DocumentId).NotNull().WithMessage("Văn bản không được trống.");
            RuleFor(x => x.ForSending).NotNull().WithMessage("Trạng thái văn bản phục vụ cho việc gửi nhận không được trống.");
            RuleFor(x => x.Forwarded).NotNull().WithMessage("Là chuyển tiếp không được trống.");
            RuleFor(x => x.Assigned).NotNull().WithMessage("Là giao xử lý không được trống.");
            RuleFor(x => x.AddedDocumentBook).NotNull().WithMessage("Đã vào sổ không được trống.");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
