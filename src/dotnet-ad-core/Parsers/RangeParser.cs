using System;
using System.ComponentModel.DataAnnotations;
using DotNet.AzureDiagnostics.Core.Models;

namespace DotNet.AzureDiagnostics.Core.Parsers
{
    public static class RangeParser
    {
        public static ValidationResult TryGetRange(string last, string from, string to, out Range range)
        {
            range = null;

            if (string.IsNullOrEmpty(last) && string.IsNullOrEmpty(from) ||
                !string.IsNullOrEmpty(last) && !string.IsNullOrEmpty(from))
            {
                return new ValidationResult("Either '--last' or '--from' should be set.");
            }

            if (string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
            {
                return new ValidationResult("'--to' can only be used in conjonction with '--from'.");
            }

            DateTime rangeFrom;

            if (string.IsNullOrEmpty(from))
            {
                rangeFrom = DateTime.UtcNow - last.ParseIso8601TimeDuration();
            }
            else
            {
                from.TryParseIso8601DateTime(out rangeFrom);
            }

            DateTime? rangeTo = null;

            if (!string.IsNullOrEmpty(from) && to.TryParseIso8601DateTime(out var tmpTo))
            {
                rangeTo = tmpTo;
            }

            range = new Range
            {
                From = rangeFrom,
                To = rangeTo
            };

            return ValidationResult.Success;
        }
    }
}
