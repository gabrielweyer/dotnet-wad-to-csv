using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace DotNet.BlobToCsv.Validation
{
    public class WritableFileAttribute : ValidationAttribute
    {
        public WritableFileAttribute()
            : base("Invalid file path for {0}")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (!(value is string fullPath))
            {
                return new ValidationResult(FormatErrorMessage($"{context.DisplayName}. The value is not a string."));
            }

            try
            {
                File.WriteAllText(fullPath, "q");
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                var message = $"{Environment.NewLine}{Environment.NewLine}\tException: {e.GetType()}{Environment.NewLine}\tMessage: {e.Message}";
                return new ValidationResult(FormatErrorMessage($"{context.DisplayName}. {message}"));
            }
        }
    }
}
