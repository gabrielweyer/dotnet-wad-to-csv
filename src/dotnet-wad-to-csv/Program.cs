using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNet.WadToCsv
{
    class Program
    {
        [Option(ShortName = "d", LongName = "duration", Description = "ISO 8601 duration")]
        public string Duration { get; }

        [Required]
        [Option(ShortName = "o", LongName = "output", Description = "Output file path")]
        public string OutputFilePath { get; }

        static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        private async Task OnExecuteAsync()
        {
            // TODO: test validity of output file path

            var fullPath = Path.GetFullPath(OutputFilePath);

            // TODO: default duration if missing

            // TODO: cancellation token

            var storageConnectionString = Prompt.GetPassword("SAS", ConsoleColor.White, ConsoleColor.DarkBlue);

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table =  tableClient.GetTableReference("WADLogsTable");

            if (!await table.ExistsAsync())
            {
                throw new InvalidOperationException("The table 'WADLogsTable' does not exist in this storage account.");
            }

            // TODO: compute timestamp

            const string since = "0636649120905773478";

            var query = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan, since))
                .Select(new[] { "PreciseTimeStamp", "Level", "Message" });

            EntityResolver<WadLogs> resolver = (pk, rk, ts, props, etag) => new WadLogs {
                Generated = props["PreciseTimeStamp"].DateTime.GetValueOrDefault(),
                Level = ParseLevel(props["Level"].Int32Value),
                Message = props["Message"].StringValue,
            };

            TableContinuationToken continuationToken = null;

            using (var outputFile = File.CreateText(fullPath))
            {
                outputFile.WriteLine("Generated,Level,Message");

                do
                {
                    var result = await table.ExecuteQuerySegmentedAsync(query, resolver, continuationToken);

                    foreach (var log in result.Results)
                    {
                        if (log.Message.Length <= 12 || log.Message[11] != 'M')
                        {
                            continue;
                        }

                        outputFile.WriteLine($"{FormatGenerated(log.Generated)},{log.Level},{FormatMessage(log.Message)}");
                    }

                    continuationToken = result.ContinuationToken;
                } while (continuationToken != null);
            }

            string ParseLevel(int? level)
            {
                // TODO: investigate why Warning is 3 and Information is 4

                switch (level)
                {
                    case 1:
                        return "Critical";
                    case 2:
                        return "Error";
                    case 4:
                        return "Information";
                    case 3:
                        return "Warning";
                    case 5:
                        return "Verbose";
                }

                return "Undefined";
            }

            string FormatMessage(string message)
            {
                return message.Substring(33, message.Length - 23 - 33);
            }

            string FormatGenerated(DateTime generated)
            {
                return generated.ToString("yyyy-MM-ddTHH:mm:ss.fffT");
            }
        }

        class WadLogs
        {
            public DateTime Generated { get; set; }
            public string Message { get; set; }
            public string Level { get; set; }
        }
    }
}
