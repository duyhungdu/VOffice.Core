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
    public class ImportantJobValidator : AbstractValidator<ImportantJob>
    {
        public ImportantJobValidator()
        {
            RuleFor(x => x.Content).NotEmpty().WithMessage("Nội dung không được trống.");          
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}
