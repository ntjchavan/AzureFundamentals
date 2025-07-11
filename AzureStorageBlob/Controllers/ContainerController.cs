using Azure.Storage.Blobs.Models;
using AzureStorageBlob.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageBlob.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        private readonly IContainerService _blobStorageService;

        public ContainerController(IContainerService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpPost]
        [Route("create-container/{containerName}")]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            var response = await _blobStorageService.CreateContainerAsync(containerName);

            if (response.IsNewlyCreated)
            {
                return CreatedAtAction(nameof(CreateContainer), new { containerName }, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("create-container-param")]
        public async Task<IActionResult> CreateContainerParameter(string containerName)
        {
            var response = await _blobStorageService.CreateContainerAsync(containerName);

            if (response.IsNewlyCreated)
            {
                return CreatedAtAction(nameof(CreateContainer), new { containerName }, response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("create-container-body")]
        public async Task<IActionResult> CreateContainerBody([FromBody] string containerName)
        {
            var response = await _blobStorageService.CreateContainerAsync(containerName);

            if (response.IsNewlyCreated)
            {
                return CreatedAtAction(nameof(CreateContainer), new { containerName }, response);
            }

            return Ok(response);
        }

        [HttpGet("container-list")]
        public async Task<IActionResult> GetContainerNamesList()
        {
            var response = await _blobStorageService.GetAllContainerNamesAsync();

            return Ok(response);
        }


        [HttpDelete("delete-container")]
        public async Task<IActionResult> DeleteContainer(string containerName)
        {
            var respone = await _blobStorageService.DeleteContainerAsync(containerName);

            return Ok(respone);
        }

        [HttpPost("update-access")]
        public async Task<IActionResult> UpdateContaienerAccess(string containerName, string accessLevel)
        {
            if (!Enum.TryParse<PublicAccessType>(accessLevel, true, out var parsedAccess))
            {
                return BadRequest("Invalid access level. Use: None, Blob or BlobContainer");
            }
            var response = await _blobStorageService.SetContainerAccessLevelAsync(containerName, parsedAccess);
            return Ok(response);
        }

    }
}
