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
    public class DocumentDeliveredValidator : AbstractValidator<DocumentDelivered>
    {
        public DocumentDeliveredValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Trích yếu không được trống.");
            RuleFor(x => x.DocumentDate).NotNull().WithMessage("Ngày văn bản không được trống.");
            RuleFor(x => x.DocumentDate).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày văn bản không hợp lệ.");
            RuleFor(x => x.DeliveredDate).NotNull().WithMessage("Ngày nhận văn bản không được trống.");
            RuleFor(x => x.DeliveredDate).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày nhận văn bản không hợp lệ.");
            RuleFor(x => x.DepartmentId).NotNull().WithMessage("Đơn vị không được trống.");

            RuleFor(x => x.RecipientsDivision).NotEmpty().WithMessage("Nơi nhận không được trống.");
            RuleFor(x => x.SignedBy).NotEmpty().WithMessage("Người kí không được trống.");
           

            RuleFor(x => x.DocumentTypeId).NotNull().WithMessage("Loại văn bản không được trống.");
            RuleFor(x => x.NumberOfCopies).NotNull().WithMessage("Số bản không được trống.");
            RuleFor(x => x.NumberOfPages).NotNull().WithMessage("Số trang không được trống.");

            RuleFor(x => x.SecretLevel).NotNull().WithMessage("Độ mật không được trống.");
            RuleFor(x => x.UrgencyLevel).NotNull().WithMessage("Độ khẩn không được trống.");

            RuleFor(x => x.LegalDocument).NotNull().WithMessage("Là văn bản pháp luật không được trống.");

            RuleFor(x => x.AttachmentName).NotEmpty().WithMessage("Tên tệp không được trống.");
            RuleFor(x => x.AttachmentPath).NotEmpty().WithMessage("Đường dẫn tệp không được trống.");

            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
            RuleFor(x => x.EditedOn).NotNull().WithMessage("Ngày cập nhật không được trống.");
            RuleFor(x => x.EditedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày cập nhật không hợp lệ.");
            RuleFor(x => x.EditedBy).NotEmpty().WithMessage("Chọn người cập nhật.");


            
        }
    }
}
