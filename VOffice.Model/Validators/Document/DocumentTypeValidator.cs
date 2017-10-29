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
    public class DocumentTypeValidator : AbstractValidator<DocumentType>
    {
        public DocumentTypeValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Mã loại văn bản không được để trống.");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Tên loại văn bản không được để trống.");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
            RuleFor(x => x.EditedOn).NotNull().WithMessage("Ngày cập nhật không được trống.");
            RuleFor(x => x.EditedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày cập nhật không hợp lệ.");
            RuleFor(x => x.EditedBy).NotEmpty().WithMessage("Chọn người cập nhật.");

        }
    }
}
