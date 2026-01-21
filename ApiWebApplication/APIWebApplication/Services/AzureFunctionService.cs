namespace APIWebApplication.Services
{

    public interface IAzureFunctionService
    {
        Task<string> CallAzureFunction(object request);
    }

    public class AzureFunctionService : IAzureFunctionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AzureFunctionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> CallAzureFunction(object request)
        {
            // Function name and key can be read from config
            var functionName = _configuration["AzureFunction:FunctionName"];
            var functionKey = _configuration["AzureFunction:FunctionKey"];

            var url = string.IsNullOrEmpty(functionKey) 
                ? functionName 
                : $"{functionName}?code={functionKey}";

            var response = await _httpClient.PostAsJsonAsync(url, request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
