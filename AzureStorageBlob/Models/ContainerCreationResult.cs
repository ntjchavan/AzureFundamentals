namespace AzureStorageBlob.Models
{
    public class ContainerCreationResult
    {
        public string ContainerName { get; set; } = string.Empty;
        public bool IsNewlyCreated { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
