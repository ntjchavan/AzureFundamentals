using APIWebApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IAzureFunctionService _azureFunction;

        public TestController(IAzureFunctionService azureFunction)
        {
            _azureFunction = azureFunction;
        }

        [HttpPost]
        [Route("post-execute-function")]
        public async Task<IActionResult> ExecuteFunction([FromBody] object obj)
        {
            var response = await _azureFunction.CallAzureFunction(obj);

            return Ok(response);
        }
    }
}
