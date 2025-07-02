namespace AzureStorageBlob.Services
{
    public interface IBlobService
    {
        Task<string> UploadBlobAsync(string containerName, IFormFile file);
        Task<List<string>> GetBlobListAsync(string containerName);
        Task<bool> DeleteBlobAsync(string containerName, string blobName);
    }
}
