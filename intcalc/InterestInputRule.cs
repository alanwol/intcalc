using System;
using System.Globalization;
using System.Windows.Controls;

namespace intcalc
{
    class InterestInputRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                string vs = value as string;
                if (vs.Length != 0)
                {
                    try
                    {
                        decimal v = decimal.Parse(vs, cultureInfo);
                        if (v >= 2.0m)
                        {
                            return new ValidationResult(false, "Validation error. Value should be less than 2.");
                        }
                    }
                    catch (FormatException)
                    {
                        return new ValidationResult(false, "Validation error. Invalid input entered.");
                    }
                }

                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Validation error. Field input required.");
        }
    }
}
