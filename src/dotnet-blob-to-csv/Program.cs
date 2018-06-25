﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.BlobToCsv.Services;
using DotNet.BlobToCsv.Validation;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DotNet.BlobToCsv
{
    class Program
    {
        [Iso8601DateTime]
        [Option(ShortName = "f", LongName = "from",
            Description = "ISO 8601 date time in UTC, time can be omitted")]
        public string From { get; }

        [Iso8601DateTime]
        [Option(ShortName = "t", LongName = "to",
            Description = "ISO 8601 date time in UTC, time can be omitted")]
        public string To { get; }

        [Required]
        [WritableFile]
        [Option(ShortName = "o", LongName = "output", Description = "Required. Output file path")]
        public string OutputFilePath { get; }

        [Required]
        [Option(ShortName = "c", LongName = "container", Description = "Required. The name of the storage container")]
        public string Container { get; }

        [Option(ShortName = "p", LongName = "prefix", Description = "The prefix (if any)")]
        public string Prefix { get; set; }

        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        private static readonly CancellationToken Token = Cts.Token;

        static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        private async Task OnExecuteAsync(string[] args)
        {
            var outputFilePath = Path.GetFullPath(OutputFilePath);

            From.TryParseIso8601DateTime(out var from);
            To.TryParseIso8601DateTime(out var to);

            var sas = Prompt.GetPassword("Shared Access Signature:", ConsoleColor.White, ConsoleColor.DarkBlue);

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            try
            {
                if (Prefix.Last() != '/')
                {
                    Prefix += "/";
                }

                var datePrefixes = PrefixService.BuildBlobPrefixes(from, to, Prefix);

                var repository = new Repository(sas, Container);

                var blobs = new List<CloudBlockBlob>();

                foreach (var datePrefix in datePrefixes)
                {
                    blobs.AddRange(await repository.ListLogBlobsAsync(datePrefix, CancellationToken.None));
                }

                var tempDirectory = Path.Combine(Path.GetTempPath(), "wad-to-csv",
                    Path.GetRandomFileName().Replace(".", string.Empty));
                Directory.CreateDirectory(tempDirectory);

                var filtered = PrefixService.Filter(blobs, from, to, Prefix);

                await repository.DownloadLogBlobsAsync(filtered, tempDirectory, CancellationToken.None);

                CsvWriter.Write(tempDirectory, outputFilePath);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception: {0}", e.GetType());
                Console.WriteLine("Message: {0}", e.Message);
                Console.WriteLine("StackTrace:");
                Console.WriteLine(e.Demystify().StackTrace);
            }
            finally
            {
                Console.ResetColor();
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
