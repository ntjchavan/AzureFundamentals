using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorageBlob.Models;

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
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(file.FileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream);

            return blobClient.Uri.ToString();
        }

        public async Task<BlobDetails> GetBlobDetailsAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return new BlobDetails();
            }

            BlobProperties properties = await blobClient.GetPropertiesAsync();

            return new BlobDetails
            {
                Name = blobName,
                ContentType = properties.ContentType,
                Size = properties.ContentLength,
                LastModified = properties.LastModified,
                Uri = blobClient.Uri.ToString()
            };
        }

        public async Task<List<string>> GetBlobListAsync(string containerName)
        {
            List<string> blobList = new List<string>();
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!await containerClient.ExistsAsync())
            {
                return blobList;
            }

            await foreach (var item in containerClient.GetBlobsAsync())
            {
                blobList.Add(item.Name);
            }
            return blobList;
        }

        public async Task<(Stream? Content, string? ContentType, bool IsBlobAvailable)> DownloadBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);


            if (!await blobClient.ExistsAsync())
                return (null, null, false);

            var result = await blobClient.DownloadStreamingAsync();
            //var data = (result.Value.Content, result.Value.Details.ContentType);
            return (result.Value.Content, result.Value.Details.ContentType, true);
        }

        public async Task<bool> DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!await containerClient.ExistsAsync())
            {
                return false;
            }

            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> DeleteBlobsAsync(string containerName, List<string> blobNames)
        {
            var deletedBlobs = new List<string>();
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            foreach (var blobName in blobNames)
            {
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                var result = await blobClient.DeleteIfExistsAsync();

                if (result)
                {
                    deletedBlobs.Add(blobName);
                }
            }
            return deletedBlobs;
        }

        //public async Task<string> UploadBlobAsync(string containerName, IFormFile file)
        //{
        //    var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        //    await containerClient.CreateIfNotExistsAsync(); // Optional, it will create container if not exists

        //    var blobClient = containerClient.GetBlobClient(file.FileName);

        //    using var stream = file.OpenReadStream();
        //    await blobClient.UploadAsync(stream, overwrite: true);

        //    return blobClient.Uri.ToString();
        //}

        //public async Task<List<string>> GetBlobListAsync(string containerName)
        //{
        //    var blobList = new List<string>();
        //    var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        //    // Optional: check if the container exists
        //    if (!await containerClient.ExistsAsync())
        //    {
        //        return blobList;
        //    }

        //    //await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        //    //{
        //    //    blobList.Add(blobItem.Name);
        //    //}

        //    // we can customize that how many blobs return first time. Bydefault it 5000 size
        //    // blobPages is type of Page<BlobItem>
        //    await foreach (var blobPages in containerClient.GetBlobsAsync().AsPages(pageSizeHint: 1000))
        //    {
        //        foreach (BlobItem item in blobPages.Values)
        //        {
        //            blobList.Add(item.Name);
        //        }
        //    }

        //    return blobList;
        //}

        //public async Task<bool> DeleteBlobAsync(string containerName, string blobName)
        //{
        //    var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        //    if (!await containerClient.ExistsAsync())
        //    {
        //        return false;
        //    }

        //    BlobClient blobClient = containerClient.GetBlobClient(blobName);

        //    var result = await blobClient.DeleteIfExistsAsync();

        //    return result.Value;
        //}

    }
}
