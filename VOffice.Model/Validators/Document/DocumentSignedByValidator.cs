﻿using VOffice.Core.Validations;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOffice.Model;

namespace VOffice.Model.Validators
{
    public class DocumentSignedByValidator : AbstractValidator<DocumentSignedBy>
    {
        public DocumentSignedByValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Họ tên không được trống.");
            RuleFor(x => x.DepartmentId).NotNull().WithMessage("Đơn vị không được trống.");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
