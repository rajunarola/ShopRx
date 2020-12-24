using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
namespace RxFair.Utility.Helpers
{
    public interface IStorageClient
    {
        Task UploadAsync(string containerName, string blobName, string filePath);
        Task UploadAsync(string containerName, string blobName, Stream stream);
        Task<MemoryStream> DownloadAsync(string containerName, string blobName);
        Task DownloadAsync(string containerName, string blobName, string path);
        Task DeleteAsync(string containerName, string blobName);
        Task<bool> ExistsAsync(string containerName, string blobName);

        Task<string> GetBlobSasUriAsync(string containerName, string blobName, string policyName = null);
    }
    public class AzureStorageClient : IStorageClient
    {
        readonly CloudBlobClient _client;
        public AzureStorageClient(string connectionSetting)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionSetting);
            _client = storageAccount.CreateCloudBlobClient();
        }
        #region Private
        private async Task<CloudBlobContainer> GetContainerAsync(string containerName, string directoryName = "", bool isdirectory = false)
        {
            CloudBlobContainer container = _client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }
        private async Task<CloudBlockBlob> GetBlockBlobAsync(string containerName, string blobName)
        {

            //Container
            var blobContainer = await GetContainerAsync(containerName);
            //Blob
            var blockBlob = blobContainer.GetBlockBlobReference(blobName);
            return blockBlob;
        }
        #endregion
        #region Public
        public async Task UploadAsync(string containerName, string blobName, string filePath)
        {
            var blockBlob = await GetBlockBlobAsync(containerName, blobName);
            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                fileStream.Position = 0;
                await blockBlob.UploadFromStreamAsync(fileStream);
            }
        }
        public async Task UploadAsync(string containerName, string blobName, Stream stream)
        {
            var blockBlob = await GetBlockBlobAsync(containerName, blobName);
            stream.Position = 0;
            await blockBlob.UploadFromStreamAsync(stream);
        }
        public async Task<MemoryStream> DownloadAsync(string containerName, string blobName)
        {
            var blockBlob = await GetBlockBlobAsync(containerName, blobName);
            using (var stream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(stream);
                return stream;
            }
        }
        public async Task DownloadAsync(string containerName, string blobName, string path)
        {
            var blockBlob = await GetBlockBlobAsync(containerName, blobName);
            await blockBlob.DownloadToFileAsync(path, FileMode.Create);
        }
        public async Task DeleteAsync(string containerName, string blobName)
        {
            var blockBlob = await GetBlockBlobAsync(containerName, blobName);
            await blockBlob.DeleteAsync();
        }
        public async Task<bool> ExistsAsync(string containerName, string blobName)
        {
            var blockBlob = await GetBlockBlobAsync(containerName, blobName);
            return await blockBlob.ExistsAsync();
        }
        public async Task<string> GetBlobSasUriAsync(string containerName, string blobName, string policyName = null)
        {
            string sasBlobToken;

            // Get a reference to a blob within the container.
            // Note that the blob may not exist yet, but a SAS can still be created for it.
            var container = await GetContainerAsync(containerName);
            var blob = await GetBlockBlobAsync(containerName, blobName);

            if (policyName == null)
            {
                long expiryHours = Convert.ToInt32(2);
                // Create a new access policy and define its constraints.
                // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad-hoc SAS, and 
                // to construct a shared access policy that is saved to the container's shared access policies. 
                SharedAccessBlobPolicy adHocSas = new SharedAccessBlobPolicy()
                {
                    // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request. 
                    // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.

                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(expiryHours),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
                };

                // Generate the shared access signature on the blob, setting the constraints directly on the signature.
                sasBlobToken = blob.GetSharedAccessSignature(adHocSas);
            }
            else
            {
                // Generate the shared access signature on the blob. In this case, all of the constraints for the
                // shared access signature are specified on the container's stored access policy.
                sasBlobToken = blob.GetSharedAccessSignature(null, policyName);
            }

            // Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }
        #endregion
    }
}
