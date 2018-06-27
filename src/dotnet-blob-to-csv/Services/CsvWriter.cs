using System;
using System.IO;
using System.Linq;
using CsvHelper;
using DotNet.BlobToCsv.Models;

namespace DotNet.BlobToCsv.Services
{
    public static class CsvWriter
    {
        public static void Write(DateTime from, DateTime to, string logDirectory, string outputFilePath)
        {
            using (var textWriter = File.CreateText(outputFilePath))
            {
                var csvWriter = new CsvHelper.CsvWriter(textWriter);
                csvWriter.Configuration.RegisterClassMap<ApplicationLogReadMap>();

                foreach (var logFile in
                    Directory.GetFileSystemEntries(logDirectory, "*.applicationLog.csv", SearchOption.AllDirectories))
                {
                    using (var textReader = File.OpenText(logFile))
                    {
                        var csvReader = new CsvReader(textReader);
                        csvReader.Configuration.RegisterClassMap<ApplicationLogReadMap>();
                        var logLines = csvReader.GetRecords<ApplicationLog>();
                        csvWriter.WriteRecords(logLines.Where(l => l.Generated >= from && l.Generated <= to));
                    }
                }
            }
        }
    }
}
