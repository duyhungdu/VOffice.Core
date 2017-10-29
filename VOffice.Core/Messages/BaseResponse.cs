using VOffice.Core.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Core.Messages
{
    /// <summary>
    /// BaseResponse is used to implement the Request-Response pattern.
    /// All returned objects in the Service layer should inherit from this class.
    /// </summary>

    public class BaseResponse
    {
        public bool ShowNotification
        {
            get; set;
        }
        public bool IsSuccess
        {
            get; set;
        }
        public string Message
        {
            get; set;
        }
        public List<ValidationRule> BrokenRules
        {
            get; set;
        }

        public void AddBrokenRules(string propertyName, string rule)
        {
            BrokenRules.Add(new ValidationRule { PropertyName = propertyName, Rule = rule });
        }

        public BaseResponse()
        {
            BrokenRules = new List<ValidationRule>();
            IsSuccess = true;
        }

        public bool IsValid
        {
            get
            {
                return !BrokenRules.Any();
            }
        }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public BaseResponse(T value)
        {
            IsSuccess = true;
            this.Value = value;
            BrokenRules = new List<ValidationRule>();
        }

        public BaseResponse(T value, List<ValidationRule> errors)
        {
            this.Value = value;
            BrokenRules = errors;
            IsSuccess = true;
            if (errors.Count > 0)
            {
                //IsSuccess = false;
            }
        }

        public BaseResponse()
        {
            IsSuccess = true;
            BrokenRules = new List<ValidationRule>();
        }

        public T Value
        {
            get; set;
        }
    }
}
