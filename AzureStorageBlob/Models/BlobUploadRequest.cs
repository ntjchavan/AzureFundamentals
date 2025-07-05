namespace AzureStorageBlob.Models
{
    public class BlobUploadRequest
    {
        public IFormFile? File { get; set; }
        public string AccessTier { get; set; } = "Hot"; // Hot, Cool Archive
        public string BlobType { get; set; } = "Block"; //Block, Append, Page
    }

    public class BlobUploadMetadataRequest
    {
        public IFormFile? File { get; set; }
        public string ContainerName { get; set; } = string.Empty;
        public Dictionary<string, string> MetaData { get; set; } = new();
    }
}