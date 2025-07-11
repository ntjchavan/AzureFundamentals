using Azure.Storage.Blobs.Models;
using AzureStorageBlob.Models;

namespace AzureStorageBlob.Services
{
    public interface IContainerService
    {
        Task<ContainerCreationResult> CreateContainerAsync(string containerName);
        Task<List<string>> GetAllContainerNamesAsync();
        Task<bool> DeleteContainerAsync(string containerName);
        Task<bool> SetContainerAccessLevelAsync(string containerName, PublicAccessType accessType);
    }

}
