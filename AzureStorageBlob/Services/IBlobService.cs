using AzureStorageBlob.Models;

namespace AzureStorageBlob.Services
{
    public interface IBlobService
    {
        Task<string> UploadBlobAsync(string containerName, IFormFile file);
        Task<BlobDetails> GetBlobDetailsAsync(string containerName, string blobName);
        Task<List<string>> GetBlobListAsync(string containerName);
        Task<(Stream Content, string ContentType, bool IsBlobAvailable)> DownloadBlobAsync(string containerName, string blobName);
        Task<bool> DeleteBlobAsync(string containerName, string blobName);
        Task<List<string>> DeleteBlobsAsync(string containerName, List<string> blobNames);
    }
}
