using AzureStorageBlob.Models;
using AzureStorageBlob.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageBlob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost]
        [Route("upload-blob/{containerName}")]
        public async Task<IActionResult> UploadBlob(string containerName, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file found.");
            }
            var response = await _blobService.UploadBlobAsync(containerName, file);

            return Ok(response);
        }

        [HttpPost]
        [Route("upload-blob-tier/{containerName}")]
        public async Task<IActionResult> UploadBlobWithTier(string containerName, [FromForm] BlobUploadRequest uploadRequest)
        {
            if (uploadRequest.File == null || uploadRequest.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var response = await _blobService.UploadBlobWithTierAsync(containerName, uploadRequest);
            return Ok(new { Message = "Blob uploaded successfully", BlobUrl = response });
        }

        [HttpPost("upload-blob-metadata")]
        public async Task<IActionResult> UploadBlobWithMetadata([FromForm] BlobUploadMetadataRequest blobUpload)
        {
            var response = await _blobService.UploadBlobWithMetadataAsync(blobUpload);

            return Ok(response);
        }

        [HttpGet("blob-list/{containerName}")]
        public async Task<IActionResult> GetBlobList(string containerName)
        {
            var response = await _blobService.GetBlobListAsync(containerName);
            if (!response.Any())
            {
                return NotFound($"No blob found or container '{containerName}' does not exist.");
            }
            return Ok(response);
        }

        [HttpGet("get-blob/{containerName}/{blobName}")]
        public async Task<IActionResult> GetBlobDetails(string containerName, string blobName)
        {
            var response = await _blobService.GetBlobDetailsAsync(containerName, blobName);

            if (string.IsNullOrEmpty(response.Uri))
            {
                return NotFound($"Blob '{blobName}' not found in container '{containerName}'");
            }
            return Ok(response);
        }

        [HttpGet("get-blob-metadata/{containerName}")]
        public async Task<IActionResult> GetBlobMetadata(string containerName, [FromQuery] string blobName)
        {
            var response = await _blobService.GetBlobWithMetadataAsync(containerName, blobName);

            return Ok(new
            {
                BlobName = response.Item1,
                Metadata = response.Item2
            });
        }

        [HttpGet("download/{containerName}/{blobName}")]
        public async Task<IActionResult> DownloadBlob(string containerName, string blobName)
        {
            var (stream, contentType, flag) = await _blobService.DownloadBlobAsync(containerName, blobName);

            if (!flag)
            {
                return NotFound($"Blob {blobName} not found in container {containerName}");
            }

            return File(stream, "image/jpeg", blobName);
        }

        [HttpDelete("blob-delete/{containerName}/{blobName}")]
        public async Task<IActionResult> DeleteBlobAsync(string containerName, string blobName)
        {
            var response = await _blobService.DeleteBlobAsync(containerName, blobName);
            if (response)
            {
                return Ok($"Blob {blobName} deleted from container {containerName}.");
            }
            return NotFound($"Blob {blobName} not fount in container {containerName}.");
        }

        [HttpDelete("delete-multiple-blobs/{containerName}")]
        public async Task<IActionResult> DeleteMultipleBlobsAsync(string containerName, [FromBody] List<string> blobNames)
        {
            var response = await _blobService.DeleteBlobsAsync(containerName, blobNames);

            var blobs = new { Deleted = response, Message = $"Deleted {response.Count} blobs from your container {containerName}" };
            return Ok(blobs);
        }

        // delete multile blobs

    }
}
