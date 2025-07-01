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
    }
}
