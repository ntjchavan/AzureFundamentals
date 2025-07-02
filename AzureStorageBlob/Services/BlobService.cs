using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureStorageBlob.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadBlobAsync(string containerName, IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(); // Optional, it will create container if not exists

            var blobClient = containerClient.GetBlobClient(file.FileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task<List<string>> GetBlobListAsync(string containerName)
        {
            var blobList = new List<string>();
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            // Optional: check if the container exists
            if (!await containerClient.ExistsAsync())
            {
                return blobList;
            }

            //await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            //{
            //    blobList.Add(blobItem.Name);
            //}

            // we can customize that how many blobs return first time. Bydefault it 5000 size
            // blobPages is type of Page<BlobItem>
            await foreach (var blobPages in containerClient.GetBlobsAsync().AsPages(pageSizeHint: 1000))
            {
                foreach (BlobItem item in blobPages.Values)
                {
                    blobList.Add(item.Name);
                }
            }

            return blobList;
        }

        public async Task<bool> DeleteBlobAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!await containerClient.ExistsAsync())
            {
                return false;
            }

            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            var result = await blobClient.DeleteIfExistsAsync();

            return result.Value;
        }

    }
}
