using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Gui.Infrastructure.Validation
{
    using System.Globalization;
    using System.Windows.Controls;

    public class LongValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return long.TryParse((value ?? "").ToString(), out long parsedValue) ? ValidationResult.ValidResult : new ValidationResult(false, "Field must be a valid Integer");
        }
    }
}
