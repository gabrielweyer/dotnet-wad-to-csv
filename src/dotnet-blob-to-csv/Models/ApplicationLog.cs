using System;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace DotNet.BlobToCsv.Models
{
    public class ApplicationLog
    {
        public string Level { get; set; }
        public DateTime Generated { get; set; }
        public string Message { get; set; }
    }

    public sealed class ApplicationLogReadMap : ClassMap<ApplicationLog>
    {
        public ApplicationLogReadMap()
        {
            Map(l => l.Level).Name("Level", "level");
            Map(l => l.Generated).Name("Generated", "eventTickCount").TypeConverter<DateTimeConverter>();;
            Map(l => l.Message).Name("Message", "message");
        }
    }

    public class DateTimeConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return new DateTime(long.Parse(text), DateTimeKind.Utc);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((DateTime) value).ToString("yyyy-MM-ddTHH:mm:ss.fffT");
        }
    }
}
