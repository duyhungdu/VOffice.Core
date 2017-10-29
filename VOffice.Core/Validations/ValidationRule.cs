using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VOffice.Core.Validations
{
    public class ValidationRule
    {
        public string PropertyName;
        public string Rule;
        public Regex RegularExpression;
        public ValidationRule()
        {
        }
        public ValidationRule(string propertyName, string rule)
        {
            PropertyName = propertyName;
            Rule = rule;
        }
    }
    public static class ValidationRuleExtension
    {
        public static string ToString(this List<ValidationRule> list, string separator)
        {
            StringBuilder text = new StringBuilder();
            if (list != null)
            {
                foreach (ValidationRule rule in list)
                {
                    text.Append(rule.Rule);
                    text.Append(separator);
                }
                return text.ToString();
            }
            else
                return string.Empty;
        }
    }
}
