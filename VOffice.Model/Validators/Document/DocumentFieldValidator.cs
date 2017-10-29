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
    public class DocumentFieldValidator : AbstractValidator<DocumentField>
    {
        public DocumentFieldValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Mã lĩnh vực văn bản không được để trống.");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Tên lĩnh vực văn bản không được để trống.");
            RuleFor(x => x.AllowClientEdit).NotNull().WithMessage("Cho phép sửa không được để trống.");
            RuleFor(x => x.Active).NotNull().WithMessage("Kích hoạt không được để trống.");
            RuleFor(x => x.Deleted).NotNull().WithMessage("Đánh dấu xóa không được để trống.");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
            RuleFor(x => x.EditedOn).NotNull().WithMessage("Ngày sửa không được trống.");
            RuleFor(x => x.EditedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày sửa không hợp lệ.");
            RuleFor(x => x.EditedBy).NotEmpty().WithMessage("Chọn người sửa.");
        }
    }
}
