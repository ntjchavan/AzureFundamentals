using AzureStorageBlob.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageBlob.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;

        public ContainerController(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpPost]
        [Route("create-container/{containerName}")]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            var response = await _blobStorageService.CreateContainerAsync(containerName);

            return Ok(response);
        }
    }
}
