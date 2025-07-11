using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorageBlob.Models;

namespace AzureStorageBlob.Services
{
    public class ContainerService : IContainerService
    {
        BlobServiceClient _blobServiceClient;
        //public BlobStorageService(IConfiguration configuration)
        //{
        //    var connectionString = configuration["AzureConnection:BlobStorageConnection"];
        //    _blobServiceClient = new BlobServiceClient(connectionString);
        //}

        //To use like below, you need to register connection string in program.cs file.
        public ContainerService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<ContainerCreationResult> CreateContainerAsync(string containerName)
        {
            // this will throws error if container exists
            //await _blobServiceClient.CreateBlobContainerAsync(containerName);

            // Or
            // this will create container if not exists. it will not throws any errors
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var result = await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var response = new ContainerCreationResult
            {
                ContainerName = containerName,
                IsNewlyCreated = result != null,
                Message = result != null ? $"Container {containerName} created successfully" : $"Container {containerName} already exists."
            };
            return response;
        }

        public async Task<List<string>> GetAllContainerNamesAsync()
        {
            var containerList = new List<string>();

            await foreach (var container in _blobServiceClient.GetBlobContainersAsync())
            {
                containerList.Add(container.Name);
            }
            return containerList;
        }

        public async Task<bool> DeleteContainerAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var result = await containerClient.DeleteIfExistsAsync();

            return result;
        }

        public async Task<bool> SetContainerAccessLevelAsync(string containerName, PublicAccessType accessType)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!await containerClient.ExistsAsync())
            {
                return false;
            }

            await containerClient.SetAccessPolicyAsync(accessType);
            return true;
        }
    }

}
