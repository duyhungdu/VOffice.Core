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
    public class DocumentAttachmentValidator : AbstractValidator<DocumentAttachment>
    {
        public DocumentAttachmentValidator()
        {
            RuleFor(x => x.FileName).NotEmpty().WithMessage("Tên tệp không được trống.");
            RuleFor(x => x.FilePath).NotEmpty().WithMessage("Đường dẫn tệp không được trống.");
            RuleFor(x => x.DocumentId).NotNull().WithMessage("Văn bản không được trống.");
            RuleFor(x => x.DepartmentId).NotNull().WithMessage("Đơn vị không được trống.");


            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
            RuleFor(x => x.EditedOn).NotNull().WithMessage("Ngày cập nhật không được trống.");
            RuleFor(x => x.EditedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày cập nhật không hợp lệ.");
            RuleFor(x => x.EditedBy).NotEmpty().WithMessage("Chọn người cập nhật.");
        }
    }
}
