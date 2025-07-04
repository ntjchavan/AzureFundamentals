namespace AzureStorageBlob.Models
{
    public class BlobUploadRequest
    {
        public IFormFile File {  get; set; }
        public string AccessTier { get; set; } = "Hot"; // Hot, Cool Archive
        public string BlobType { get; set; } = "Block"; //Block, Append, Page
    }
}
