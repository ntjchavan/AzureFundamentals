using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureStorageBlob.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        BlobServiceClient _blobServiceClient;
        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureConnection:BlobStorageConnection"];
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<BlobContainerInfo> CreateContainerAsync(string containerName)
        {
            // this will throws error if container exists
            //await _blobServiceClient.CreateBlobContainerAsync(containerName);
            
            // Or
            // this will create container if not exists. it will not throws any errors
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var result = await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            return result;
        }

    }

}
