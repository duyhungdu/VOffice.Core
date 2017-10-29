using VOffice.Core.Messages;
using VOffice.Core.Validations;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VOffice.ApplicationService
{
    public abstract class BaseService
    {
        public BaseService()
        {
        }
        protected BaseResponse Run(Action action)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                action();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                SetErrorResponse(response, ex);
            }

            return response;
        }



        protected BaseResponse<T> Run<T>(Func<T> func)
        {
            BaseResponse<T> response = new BaseResponse<T>();
            try
            {
                response.Value = func();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                SetErrorResponse(response, ex);
            }

            return response;
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        protected List<ValidationRule> Validate<T>(T model, AbstractValidator<T> validator)
        {
            var result = validator.Validate(model);
            var errors = new List<ValidationRule>();
            if (!result.IsValid)
            {
                var failures = result.Errors;
                foreach (var f in failures)
                {
                    errors.Add(new ValidationRule { PropertyName = f.PropertyName, Rule = f.ErrorMessage });
                }
            }
            return errors;
        }
        protected void SetErrorResponse(BaseResponse response, Exception ex)
        {
            response.IsSuccess = false;
            response.Message = HttpUtility.JavaScriptStringEncode(ex.ToString());
        }
    }
}
