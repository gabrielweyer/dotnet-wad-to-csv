namespace DotNet.AzureDiagnostics.Core.Helpers
{
    public static class StorageAccountHelper
    {
        public static string GetStorageAccountName(string connectionString)
        {
            var lastIndex = connectionString.LastIndexOf(':');

            if (lastIndex == -1) return null;

            var storageAccountUri = connectionString.Substring(lastIndex + 3);

            var firstindex = storageAccountUri.IndexOf('.');

            return firstindex == -1 ? null : storageAccountUri.Substring(0, firstindex);
        }
    }
}
