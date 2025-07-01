namespace AzureStorageBlob.Services
{
    public interface IBlobService
    {
        Task<string> UploadBlobAsync(string containerName, IFormFile file);
    }
}
