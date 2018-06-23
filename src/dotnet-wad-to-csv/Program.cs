using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNet.WadToCsv.Models;
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
                    Level = props["Level"].Int32Value.GetValueOrDefault(),
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
                            outputFile.WriteLine(log);
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
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            Console.WriteLine("ConsoleCancelEvent received => Cancelling token");
            consoleCancelEventArgs.Cancel = true;
            Cts.Cancel();
        }
    }
}
