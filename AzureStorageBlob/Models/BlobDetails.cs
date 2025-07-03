namespace AzureStorageBlob.Models
{
    public class BlobDetails
    {
        public string Name { get; set; } = string.Empty;
        public long? Size { get; set; }
        public string? ContentType { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public string Uri { get; set; } = string.Empty;
    }
}
