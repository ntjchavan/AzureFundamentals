using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionApp
{
    public static class HttpTriggerFunctions
    {
        [FunctionName("get-hello-world")]
        public static async Task<IActionResult> GetHelloWorldMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest request,
            ILogger logger
            )
        {
            logger.LogInformation("hello world function execution started");

            string message = request.Query["message"];

            await Task.Delay(100);

            string response = string.IsNullOrEmpty(message)
                ? "HTTP function trigger successfully. Pass message as a query string."
                : $"Hello {message}, how are you?";

            logger.LogInformation("hello world function executed.");
            return new OkObjectResult(response);
        }

        [FunctionName("post-hello-world")]
        public static async Task<IActionResult> PostHelloWorldMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
            ILogger logger
            )
        {
            logger.LogInformation("Post-message execution started.");

            StreamReader reader = new StreamReader(request.Body);
            string body = await reader.ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(body);

            string message = data?.message;

            string response = string.IsNullOrEmpty(message)
                ? "Hello Good morning, how are you? Pass message in request body"
                : $"Hello {data.message}, I am fine.";

            logger.LogInformation("Post-message executed.");
            return new OkObjectResult(response);
        }
    }
}
