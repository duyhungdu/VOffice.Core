using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Core.Validations
{
    public class ValidSQLDateTimeValidator<T> : PropertyValidator
    {
        public ValidSQLDateTimeValidator()
           : base("Property {PropertyName} !")
        {
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var date = (DateTime)context.PropertyValue;
            var valid = false;
            DateTime minDateTime = DateTime.MaxValue;
            DateTime maxDateTime = DateTime.MinValue;
            minDateTime = new DateTime(1753, 1, 1);
            maxDateTime = new DateTime(9999, 12, 31, 23, 59, 59, 997);
            if (date >= minDateTime && date <= maxDateTime)
            {
                valid = true;
            }
            return valid;
        }
    }
}
