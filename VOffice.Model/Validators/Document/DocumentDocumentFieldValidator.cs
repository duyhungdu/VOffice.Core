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
    /// <summary>
    /// DocumentField API. An element of DocumentService
    /// </summary>
    public class DocumentDocumentFieldValidator : AbstractValidator<DocumentDocumentField>
    { 
        public DocumentDocumentFieldValidator()
        {
            RuleFor(x => x.DocumentId).NotNull().WithMessage("Văn bản không được trống.");
            RuleFor(x => x.DocumentFieldDepartmentId).NotNull().WithMessage("Lĩnh vực không được trống.");

            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
