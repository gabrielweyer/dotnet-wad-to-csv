using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DotNet.BlobToCsv.Services
{
    public class Repository
    {
        private readonly CloudBlobContainer _container;

        public Repository(string sas, string container)
        {
            var storageAccount = CloudStorageAccount.Parse(sas);
            var blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(container);
        }

        public async Task DownloadLogBlobsAsync(List<CloudBlockBlob> blobs, string tempDirectory, CancellationToken cancellationToken)
        {

            foreach (var blob in blobs)
            {
                var filePath = Path.Combine(tempDirectory, blob.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                await blob.DownloadToFileAsync(
                    filePath,
                    FileMode.Create,
                    AccessCondition.GenerateEmptyCondition(),
                    new BlobRequestOptions(),
                    new OperationContext(),
                    cancellationToken);
            }
        }

        public async Task<List<CloudBlockBlob>> ListLogBlobsAsync(string prefix, CancellationToken cancellationToken)
        {
            BlobContinuationToken continuationToken = null;

            var blobs = new List<CloudBlockBlob>();

            do
            {
                var result = await _container.ListBlobsSegmentedAsync(
                    prefix,
                    true,
                    BlobListingDetails.None,
                    null,
                    continuationToken,
                    new BlobRequestOptions(),
                    new OperationContext(),
                    cancellationToken);

                blobs.AddRange(result.Results.OfType<CloudBlockBlob>());

                continuationToken = result.ContinuationToken;
            } while (continuationToken != null);

            return blobs;
        }
    }
}
