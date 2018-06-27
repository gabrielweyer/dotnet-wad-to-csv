using System;
using System.ComponentModel.DataAnnotations;
using DotNet.AzureDiagnostics.Core.Parsers;

namespace DotNet.AzureDiagnostics.Core.Validation
{
    public class Iso8601TimeDurationAttribute : ValidationAttribute
    {
        public Iso8601TimeDurationAttribute()
            : base("{0} is not a valid ISO 8601 time duration. Refer to the time component of https://en.wikipedia.org/wiki/ISO_8601#Durations")
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            try
            {
                (value as string).ParseIso8601TimeDuration();
                return ValidationResult.Success;
            }
            catch (FormatException)
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }
        }
    }
}
