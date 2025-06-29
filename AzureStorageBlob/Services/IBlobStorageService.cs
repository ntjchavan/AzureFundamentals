using Azure.Storage.Blobs.Models;

namespace AzureStorageBlob.Services
{
    public interface IBlobStorageService
    {
        Task<BlobContainerInfo> CreateContainerAsync(string containerName);
    }

}
