using System.ComponentModel.DataAnnotations;
using DotNet.AzureDiagnostics.Core.Parsers;

namespace DotNet.AzureDiagnostics.Core.Validation
{
    public class Iso8601DateTimeAttribute : ValidationAttribute
    {
        public Iso8601DateTimeAttribute()
            : base("{0} is not a valid ISO 8601 date time. Refer to https://en.wikipedia.org/wiki/ISO_8601")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            return value is string input && input.TryParseIso8601DateTime(out _)
                ? ValidationResult.Success
                : new ValidationResult(FormatErrorMessage(context.DisplayName));
        }
    }
}
