using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNet.WadToCsv.Validation;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNet.WadToCsv
{
    class Program
    {
        [Required]
        [Iso8601TimeDuration]
        [Option(ShortName = "l", LongName = "last",
            Description = "ISO 8601 duration, substracted from the current UTC time")]
        public string Last { get; }

        [Required]
        [WritableFile]
        [Option(ShortName = "o", LongName = "output", Description = "Output file path")]
        public string OutputFilePath { get; }

        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        private static readonly CancellationToken Token = Cts.Token;

        static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        private async Task OnExecuteAsync()
        {
            var fullPath = Path.GetFullPath(OutputFilePath);
            var last = Last.ParseIso8601TimeDuration();

            var storageConnectionString = Prompt.GetPassword("SAS", ConsoleColor.White, ConsoleColor.DarkBlue);

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("WADLogsTable");

            try
            {

                if (!await table.ExistsAsync(null, null, Token))
                {
                    throw new InvalidOperationException(
                        "The table 'WADLogsTable' does not exist in this storage account.");
                }

                var since = $"0{(DateTime.UtcNow - last).Ticks}";

                var query = new TableQuery<DynamicTableEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan, since))
                    .Select(new[] {"PreciseTimeStamp", "Level", "Message"});

                EntityResolver<WadLogs> resolver = (pk, rk, ts, props, etag) => new WadLogs
                {
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
                        var result = await table.ExecuteQuerySegmentedAsync(query, resolver, continuationToken, null,
                            null, Token);

                        foreach (var log in result.Results)
                        {
                            if (log.Message.Length <= 12 || log.Message[11] != 'M')
                            {
                                continue;
                            }

                            outputFile.WriteLine(
                                $"{FormatGenerated(log.Generated)},{log.Level},{FormatMessage(log.Message)}");
                        }

                        continuationToken = result.ContinuationToken;
                    } while (continuationToken != null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.GetType());
                Console.WriteLine("Message: {0}", e.Message);
                Console.WriteLine("StackTrace:");
                Console.WriteLine(e.Demystify().StackTrace);
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

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            Console.WriteLine("ConsoleCancelEvent received => Cancelling token");
            consoleCancelEventArgs.Cancel = true;
            Cts.Cancel();
        }

        class WadLogs
        {
            public DateTime Generated { get; set; }
            public string Message { get; set; }
            public string Level { get; set; }
        }
    }
}
