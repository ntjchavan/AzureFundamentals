﻿using Azure.Storage.Blobs;
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

        public async Task<string> UploadBlobWithTierAsync(string containerName, BlobUploadRequest uploadRequest)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobName = uploadRequest.File?.FileName!;

            using var stream = uploadRequest.File?.OpenReadStream();

            var accessTier = uploadRequest.AccessTier.ToLower() switch
            {
                "cool" => AccessTier.Cool,
                "cold" => AccessTier.Cold,
                "archive" => AccessTier.Archive,
                _ => AccessTier.Hot
            };

            switch (uploadRequest.BlobType.ToLower())
            {
                case "append":
                    break;

                default:
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);
                    await blobClient.UploadAsync(stream, new BlobUploadOptions
                    {
                        AccessTier = accessTier,
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = uploadRequest.File?.ContentType
                        }
                    });
                    return blobClient.Uri.ToString();
            }
            return blobName;
        }

        public async Task<string> UploadBlobWithMetadataAsync(BlobUploadMetadataRequest request)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(request.ContainerName);

            if(!await containerClient.ExistsAsync())
            {
                return string.Empty;
            }

            BlobUploadOptions uploadOptions = new BlobUploadOptions
            {
                Metadata = request.MetaData,
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = request.File?.ContentType
                }
            };

            BlobClient blobName = containerClient.GetBlobClient(request.File?.FileName);
            using var stream = request.File?.OpenReadStream();
            await blobName.UploadAsync(stream, uploadOptions);

            return blobName.Uri.ToString();
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

        public async Task<(string, IDictionary<string, string>)> GetBlobWithMetadataAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return (string.Empty, new Dictionary<string, string>());
            }
            var properties = await blobClient.GetPropertiesAsync();
            var metadata = properties.Value.Metadata;

            return (blobClient.Uri.ToString(), metadata);
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
