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

        // delete multile blobs

    }
}
