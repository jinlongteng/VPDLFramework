using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VPDLFramework.ValidationModels
{
    public class IPTextValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            IPAddress ipAddress;            
            if (value.ToString().Split('.').Length==4&&IPAddress.TryParse(value.ToString(), out ipAddress))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "not valid");
            }
        }
    }
}
