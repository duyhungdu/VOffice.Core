using FluentValidation;
using System;
using VOffice.Core.Validations;

namespace VOffice.Model.Validators
{
    public class EventGoogleEventValidator : AbstractValidator<EventGoogleEvent>
    {
        public EventGoogleEventValidator()
        {
            RuleFor(x => x.GoogleEventId).NotEmpty().WithMessage("Google event không được để trống");
            RuleFor(x => x.EventId).NotNull().WithMessage("Sự kiện không được trống.");
            RuleFor(x => x.CreatedOn).NotNull().WithMessage("Ngày tạo không được trống.");
            RuleFor(x => x.CreatedOn).SetValidator(new ValidSQLDateTimeValidator<DateTime>()).WithMessage("Ngày tạo không hợp lệ.");
            RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Chọn người tạo.");
        }
    }
}