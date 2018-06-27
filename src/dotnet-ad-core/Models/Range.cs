using System;

namespace DotNet.AzureDiagnostics.Core.Models
{
    public class Range
    {
        public DateTime From { get; set; }
        public DateTime? To { get; set; }

        public override string ToString()
        {
            return $"'{From:u}' to '{FormatTo()}'";

            string FormatTo()
            {
                return To.HasValue ? To.Value.ToString("u") : "Now";
            }
        }
    }
}
