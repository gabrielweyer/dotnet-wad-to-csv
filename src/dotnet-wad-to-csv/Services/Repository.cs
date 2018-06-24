using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNet.WadToCsv.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNet.WadToCsv.Services
{
    public class Repository
    {
        private readonly CloudTable _table;

        private static WadLogs Resolver(string pk, string rk, DateTimeOffset ts,
            IDictionary<string, EntityProperty> props, string etag) => new WadLogs
        {
            Generated = props["PreciseTimeStamp"].DateTime.GetValueOrDefault(),
            Level = props["Level"].Int32Value.GetValueOrDefault(),
            Message = props["Message"].StringValue,
        };

        public Repository(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("WADLogsTable");
        }

        public async Task<List<WadLogs>> GetLogsAsync(DateTime since, CancellationToken token)
        {
            var sinceAsPartitionKey = $"0{since.Ticks}";

            var query = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan,
                    sinceAsPartitionKey))
                .Select(new[] {"PreciseTimeStamp", "Level", "Message"});

            TableContinuationToken continuationToken = null;

            var logs = new List<WadLogs>();

            try
            {
                do
                {
                    var result = await _table.ExecuteQuerySegmentedAsync(query, Resolver, continuationToken, null,
                        null, token);

                    logs.AddRange(result.Results);

                    continuationToken = result.ContinuationToken;
                } while (continuationToken != null);
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == 404)
            {
                throw new InvalidOperationException(
                    "The table 'WADLogsTable' does not exist in this storage account.", e);
            }

            return logs;
        }
    }
}
